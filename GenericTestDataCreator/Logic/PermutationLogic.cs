using System;
using GenericTestDataCreator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTestDataCreator.Logic
{
    class PermutationLogic
    {
        public static int GetPermutationCount(DataGenerationRequest request)
        {
            int permutationCount = 0;
            List<int> tablePermutationCounts = new();
            

            if (!request.SelectedTables.Any() && !request.AllTables.Any())
            {
                return permutationCount;
            }

            var tables = request.SelectedTables.Any() ? request.SelectedTables : request.AllTables;

            foreach (var table in tables)
            {
                List<int> columnPermutationCounts = new();
                foreach (var column in table.Columns)
                {
                    int columnPermutationCount = 1;
                    if (column.IsNullable)
                    {
                        columnPermutationCount += 1;
                    }
                    if (column.Type == "boolean")
                    {
                        columnPermutationCount += 1;
                    }
                    columnPermutationCounts.Add(columnPermutationCount);
                }
                int tablePermutationCount = columnPermutationCounts[0];

                for (int i = 1; i < columnPermutationCounts.Count; i++)
                {
                    tablePermutationCount *= columnPermutationCounts[i];
                }

                tablePermutationCounts.Add(tablePermutationCount);

            }

            permutationCount = tablePermutationCounts[0];

            for (int i = 1; i < tablePermutationCounts.Count; i++)
            {
                permutationCount *= tablePermutationCounts[i];
            }

            return permutationCount;
        }
    }
}
