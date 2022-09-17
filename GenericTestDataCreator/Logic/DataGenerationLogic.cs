using GenericTestDataCreator.Models;
using System;
using System.Data.Common;
using System.Text;

namespace GenericTestDataCreator
{
    public class DataGenerationLogic
    {
        string? shortString = null;
        string longString = "Longest string ";
        int? smallInt = null;
        long? bigInt = null;
        bool IsNullableAdded;

        private static readonly Random random = new();

        public List<ExportTable> GenerateData(DataGenerationRequest request)
        {
            var exportTables = new List<ExportTable>();

            foreach (var table in request.Tables)
            {
                exportTables.Add(GenerateTable(table, request.DataRowCount));
            }

            return exportTables;
        }

        public ExportTable GenerateTable(ImportTable importTable, int? dataRowCount)
        {
            var exportTable = new ExportTable { Name = importTable.Name };
            var columnList = new List<string>();
            foreach (var column in importTable.Columns.Where(c => c.IsIdentity != true && c.IsComputed != true))
            {
                columnList.Add(column.Name);
            }

            columnList = columnList.Distinct().ToList();

            foreach (var item in columnList)
            {
                exportTable.Columns.Add(new ExportColumn { Name = item, Values = new List<string?>() });
            }

            foreach (var column in importTable.Columns.Where(c => c.IsIdentity != true && c.IsComputed != true))
            {
                if (column.ForeignKeyInfo != null)
                {
                    exportTable.Columns.Where(c => c.Name == column.Name).First().ForeignKeyInfo = column.ForeignKeyInfo;
                    exportTable.Columns.Where(c => c.Name == column.Name).First().IsNullable = column.IsNullable;
                    exportTable.ForeignKeyCount = importTable.ForeignKeyCount;
                    continue;
                }
                for (int i = 0; i < dataRowCount; i++)
                {
                    string? value = string.Empty;

                    switch (column.Type)
                    {
                        case "bit":
                            int randomBit;
                            if (column.IsNullable)
                            {
                                randomBit = random.Next(1, 3);
                            }
                            else
                            {
                                randomBit = random.Next(1, 2);
                            }
                            if (randomBit == 1) value = "true";
                            else if (randomBit == 2) value = "false";
                            else value = "null";
                            break;
                        case "nvarchar":
                        case "varchar":
                            value = CreateStringColumn(column);
                            break;
                        case "tinyint":
                        case "smallint":
                        case "mediumint":
                        case "int":
                        case "bigint":
                            value = CreateIntegerColumn(column);
                            break;
                        case "float":
                        case "decimal":
                            value = CreateFloatColumn(column);
                            break;
                        case "date":
                        case "datetime":
                        case "datetime2":
                            value = CreateDateTimeColumn(column);
                            break;
                        case "binary":
                        case "varbinary":
                            value = "null";
                            break;
                        default:
                            break;
                    }
                    if (value != null)
                    {
                        exportTable.Columns.Where(c => c.Name == column.Name).First().Values.Add(value);
                    }
                }

                longString = "Longest string ";
                shortString = null;
                bigInt = null;
                smallInt = null;
                IsNullableAdded = false;
            }

            return exportTable;
        }

        private string? CreateDateTimeColumn(ImportColumn column)
        {
            string value = string.Empty;

            if (column.IsComputed || column.IsIdentity)
            {
                return null;

            }
            else if (column.IsNullable == true && IsNullableAdded == false)
            {
                value = "null";
                IsNullableAdded = true;
                return value;
            }
            var endDate = new DateTime(2070, 12, 31);
            var startDate = new DateTime(1950, 1, 1);
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
            DateTime newDate = startDate + newSpan;
            switch (column.Type)
            {
                case "date":
                    value = newDate.ToString("yyyy-MM-dd");
                    break;
                case "datetime":
                case "datetime2":
                    value = newDate.ToString("yyyy-MM-dd HH:mm:ss");
                    value = value.Replace('.', ':');
                    break;
                default:
                    break;
            }
            return value;
        }

        private string? CreateFloatColumn(ImportColumn column)
        {
            string? value = string.Empty;

            if (column.IsNullable == true && IsNullableAdded == false)
            {
                value = "null";
                IsNullableAdded = true;
            }
            int randomFloatPool = 1;
            switch (column.Type)
            {
                case "float":
                    randomFloatPool = random.Next(1, 3);
                    break;
                case "decimal":
                    randomFloatPool = random.Next(1, 4);
                    break;
                default:
                    break;
            }
            
            if (randomFloatPool == 1) value = random.Next(1, 99).ToString();
            if (randomFloatPool == 2) value = random.Next(1, 32767).ToString();
            if (randomFloatPool == 3) value = random.Next(1, 8388607).ToString();
            if (randomFloatPool == 3) value = random.Next(1, int.MaxValue).ToString();

            int randomDecimalPool = random.Next(0, 3);

            switch (randomDecimalPool)
            {
                case 1: value += '.' + random.Next(1, 9).ToString(); break;
                case 2: value += '.' + random.Next(0, 99).ToString(); break;
                default:
                    break;
            }

            return value;
        }

        private string? CreateIntegerColumn(ImportColumn column)
        {
            string? value = string.Empty;

            if (bigInt == null)
            {
                switch (column.Type)
                {
                    case "tinyint": bigInt = 127; break;
                    case "smallint": bigInt = 32767; break;
                    case "mediumint": bigInt = 8388607; break;
                    case "int": bigInt = int.MaxValue; break;
                    case "bigint": bigInt = long.MaxValue; break;
                    default:
                        break;
                }
                
                value = bigInt.ToString();

                return value;
            }
            else if (smallInt == null)
            {
                smallInt = 1;
                value = smallInt.ToString();

                return value;
            }
            else if (column.IsNullable == true && IsNullableAdded == false)
            {
                value = "null";
                IsNullableAdded = true;

                return value;
            }

            int randomIntPool = 1;
            switch (column.Type)
            {
                case "tinyint": break;
                case "smallint": randomIntPool = random.Next(1, 2); break;
                case "mediumint": randomIntPool = random.Next(1, 3); break;
                case "int": randomIntPool = random.Next(1, 4); break;
                case "bigint": randomIntPool = random.Next(1, 5); break;
                default:
                    break;
            }

            if (randomIntPool == 1) value = random.Next(1, 127).ToString();
            if (randomIntPool == 2) value = random.Next(1, 32767).ToString();
            if (randomIntPool == 3) value = random.Next(1, 8388607).ToString();
            if (randomIntPool == 4) value = random.Next(1, int.MaxValue).ToString();
            if (randomIntPool == 5) value = random.NextInt64(1, long.MaxValue).ToString();

            return value;
        }

        private string? CreateStringColumn(ImportColumn column)
        {
            int maxLength = 0;

            if (column.Type == "nvarchar")
            {
                maxLength = column.MaxLength / 2;
            }
            else
            {
                maxLength = column.MaxLength;
            }

            string value = "";

            if (longString == "Longest string ")
            {
                longString = $"{longString}{GenerateRandomString(2000, 8000, 5, 20, 10)}";

                value = longString;

            }
            else if (shortString == null)
            {
                shortString = "s";
                value = shortString;
            }
            else if (column.IsNullable && IsNullableAdded == false)
            {
                value = "null";
                IsNullableAdded = true;
            }
            else
            {
                int randomStringPool = 1;
                switch (maxLength)
                {
                    case < 20:
                        break;
                    case < 50:
                        randomStringPool = random.Next(1, 2);
                        break;
                    case < 100:
                        randomStringPool = random.Next(1, 3);
                        break;
                    case < 200:
                        randomStringPool = random.Next(1, 4);
                        break;
                    case 500:
                        randomStringPool = random.Next(1, 5);
                        break;
                    case 1000:
                        randomStringPool = random.Next(1, 6);
                        break;
                    case 2000:
                        randomStringPool = random.Next(1, 7);
                        break;
                    case 4000:
                        randomStringPool = random.Next(1, 8);
                        break;
                    case 8000:
                        randomStringPool = random.Next(1, 9);
                        break;
                    default:
                        break;
                }

                if (randomStringPool == 1) value = GenerateRandomString(1, 4, 1, 1, 1);
                if (randomStringPool == 2) value = GenerateRandomString(4, 10, 1, 1, 1);
                if (randomStringPool == 3) value = GenerateRandomString(10, 20, 1, 1, 1);
                if (randomStringPool == 4) value = GenerateRandomString(20, 40, 1, 1, 1);
                if (randomStringPool == 5) value = GenerateRandomString(40, 100, 2, random.Next(2, 10), random.Next(1, 3));
                if (randomStringPool == 6) value = GenerateRandomString(100, 200, 5, random.Next(5, 20), random.Next(1, 5));
                if (randomStringPool == 7) value = GenerateRandomString(200, 800, 10, random.Next(10, 40), random.Next(1, 10));
                if (randomStringPool == 8) value = GenerateRandomString(800, 1600, 20, random.Next(20, 80), random.Next(1, 20));

            }
            if (value.Length >= maxLength)
            {
                value = value.Remove(maxLength);
            }

            return value;
        }

        private static string GenerateRandomString(int minWords, int maxWords, int minSentences, int maxSentences, int numLines)
        {
            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", 
                                "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", 
                                "magna", "aliquam", "erat" };

            int numSentences = random.Next(maxSentences - minSentences)
                + minSentences;
            int numWords = random.Next(maxWords - minWords) + minWords;

            var sb = new StringBuilder();
            for (int p = 0; p < numLines; p++)
            {
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { sb.Append(' '); }
                        string word = words[random.Next(words.Length)];
                        if (w == 0) { word = string.Concat(word.Substring(0, 1).Trim().ToUpper(), word.AsSpan(1)); }
                        sb.Append(word);
                    }
                    sb.Append(". ");
                }
                if (p < numLines - 1) sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}