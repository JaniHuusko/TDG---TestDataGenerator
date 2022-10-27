﻿using GenericTestDataCreator.Models;

namespace GenericTestDataCreator.Logic
{
    public class PermutationLogic
    {
        public static int GetPermutationCount(DataGenerationRequest request, List<ColumnConfiguration<T>> configuration)
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
                    column.PermutationCount = columnPermutationCount;
                    columnPermutationCounts.Add(columnPermutationCount);
                }

                int tablePermutationCount = columnPermutationCounts[0];

                for (int i = 1; i < columnPermutationCounts.Count; i++)
                {
                    tablePermutationCount *= columnPermutationCounts[i];
                }

                tablePermutationCounts.Add(tablePermutationCount);

            }
            // if not set for whole schema, gives permutation count per highest in table.
            if (!request.PermutationConfiguration.PermutateWholeSchema)
            {
                permutationCount = tablePermutationCounts.Max();
            }
            else
            {
                // This needs to take foreign keys into account.
                permutationCount = tablePermutationCounts[0];

                for (int i = 1; i < tablePermutationCounts.Count; i++)
                {
                    permutationCount *= tablePermutationCounts[i];
                }
            }

            return permutationCount;
        }
    }
}