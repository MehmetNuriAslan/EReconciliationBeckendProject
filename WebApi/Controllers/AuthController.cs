using Business.Abstract;
using Core.Utilities.Results.Concrete;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegisterDto)
        {
            string a;
            var userExist = _authService.UserExist(userAndCompanyRegisterDto.UserForRegister.Email);
            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }
            var companyExists = _authService.CompanyExist(userAndCompanyRegisterDto.company);
            if (!companyExists.Success)
            {
                return BadRequest(userExist.Message);
            }

            var registerResult = _authService.Register(userAndCompanyRegisterDto.UserForRegister, userAndCompanyRegisterDto.UserForRegister.Password, userAndCompanyRegisterDto.company);
            var result = _authService.CreateAccessToken(registerResult.Data, registerResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(registerResult.Message);
        }

        [HttpPost("registerSecondAccount")]
        public IActionResult RegisterSecondAccount(UserForRegisterToSecondAccountDto userForRegister)
        {
            string a;
            var userExist = _authService.UserExist(userForRegister.Email);
            if (!userExist.Success)
            {
                return BadRequest(userExist.Message);
            }
            var registerResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password,userForRegister.CompanyId);


            var result = _authService.CreateAccessToken(registerResult.Data, 0);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(registerResult.Message);
        }
        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userToLogin = _authService.Login(userForLogin);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }
            if (userToLogin.Data.IsActive)
            {
                var userCompany = _authService.GetCompany(userToLogin.Data.Id).Data;
                var result = _authService.CreateAccessToken(userToLogin.Data, userCompany.CompanyId);
                if (result.Success)
                {
                    return Ok(result.Data);
                }
                return BadRequest(result.Message);
            }

            return BadRequest("Kullanıcı Pasif Durumda Aktif Etmek için yöneticinize danışın.");

        }

        [HttpGet("confirmuser")]
        public IActionResult ConfirmUser(string value)
        {
            var user=_authService.GetByMailCnfirmValllle(value).Data;
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
            var result=_authService.Update(user);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest();
        }

        [HttpGet("sendconfirmemail")]
        public IActionResult SendConfirmEmail(int id)
        {
            var user = _authService.GetById(id).Data;
            var result=_authService.SendConfirmEmail(user);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }
    }
}
