using TestDataGeneratorAPI.Models;

namespace TestDataGeneratorAPI.Services
{
    public class ConfigurationLogic
    {
        public DataGenerationRequest AddColumnConfiguration(DataGenerationRequest request, ColumnConfiguration configuration)
        {
            var column = request.Tables.Select(table => table.Columns.FirstOrDefault(column => column.Id == configuration.Id)).FirstOrDefault();

            if (configuration.OnlyNull)
            {
                column.
            }

            return request;
        }
    }
}