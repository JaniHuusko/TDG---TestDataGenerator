using GenericTestDataCreator.Logic;
using GenericTestDataCreator.Models;
using System.Text;
using System.Timers;

namespace GenericTestDataCreator
{
    public class ConsoleUI
    {
        static int seconds = 0;

        public static void GenerateDataQuery()
        {
            System.Timers.Timer aTimer = new(1000);
            aTimer.Elapsed += OnTimedEvent;

            DataGenerationLogic dataGenerationLogic = new();
            QueryLogic queryLogic = new(dataGenerationLogic);
            string? connectionString = null;
            bool appIsOn = true;

            while (connectionString == null)
            {
                StringBuilder input = new();
                Console.WriteLine("Insert connection string:");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter) break;
                    if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                    else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar); 
                }

                connectionString = input.ToString();

                if (connectionString == null || connectionString == String.Empty)
                {
                    Console.WriteLine("Connection string not valid.");
                    connectionString = null;
                    continue;
                }
                Console.WriteLine();
            }

            while (appIsOn == true)
            {
                DataGenerationRequest request = new();
                if (connectionString != null)
                {
                    request = new() { ConnectionString = connectionString, Tables = QueryLogic.GetTables(connectionString).ToList() };
                }

                Console.WriteLine("Current tables:");
                foreach (var table in request.Tables)
                {
                    Console.WriteLine($"\t{table.Name} {table.CurrentDataRowCount}");
                }

                Console.WriteLine("Write the name of the table where you want to write data, or press Enter to write to all tables.\n");
                string? tableName = Console.ReadLine();

                if (!(tableName == null || tableName == string.Empty) && !request.Tables.Any(f => f.Name == tableName))
                {
                    Console.WriteLine($"Schema doesnt contain {tableName}.");
                    continue;
                }
                else if (tableName != null && tableName != string.Empty)
                {
                    request.ImportTable = new() { Name = tableName, Columns = request.Tables.First(t => t.Name == tableName).Columns };
                }

                Console.WriteLine("How many rows of data would you like to create?\n");
                string? rows = Console.ReadLine();
                Console.WriteLine();
                bool isNumber = int.TryParse(rows, out int rowCount);

                if (!isNumber || rowCount <= 0)
                {
                    Console.WriteLine("Input wasn't an integer.\n");
                    continue;
                }

                Console.WriteLine("Started to generate data");
                aTimer.Enabled = true;
                aTimer.Start();

                request.DataRowCount = rowCount;
                var tables = queryLogic.CreateRows(request);
                Console.WriteLine();
                Console.WriteLine($"Created test data in {seconds} seconds.");

                SaveLogic.SaveData(tables, rowCount, request.ConnectionString);

                Console.WriteLine();
                Console.WriteLine($"Successfully created {rowCount} rows to the database in {seconds} seconds.");
                Console.WriteLine();
                aTimer.Stop();
                seconds = 0;

                Console.WriteLine("Type quit if you want to quit.");
                string? quit = Console.ReadLine();
                if (quit != "quit") continue;
                else appIsOn = false;
            }
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.Write(".");
            seconds++;
        }
    }
}
