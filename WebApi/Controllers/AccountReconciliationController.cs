using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountReconciliationController : ControllerBase
    {
        private readonly IAccountReconciliationService _accountReconciliationService;

        public AccountReconciliationController(IAccountReconciliationService accountReconciliationService)
        {
            _accountReconciliationService = accountReconciliationService;
        }

        [HttpPost("addFromExel")]
        public IActionResult AddFromExel(IFormFile file, int companyId)
        {
            if (file.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + ".xlsx";
                var filepath = $"{Directory.GetCurrentDirectory()}/Content/{filename}";
                using (FileStream stream = System.IO.File.Create(filepath))
                {
                    file.CopyTo(stream);
                    stream.Flush();
                }
                var result = _accountReconciliationService.AddToExcel(filepath, companyId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya Seçimi Yapmadınız.");
        }

        [HttpPost("add")]
        public IActionResult Add(AccountReconciliation accountReconciliation)
        {
            var result = _accountReconciliationService.Add(accountReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(AccountReconciliation accountReconciliation)
        {
            var result = _accountReconciliationService.Update(accountReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("delete")]
        public IActionResult Delete(AccountReconciliation accountReconciliation)
        {
            var result = _accountReconciliationService.Delete(accountReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getById")]
        public IActionResult GetById(int id)
        {
            var result = _accountReconciliationService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int companyId)
        {
            var result = _accountReconciliationService.GetListDto(companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("sendReconciliationMail")]
        public IActionResult SendReconciliationMail(AccountReconciliationDto accountReconciliationDto)
        {
            var result = _accountReconciliationService.SendReconciliationMail(accountReconciliationDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getByCode")]
        public IActionResult GetByCode(string code)
        {
            var result = _accountReconciliationService.GetByCode(code);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
