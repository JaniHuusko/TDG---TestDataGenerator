using GenericTestDataCreator.Logic;
using GenericTestDataCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GenericTestDataCreator
{
    public class ConsoleUI
    {
        static int seconds = 0;

        public void GenerateDataQuery()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
           
            var dataGenerationLogic = new DataGenerationLogic();
            var queryLogic = new QueryLogic(dataGenerationLogic);
            var saveLogic = new SaveLogic();
            bool success = false;

            while (success != true)
            {
                Console.WriteLine("Insert connection string:");
                string? input = Console.ReadLine();

                if (input == null)
                {
                    Console.WriteLine("Connection string cant be null.");
                    continue;
                }

                DataGenerationRequest request = new() { ConnectionString = input, Tables = QueryLogic.GetTables(input).Where(t => t.Name != "sysdiagrams").ToList() };

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
                    request.ImportTable = new ImportTable { Name = tableName, Columns = request.Tables.First(t => t.Name == tableName).Columns };
                }

                Console.WriteLine("How many rows of data would you like to create?\n");
                string? rows = Console.ReadLine();
                Console.WriteLine();

                bool count = int.TryParse(rows, out int rowCount);
                if (count && rowCount > 0)
                {
                    Console.WriteLine("Started to generate data");
                    aTimer.Enabled = true;
                    aTimer.Start();
                    request.DataRowCount = rowCount;
                    var tables = queryLogic.CreateRows(request);
                    SaveLogic.SaveData(tables, rowCount, request.ConnectionString);
                    Console.WriteLine();
                    Console.WriteLine($"Successfully created {rowCount} rows to the database.");
                    success = true;
                }
                else
                {
                    Console.WriteLine("Input wasn't an integer.\n");
                }

                aTimer.Stop();
                Console.WriteLine($"Saving data took {seconds} seconds.");

            }
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.Write(".");
            seconds++;
        }
    }
}
