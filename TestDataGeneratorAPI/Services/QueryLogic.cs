using System.Data.SqlClient;
using TestDataGeneratorAPI.Models;

namespace TestDataGeneratorAPI.Services
{
    public class QueryLogic
    {
        public static DataGenerationRequest GetDataGenerationRequest(string connectionString)
        {
            DataGenerationRequest request = GetDataGenerationRequest(connectionString);

            request = ConfigurationLogic.AddConfigurationsToRequest(request);

            request.PermutationCount = PermutationLogic.GetPermutationCount(request);

            request.DataRowCount ??= request.PermutationCount;

            return request;
        }

        public static List<ImportColumn> GetColumnModels(ImportTable importTable, SqlConnection connection)
        {

            using (SqlCommand command = new(@"SELECT TAB.name as table_name, 
                                            COL.name AS column_name, 
                                            TYP.name AS data_type_name, 
                                            COL.max_length,
                                            COL.is_nullable, 
                                            COL.is_computed,
											COL.is_identity,
											COL.precision,
											COL.scale
                                            FROM sys.columns COL
                                            INNER JOIN sys.tables TAB
                                            On COL.object_id = TAB.object_id
                                            INNER JOIN sys.types TYP
                                            ON TYP.user_type_id = COL.user_type_id
											Where TAB.name = @tableName", connection))
            {
                command.Parameters.AddWithValue("@tableName", importTable.Name);
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var column = new ImportColumn
                    {
                        Name = (string)reader["column_name"],
                        MaxLength = Convert.ToInt32(reader["max_length"]),
                        IsGenerated = (bool)reader["is_computed"] ? (bool)reader["is_computed"] : (bool)reader["is_identity"],
                        IsNullable = (bool)reader["is_nullable"],
                        Type = (string)reader["data_type_name"],
                        NumericPrecision = (byte)reader["precision"],
                        NumericScale = (byte)reader["scale"],

                    };
                    importTable.Columns.Add(column);
                }
            }
            return importTable.Columns;
        }

        private static List<string> GetAllTableNamesFromDatabase(SqlConnection connection)
        {
            List<string> tableNames = new();

            using (SqlCommand command = new(@"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
                                              WHERE TABLE_NAME != 'sysdiagrams' AND TABLE_NAME != 'database_firewall_rules'", connection))
            {
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string tableName = (string)reader["TABLE_NAME"];
                    tableNames.Add((string)reader["TABLE_NAME"]);
                }
            }
            return tableNames;
        }
        private static List<ImportTable> GetForeignKeyInfo(DataGenerationRequest request)
        {
            using SqlConnection connection = new(request.ConnectionString);
            connection.Open();

            using SqlCommand command2 = new(@"SELECT 
                                             KCU1.TABLE_NAME AS 'FK_TABLE_NAME',
                                             KCU1.COLUMN_NAME AS 'FK_COLUMN_NAME',
                                             KCU2.TABLE_NAME AS 'UQ_TABLE_NAME',
                                             KCU2.COLUMN_NAME AS 'UQ_COLUMN_NAME'
                                             FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
                                             JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1
                                             ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG 
                                             AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA
                                             AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME
                                             JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2
                                             ON KCU2.CONSTRAINT_CATALOG = 
                                             RC.UNIQUE_CONSTRAINT_CATALOG 
                                             AND KCU2.CONSTRAINT_SCHEMA = 
                                             RC.UNIQUE_CONSTRAINT_SCHEMA
                                             AND KCU2.CONSTRAINT_NAME = 
                                             RC.UNIQUE_CONSTRAINT_NAME", connection);
            using SqlDataReader reader = command2.ExecuteReader();
            while (reader.Read())
            {
                if (request.Tables.Where(t => t.Name == (string)reader["FK_TABLE_NAME"]).FirstOrDefault() != null)
                {
                    var table = request.Tables.Where(t => t.Name == (string)reader["FK_TABLE_NAME"]).First();
                    table.ForeignKeyCount++;
                    var column = table.Columns.Where(c => c.Name == (string)reader["FK_COLUMN_NAME"]).First();
                    column.ForeignKeyInfo = new ForeignKeyInfo { TableName = (string)reader["UQ_TABLE_NAME"], ColumnName = (string)reader["UQ_COLUMN_NAME"] };
                }
            }
            connection.Close();
            return request.Tables;
        }
        private static List<ImportTable> SortTables(List<ImportTable> tables)
        {
            tables = tables.OrderBy(t => t.ForeignKeyCount).ToList();

            var returnTables = tables;
            foreach (var table in tables)
            {
                IEnumerable<string?> tableNames = table.Columns.Select(c => c.ForeignKeyInfo?.TableName).OfType<string>();
                foreach (var tableName in tableNames)
                {
                    var returnTable = returnTables.Where(t => t.Name == tableName).First();
                    if (returnTables.IndexOf(table) < returnTables.IndexOf(returnTable))
                    {
                        returnTables.Remove(table);
                        returnTables.Insert(returnTables.IndexOf(returnTable) + 1, table);
                    }
                }
            }

            return returnTables;
        }

        public static List<int?> GetForeignKeys(ForeignKeyInfo foreignKeyInfo, string connectionString)
        {
            var keyList = new List<int?>();
            using SqlConnection connection = new(connectionString);
            connection.Open();
            {
                using SqlCommand sqlCommand = new($"SELECT {foreignKeyInfo.ColumnName} FROM {foreignKeyInfo.TableName}", connection);
                using SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    keyList.Add((int)reader[foreignKeyInfo.ColumnName]);
                }
            }
            connection.Close();

            return keyList;
        }

        private DataGenerationRequest GetDatabaseDetails(string connectionString)
        {
            DataGenerationRequest request = new() { ConnectionString = connectionString };
            using SqlConnection connection = new(connectionString);

            // first connection in try catch.
            try
            {
                connection.Open();
            }
            catch (Exception)
            {

                throw new Exception("Connection string is not valid.");
            }
            foreach (var table in GetAllTableNamesFromDatabase(connection))
            {
                using SqlCommand count = new($"SELECT COUNT(*) FROM {table}", connection);
                var importTable = new ImportTable
                {
                    Name = table,
                    CurrentDataRowCount = (int)count.ExecuteScalar()
                };
                importTable.Columns = new List<ImportColumn>(GetColumnModels(importTable, connection));
                request.Tables.Add(importTable);
            }
            connection.Close();

            request.Tables = GetForeignKeyInfo(request);

            if (request.Tables.Count > 1)
            {
                request.Tables = SortTables(request.Tables);
            }

            return request;
        }
    }
}
