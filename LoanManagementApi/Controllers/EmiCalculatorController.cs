using LoanManagementApi.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LoanManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmiCalculatorController : ControllerBase
    {
        private readonly ILogger<EmiCalculatorController> _logger;
        private readonly IHelper _helper;
        public EmiCalculatorController(ILogger<EmiCalculatorController> logger, IHelper helper)
        {
            _logger = logger;
            _helper = helper;
        }
        [HttpPost()]
        public ActionResult CalculateEmi(double principal, double annualInterestRate, int tenureInMonths)
        {
            _logger.LogInformation("Initiating EMI calculation. Principal: {Principal}, Rate: {AnnualInterestRate}%, Tenure: {TenureInMonths} months",
         principal, annualInterestRate, tenureInMonths);

            var emi = _helper.CalculateEmi(principal, annualInterestRate, tenureInMonths);

            _logger.LogInformation("EMI calculation completed successfully. Resulting EMI: {Emi}", emi);

            //  return Ok(emi);
            return Ok(new {Principal=principal,AnnualInterestRate=annualInterestRate,TenureInMonth=tenureInMonths,Emi=emi });
        }
    }
}
