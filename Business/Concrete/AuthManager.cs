using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly ICompanyService _companyService;
        private readonly IMailService _mailService;
        private readonly IMailParameterService _mailParameterService;
        private readonly IMailTemplateService _mailTemplateService;
        public AuthManager(IUserService userService, ITokenHelper tokenHelper, ICompanyService companyService, IMailService mailService, IMailParameterService mailParameterService, IMailTemplateService mailTemplateService)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _companyService = companyService;
            _mailService = mailService; 
            _mailParameterService = mailParameterService;
            _mailTemplateService = mailTemplateService; 
        }

        public IResult CompanyExist(Company company)
        {
            var result = _companyService.CompanyExist(company);
            if (!result.Success)
            {
                return new ErrorResult(Messages.CompanyAlreadyExist);
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user,int companyId)
        {
            var claims = _userService.GetClaims(user, companyId);
            var accessToken = _tokenHelper.CreateToken(user, claims, companyId);
            return new SuccessDataResult<AccessToken>(accessToken); 
        }

        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userService.GetById(id));
        }

        public IDataResult<User> GetByMailCnfirmValllle(string value)
        {
            return new SuccessDataResult<User>(_userService.GetByMailConfirmValue(value));
        }

        public IDataResult<User> Login(UserForLogin userForLogin)
        {
            var userToCheck=_userService.GetByMail(userForLogin.Email); 
            if (userToCheck == null) 
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userForLogin.Password, userToCheck.PasswordHash,userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck,Messages.SuccessfullLogin);    
        }

        public IDataResult<UserCompanyDto> Register(UserForRegister userForRegister, string password,Company company)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password,out passwordHash,out passwordSalt);
            var user = new User()
            {
                Email = userForRegister.Email,
                AddedAt = DateTime.Now,
                MailConfirm = false,
                MailConfirmDate = DateTime.Now,
                MailConfirmValue = Guid.NewGuid().ToString(),
                PasswordHash=passwordHash,
                PasswordSalt=passwordSalt,
                Name=userForRegister.Name                
            };
            _userService.Add(user);
            _companyService.Add(company);
            _companyService.UserCompanyAdd(user.Id,company.Id);
            UserCompanyDto userCompanyDto = new UserCompanyDto
            {
                Id = user.Id,
                AddedAt= user.AddedAt,
                Email=user.Email,
                CompanyId=company.Id,
                IsActive=user.IsActive,
                MailConfirm=user.MailConfirm,
                MailConfirmDate=user.MailConfirmDate,
                MailConfirmValue=user.MailConfirmValue,
                Name=user.Name,
                PasswordHash=user.PasswordHash,
                PasswordSalt= user.PasswordSalt
            };

            SendconfirmEmail(user);
            return new SuccessDataResult<UserCompanyDto>(userCompanyDto, Messages.UserRegistered);
        }

        void SendconfirmEmail(User user)
        {
            string subject = "Kullanıcı Onay Maili";
            string body = "Kullanıcınız sisteme kayıt oldu. Kaydınızı tamamlamak için aşağıdaki linke tıklamanız gerekmektedir.";
            string link = "https://localhost:7297/api/auth/confirmuser?value=" + user.MailConfirmValue;
            string linkDescription = "Kaydı Onaylamak için Tıklayınız.";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kayıt", 2);
            string templatebody = mailTemplate.Data.Value.Replace("{{linkDescription}}", linkDescription).Replace("{{link}}", link).Replace("{{title}}", subject).Replace("{{message}}", body);


            var mailparameter = _mailParameterService.Get(2);
            SendMailDto sendMailDto = new SendMailDto
            {
                mailParameter = mailparameter.Data,
                email = user.Email,
                subject = "Kullanıcı Onay Maili",
                body = templatebody
            };

            _mailService.SendMail(sendMailDto);
        }
        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password)
        {
            throw new NotImplementedException();
        }

        public IDataResult<User> RegisterSecondAccount(UserForRegister userForRegister, string password, Company company)
        {
            throw new NotImplementedException();
        }

        public IResult Update(User user)
        {
            _userService.Update(user);
            return new SuccessResult(Messages.UserMailConfirmSuccessful);
        }

        public IResult UserExist(string email)
        {
            if (_userService.GetByMail(email)!=null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }

        public IResult SendConfirmEmail(User user)
        {
            SendconfirmEmail(user);
            return new SuccessResult(Messages.MailConfirmSendSuccessful);
        }
    }
}
