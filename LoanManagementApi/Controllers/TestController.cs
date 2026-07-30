using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }
        // GET: api/<TestController>
        [HttpGet]
        public void Get()
        {
            _logger.LogInformation("Information Test");
            _logger.LogError("Error Test");
            throw new NotImplementedException();
        }
    }
}
