using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaBsReconciliationDetailController : ControllerBase
    {
        private readonly IBaBsReconciliationDetailService _baBsReconciliationDetailService;

        public BaBsReconciliationDetailController(IBaBsReconciliationDetailService baBsReconciliationDetailService)
        {
            _baBsReconciliationDetailService = baBsReconciliationDetailService;
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
                var result = _baBsReconciliationDetailService.AddToExcel(filepath, companyId);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result.Message);
            }
            return BadRequest("Dosya Seçimi Yapmadınız.");
        }

        [HttpPost("add")]
        public IActionResult Add(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailService.Add(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        public IActionResult Update(BaBsReconciliationDetail baBsReconciliation)
        {
            var result = _baBsReconciliationDetailService.Update(baBsReconciliation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("delete")]
        public IActionResult Delete(BaBsReconciliationDetail baBsReconciliationDetail)
        {
            var result = _baBsReconciliationDetailService.Delete(baBsReconciliationDetail);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getById")]
        public IActionResult getById(int id)
        {
            var result = _baBsReconciliationDetailService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getList")]
        public IActionResult GetList(int baBsReconciliationId)
        {
            var result = _baBsReconciliationDetailService.GetList(baBsReconciliationId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
