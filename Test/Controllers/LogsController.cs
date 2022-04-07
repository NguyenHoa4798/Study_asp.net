using LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private ILoggerManager _logger;
        public LogsController(ILoggerManager logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Here is info message from our values controller.");
            _logger.LogDebug("Here is debug message from our values controller.");
            _logger.LogWarn("Here is warn message from our values controller.");
            _logger.LogError("Here is an error message from our values controller.");
            return new string[] { "value1", "value2" };
        }
    }
}
