using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaBsReconciliationController : ControllerBase
    {
        private readonly IBaBsReconciliationService _baBsReconciliationService;

        public BaBsReconciliationController(IBaBsReconciliationService baBsReconciliationService)
        {
            _baBsReconciliationService = baBsReconciliationService;
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
                var result = _baBsReconciliationService.AddToExcel(filepath, companyId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya Seçimi Yapmadınız.");
        }

        [HttpPost("add")]
        public IActionResult Add(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Add(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Update(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("delete")]
        public IActionResult Delete(BaBsReconciliation baBsReconciliation)
        {
            var result = _baBsReconciliationService.Delete(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getById")]
        public IActionResult getById(int id)
        {
            var result = _baBsReconciliationService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int companyId)
        {
            var result = _baBsReconciliationService.GetListDto(companyId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("sendReconciliationMail")]
        public IActionResult SendReconciliationMail(BaBsReconciliationDto baBsReconciliationDto)
        {
            var result = _baBsReconciliationService.SendReconciliationMail(baBsReconciliationDto);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getByCode")]
        public IActionResult GetByCode(string code)
        {
            var result = _baBsReconciliationService.GetByCode(code);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

    }
}

