using GenericTestDataCreator.Models;
using System.Text;

namespace GenericTestDataCreator
{
    public class DataGenerationLogic
    {
        string? shortString = null;
        string longString = "Longest string ";
        int? smallInteger = null;
        long? bigInteger = null;
        bool IsNullableAdded;

        private static readonly Random random = new();

        public List<ExportTable> GenerateData(DataGenerationRequest request)
        {
            List<ExportTable> exportTables = new();

            foreach (var table in request.AllTables)
            {
                exportTables.Add(GenerateTable(table, request.DataRowCount));
            }

            return exportTables;
        }

        public ExportTable GenerateTable(ImportTable importTable, int? dataRowCount)
        {
            ExportTable exportTable = new() { Name = importTable.Name };
            List<string> columnNames = new();

            foreach (var column in importTable.Columns.Where(c => c.IsGenerated!= true))
            {
                columnNames.Add(column.Name);
            }

            columnNames = columnNames.Distinct().ToList();

            foreach (var columnName in columnNames)
            {
                exportTable.Columns.Add(new() { Name = columnName });
            }

            foreach (var column in importTable.Columns.Where(c => c.IsGenerated))
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
                bigInteger = null;
                smallInteger = null;
                IsNullableAdded = false;
            }

            return exportTable;
        }

        private string? CreateDateTimeColumn(ImportColumn column)
        {
            string value = string.Empty;

            if (column.IsGenerated)
            {
                return null;

            }
            else if (column.IsNullable == true && IsNullableAdded == false)
            {
                value = "null";
                IsNullableAdded = true;
                return value;
            }

            DateTime endDate = new(2070, 12, 31);
            DateTime startDate = new(1950, 1, 1);
            TimeSpan timeSpan = endDate - startDate;
            TimeSpan newSpan = new(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
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
            int randomIntegerPool = 1;
            switch (column.Type)
            {
                case "float":
                    randomIntegerPool = random.Next(1, 3);
                    break;
                case "decimal":
                    randomIntegerPool = random.Next(1, 4);
                    break;
                default:
                    break;
            }
            
            if (randomIntegerPool == 1) value = random.Next(1, 99).ToString();
            if (randomIntegerPool == 2) value = random.Next(1, 32767).ToString();
            if (randomIntegerPool == 3) value = random.Next(1, 8388607).ToString();
            if (randomIntegerPool == 3) value = random.Next(1, int.MaxValue).ToString();

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

            if (bigInteger == null)
            {
                switch (column.Type)
                {
                    case "tinyint": bigInteger = 127; break;
                    case "smallint": bigInteger = 32767; break;
                    case "mediumint": bigInteger = 8388607; break;
                    case "int": bigInteger = int.MaxValue; break;
                    case "bigint": bigInteger = long.MaxValue; break;
                    default:
                        break;
                }
                
                value = bigInteger.ToString();

                return value;
            }
            else if (smallInteger == null)
            {
                smallInteger = 1;
                value = smallInteger.ToString();

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
            int maxLength;
            string value = "";

            if (column.Type == "nvarchar")
            {
                maxLength = column.MaxLength / 2;
            }
            else
            {
                maxLength = column.MaxLength;
            }

            

            if (longString == "Longest string ")
            {
                longString = $"{longString}{GenerateRandomString(4000, 8000, 5, 20, 10)}";

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

        private static string GenerateRandomString(int minWords, int maxWords, int minSentences, int maxSentences, int numberOfLines)
        {
            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", 
                                "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", 
                                "magna", "aliquam", "erat" };

            int numberOfSentences = random.Next(maxSentences - minSentences)
                + minSentences;
            int numWords = random.Next(maxWords - minWords) + minWords;

            StringBuilder stringBuilder = new();
            for (int paragraphCount = 0; paragraphCount < numberOfLines; paragraphCount++)
            {
                for (int sentenceCount = 0; sentenceCount < numberOfSentences; sentenceCount++)
                {
                    for (int wordCount = 0; wordCount < numWords; wordCount++)
                    {
                        if (wordCount > 0) { stringBuilder.Append(' '); }
                        string word = words[random.Next(words.Length)];
                        if (wordCount == 0) { word = string.Concat(word[..1].Trim().ToUpper(), word.AsSpan(1)); }
                        stringBuilder.Append(word);
                    }
                    stringBuilder.Append(". ");
                }
                if (paragraphCount < numberOfLines - 1) stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}