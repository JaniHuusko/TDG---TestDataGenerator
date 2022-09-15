using GenericTestDataCreator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Logic
{
    public class SaveLogic
    {
        private static readonly Random random = new();
        public void SaveData(List<ExportTable> tables, int rowCount, string connectionString)
        {
            var sqlStatements = new List<string>();

            foreach (var table in tables.Where(t => t.ForeignKeyCount == 0))
            {
                string sqlString = GenerateSqlString(table, rowCount);
                sqlStatements.Add(sqlString);
            }

            SaveStatementsToDataBase(sqlStatements, connectionString);

            if (tables.Any(t => t.ForeignKeyCount > 0))
            {
                SaveForeignKeyTables(tables.Where(t => t.ForeignKeyCount > 0).ToList(), rowCount, connectionString);
            }
        }

        
        private static void SaveStatementsToDataBase(IEnumerable<string> sqlStatements, string connectionString)
        {
            int tableIndex = 0;
            using SqlConnection connection = new(connectionString);
            connection.Open();

            foreach (var statement in sqlStatements)
            {

                using SqlCommand command = new(statement, connection);
                try
                {
                    int recordsAffected = command.ExecuteNonQuery();
                }
                catch (SqlException)
                {
                    throw new Exception($"Statement {statement} failed to save.");
                }

                tableIndex++;
            }

            connection.Close();
        }

        private static string GenerateSqlString(ExportTable table, int rowCount)
        {
            var sqlString = new StringBuilder();
            var sqlColumnsRow = new StringBuilder(@$"INSERT INTO {table.Name} (");

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
                    sqlString.Append(sqlColumnsRow);
                }
            }

            int rowLimit = 999;

            for (int i = 0; i < rowCount; i++)
            {
                if (i > rowLimit)
                {
                    sqlString.Remove(sqlString.Length - 2, 1);
                    rowLimit += 999;
                    sqlString.Append(sqlColumnsRow);

                }
                var sqlValuesRow = new StringBuilder("(");
                foreach (var column in table.Columns)
                {
                    if (column.Values[i] == "null")
                    {
                        sqlValuesRow.Append("null");
                    }
                    else
                    {
                        sqlValuesRow.Append($"'{column.Values[i]}'");
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
                if (i != rowCount - 1)
                {
                    sqlValuesRow.Append(",\n");
                }
               
                sqlString.Append(sqlValuesRow);
            }

            return sqlString.ToString();
        }

        private void SaveForeignKeyTables(List<ExportTable> tables, int rowCount, string connectionString)
        {
            bool isNullableAdded = false;
            foreach (var table in tables)
            {
                var sqlStatements = new List<string>();
                foreach (var column in table.Columns)
                {
                    // Here could be implemented configuration for one to one or one to many relations.
                    if (column.ForeignKeyInfo != null)
                    {
                        var foreignKeyList = GetForeignKeys(column.ForeignKeyInfo, connectionString);

                        for (int i = 0; i < rowCount; i++)
                        {
                            if (!isNullableAdded && column.IsNullable)
                            {
                                foreignKeyList.Add(null);
                                column.Values.Add("null");
                                continue;
                            }
                            int index = random.Next(foreignKeyList.Count());
                            column.Values.Add(foreignKeyList[index].ToString() ?? "null");
                        }
                    }
                }
                string sqlString = GenerateSqlString(table, rowCount);
                sqlStatements.Add(sqlString);
                SaveStatementsToDataBase(sqlStatements, connectionString);
            }
        }

        private List<int?> GetForeignKeys(ForeignKeyInfo foreignKeyInfo, string connectionString)
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
    }
}
