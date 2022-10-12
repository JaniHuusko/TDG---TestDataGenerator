using GenericTestDataCreator.Models;
using GenericTestDataCreator.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace GenericTestDataCreator.Logic
{
    public class SaveLogic
    {
        private static readonly Random random = new();

        public static void SaveData(List<ExportTable> tables, int rowCount, string connectionString)
        {
            List<string> sqlStatements = new();
            if (rowCount > 50000)
            {
                SqlConnection connection = new SqlConnection(connectionString);
                foreach (var table in tables.Where(t => t.ForeignKeyCount == 0))
                {
                    string sqlStatement = GenerateSqlString(table, rowCount);
                    connection.Open();
                    SaveStatementToDataBase(GenerateSqlString(table, rowCount), connection);
                    connection.Close();
                }
            }
            else
            {
                foreach (var table in tables.Where(t => t.ForeignKeyCount == 0))
                {
                    sqlStatements.Add(GenerateSqlString(table, rowCount));
                }
                SaveStatementsToDataBase(sqlStatements, connectionString);
            }


            if (tables.Any(t => t.ForeignKeyCount > 0))
            {
                SaveForeignKeyTables(tables.Where(t => t.ForeignKeyCount > 0).ToList(), rowCount, connectionString);
            }
        }

        
        private static void SaveStatementsToDataBase(IEnumerable<string> sqlStatements, string connectionString)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            foreach (var statement in sqlStatements)
            {
                SaveStatementToDataBase(statement, connection);
            }

            connection.Close();
        }

        private static void SaveStatementToDataBase(string statement, SqlConnection connection)
        {
            using SqlCommand command = new(statement, connection);
            try
            {
                int recordsAffected = command.ExecuteNonQuery();
            }
            catch (SqlException)
            {
                throw new Exception($"Statement\n{statement}\nfailed to save.");
            }
        }

        private static string GenerateSqlString(ExportTable table, int rowsToAdd)
        {
            StringBuilder sqlStatement = new();
            StringBuilder sqlColumnsRow = new(@$"INSERT INTO {table.Name} (");

            foreach (var columnName in table.Columns.Select(c => c.Name))
            {
                sqlColumnsRow.Append(columnName);
                if (columnName != table.Columns.Last().Name)
                {
                    sqlColumnsRow.Append(", ");
                }
                else
                {
                    sqlColumnsRow.Append(")\nVALUES");
                    sqlStatement.Append(sqlColumnsRow);
                }
            }

            int rowLimit = 999;

            for (int rowCount = 0; rowCount < rowsToAdd; rowCount++)
            {
                if (rowCount > rowLimit)
                {
                    // Removes comma from the end of the sql row and start new insert if rowcount is maxed.
                    sqlStatement.Remove(sqlStatement.Length - 2, 1);
                    rowLimit += 999;
                    sqlStatement.Append(sqlColumnsRow);

                }
                StringBuilder sqlValuesRow = new("(");
                foreach (var column in table.Columns)
                {
                    if (column.Values[rowCount] == "null")
                    {
                        sqlValuesRow.Append("null");
                    }
                    //else if(column.Type == "decimal" || column.Type == "float")
                    //{
                    //    sqlValuesRow.Append($"{column.Values[rowCount]}");
                    //}
                    else
                    {
                        sqlValuesRow.Append($"'{column.Values[rowCount]}'");
                    }

                    if (column.Name != table.Columns.Last().Name)
                    {
                        sqlValuesRow.Append(", ");
                    }
                    else
                    {
                        sqlValuesRow.Append(')');
                    }
                }

                if (rowCount != rowsToAdd - 1)
                {
                    sqlValuesRow.Append(",\n");
                }

                sqlStatement.Append(sqlValuesRow);
            }

            return sqlStatement.ToString();
        }

        private static void SaveForeignKeyTables(List<ExportTable> tables, int rowCount, string connectionString)
        {
            bool isNullableAdded = false;
            SqlConnection connection = new(connectionString);

            foreach (var table in tables)
            {
                foreach (var column in table.Columns)
                {
                    // Here could be implemented configuration for one to one or one to many relations.
                    if (column.ForeignKeyInfo != null)
                    {
                        List<int?> foreignKeyList = QueryLogic.GetForeignKeys(column.ForeignKeyInfo, connectionString);

                        for (int i = 0; i < rowCount; i++)
                        {
                            if (!isNullableAdded && column.IsNullable)
                            {
                                foreignKeyList.Add(null);
                                column.Values.Add("null");
                                continue;
                            }
                            int index = random.Next(foreignKeyList.Count);
                            column.Values.Add(foreignKeyList[index].ToString() ?? "null");
                        }
                    }
                }
                string sqlStatement = GenerateSqlString(table, rowCount);

                connection.Open();
                SaveStatementToDataBase(sqlStatement, connection);
                connection.Close();
            }
        }
    }
}
