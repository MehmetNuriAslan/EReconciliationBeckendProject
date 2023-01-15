using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermsAndConditionsController : ControllerBase
    {
        private readonly ITermsAndConditionService _termsAndConditionService;
        public TermsAndConditionsController(ITermsAndConditionService termsAndConditionService)
        {
            _termsAndConditionService = termsAndConditionService;
        }

        [HttpPost("update")]
        public IActionResult Update(TermsAndCondition termsAndCondition)
        {
            var result = _termsAndConditionService.Update(termsAndCondition);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpGet("get")]
        public IActionResult Get()
        {
            var result = _termsAndConditionService.Get();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
