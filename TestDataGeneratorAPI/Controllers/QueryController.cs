using Microsoft.AspNetCore.Mvc;
using TestDataGeneratorAPI.Models;
using TestDataGeneratorAPI.Services;

namespace TestDataGeneratorAPI.Controllers
{
    [ApiController]
    public class QueryController : Controller
    {
        ConfigurationLogic configurationLogic = new();
        [HttpGet("api/datagenerationrequest")]
        public ActionResult<DataGenerationRequest> GetDatagenerationrequest(string connectionString)
        {
            return QueryLogic.GetDataGenerationRequest(connectionString);
        }

        [HttpPost("api/columnconfiguration")]
        public ActionResult<DataGenerationRequest> PostConfiguration(DataGenerationRequest request, ColumnConfiguration configuration)
        {
            return configurationLogic.AddColumnConfiguration(request, configuration);
        }
    }
}
