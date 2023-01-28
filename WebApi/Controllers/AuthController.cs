using Business.Abstract;
using Core.Utilities.Hashing;
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
        private readonly IForgotPasswordService _forgotPasswordService;
        public AuthController(IAuthService authService, IForgotPasswordService forgotPasswordService)
        {
            _authService = authService;
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegisterDto)
        {
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
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Kullanıcı Pasif Durumda Aktif Etmek için yöneticinize danışın.");

        }

        [HttpGet("confirmuser")]
        public IActionResult ConfirmUser(string value)
        {
            var user=_authService.GetByMailCnfirmValllle(value).Data;
            if (user.MailConfirm)
            {
                return BadRequest("Kullanıcı Zaten Onaylı. Aynı maili tekrar onaylayamassınız.");
            }
            user.MailConfirm = true;
            user.MailConfirmDate = DateTime.Now;
            var result=_authService.Update(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("sendconfirmemail")]
        public IActionResult SendConfirmEmail(string email)
        {
            var user = _authService.GetByEmail(email).Data;
            if (user == null)
            {
                return BadRequest("Kullanıcı Bulunamadı");
            }
            if (user.MailConfirm)
            {
                return BadRequest("Kullanıcının Maili Onaylı");
            }

            var result=_authService.SendConfirmEmailAgain(user);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("forgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            var user = _authService.GetByEmail(email).Data;
            if (user == null)
            {
                return BadRequest("Kullanıcı Bulunamadı");
            }
            var lists=_forgotPasswordService.GetListByUserId(user.Id).Data;
            foreach (var item in lists)
            {
                item.IsActive = false;
                _forgotPasswordService.Update(item);
            }
            var forgotPassword = _forgotPasswordService.CreateForgotPassword(user).Data;

            var result = _authService.SendForgotPasswordEmail(user, forgotPassword.Value);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }


        [HttpGet("forgotPasswordLinkCheck")]
        public IActionResult ForgotPasswordLinkCheck(string value)
        {


            var result = _forgotPasswordService.GetForgotPassword(value);
            if (result==null)
            {
                return BadRequest("Tıkladığınız link geçersiz!");
                
            }
            if (result.IsActive==true)
            {
                DateTime date1=DateTime.Now.AddHours(-1);
                DateTime date2=DateTime.Now;
                if (result.SendDate>=date1&& result.SendDate <= date2)
                {
                    return Ok(true);
                }
                 else
            {
                return BadRequest("Tıkladığınız link geçersiz!");
            }
            }
            else
            {
                return BadRequest("Tıkladığınız link geçersiz!");
            }

            
        }

        [HttpPost("changePasswordToForgotPassword")]
        public IActionResult ChangePasswordToForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {


            var forgotPasswordResult = _forgotPasswordService.GetForgotPassword(forgotPasswordDto.Value);
            forgotPasswordResult.IsActive = false;
            _forgotPasswordService.Update(forgotPasswordResult);

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(forgotPasswordDto.Password, out passwordHash, out passwordSalt);
            var userResult = _authService.GetById(forgotPasswordResult.UserId).Data;
            userResult.PasswordHash = passwordHash; 
            userResult.PasswordSalt = passwordSalt;
            var result = _authService.Update(userResult);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);


        }

    }
}
