using Business.Abstract;
using Business.BusinessAspect;
using Business.Constants;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AccountReconciliationManager : IAccountReconciliationService
    {
        private readonly IAccountReconciliationDal _accountReconciliationDal;
        private readonly ICurrencyAccountService _currencyAccountService;
        private readonly IMailService _mailService;
        private readonly IMailTemplateService _mailTemplateService;
        private readonly IMailParameterService _mailParameterService;

        public AccountReconciliationManager(IAccountReconciliationDal accountReconciliationDal, ICurrencyAccountService currencyAccountService, IMailService mailService, IMailTemplateService mailTemplateService, IMailParameterService mailParameterService)
        {
            _accountReconciliationDal = accountReconciliationDal;
            _currencyAccountService = currencyAccountService;
            _mailService = mailService;
            _mailTemplateService = mailTemplateService;
            _mailParameterService = mailParameterService;
        }


        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.Add,Admin")]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Add(AccountReconciliation accountReconciliation)
        {
            string guid = Guid.NewGuid().ToString();
            accountReconciliation.Guid = guid;
            _accountReconciliationDal.Add(accountReconciliation);
            return new SuccessResult(Messages.AddedAccounReconciliation);
        }

        [PerformanceAspect(3)]
       [SecuredOperation("AccountReconciliation.Add,Admin")]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        [TransactionScopeAspect]
        public IResult AddToExcel(string filePath, int companyId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string code = reader.GetString(0);
                        string guid = Guid.NewGuid().ToString();
                        if (code != "Cari Kodu" && code != null)
                        {
                            DateTime startingDate = reader.GetDateTime(1);
                            DateTime endingDate = reader.GetDateTime(2);
                            double currencyId = reader.GetDouble(3);
                            double debit = reader.GetDouble(4);
                            double credit = reader.GetDouble(5);

                            int id = _currencyAccountService.GetByCode(code, companyId).Data.Id;
                            AccountReconciliation accountReconciliation = new AccountReconciliation()
                            {
                                CompanyId = companyId,
                                CurencyAccountId = id,
                                CurencyCredit = Convert.ToDecimal(credit),
                                CurencyDebit = Convert.ToDecimal(debit),
                                CurencyId = Convert.ToInt32(currencyId),
                                StartingDate = startingDate,
                                EndingDate = endingDate,
                                Guid=guid
                            };
                            _accountReconciliationDal.Add(accountReconciliation);
                        }
                    }
                }
            }
            File.Delete(filePath);
            return new SuccessResult(Messages.AddedAccounReconciliation);
        }

        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.Delete,Admin")]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Delete(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Delete(accountReconciliation);
            return new SuccessResult(Messages.DeletedAccounReconciliation);
        }

        [PerformanceAspect(3)]
        public IDataResult<AccountReconciliation> GetByCode(string code)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(s => s.Guid == code));
        }

        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.Get,Admin")]
        [CacheAspect(60)]
        public IDataResult<AccountReconciliation> GetById(int id)
        {
            return new SuccessDataResult<AccountReconciliation>(_accountReconciliationDal.Get(s => s.Id == id));
        }


        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<AccountReconciliation>> GetList(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliation>>(_accountReconciliationDal.Getlist(s => s.CompanyId == companyId));
        }

        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<Entities.Dtos.AccountReconciliationDto>> GetListDto(int companyId)
        {
            return new SuccessDataResult<List<AccountReconciliationDto>>(_accountReconciliationDal.GetAllDto(companyId));
        }

        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.SendMail,Admin")]
        public IResult SendReconciliationMail(AccountReconciliationDto accountReconciliationDto)
        {
            string subject = "Mutabakat Maili";
            string body = $"Şirket Adımız:{accountReconciliationDto.CompanyName}<br/>" +
                $"Şirket Vergi Dairesi: {accountReconciliationDto.CompanyTaxDepartment}<br/>" +
                $"Şirket Vergi Numarası: {accountReconciliationDto.CompanyTaxIdNumber} - {accountReconciliationDto.CompanyIdentityNumber}<br/><hr>" +
                $"Sizin Şirket: {accountReconciliationDto.AccountName} <br/>" +
                $"Sizin Şirket Vergi Dairesi: {accountReconciliationDto.AccountTaxDepartment}<br/>" +
                $"Sizin Şirket Vergi Numarası: {accountReconciliationDto.AccountTaxIdNumber} - {accountReconciliationDto.AccountIdentityNumber}<br/><hr>" +
                $"Borç: {accountReconciliationDto.CurencyDebit} {accountReconciliationDto.CurencyCode}<br/>" +
                $"Alacak: {accountReconciliationDto.CurencyCredit} {accountReconciliationDto.CurencyCode}<br/>";
            string link = "https://localhost:7297/api/AccountReconciliation/getByCode?code=" + accountReconciliationDto.Guid;
            string linkDescription = "Mutabakatı Cevaplamak için Tıklayınız.";

            var mailTemplate = _mailTemplateService.GetByTemplateName("Kayit", 2);
            string templatebody = mailTemplate.Data.Value.Replace("{{linkDescription}}", linkDescription).Replace("{{link}}", link).Replace("{{title}}", subject).Replace("{{message}}", body);


            var mailparameter = _mailParameterService.Get(2);
            Entities.Dtos.SendMailDto sendMailDto = new Entities.Dtos.SendMailDto
            {
                mailParameter = mailparameter.Data,
                email = accountReconciliationDto.AccountEmail,
                subject = subject,
                body = templatebody
            };

            _mailService.SendMail(sendMailDto);
            return new SuccessResult(Messages.SendMailSuccessFul);
        }

        [PerformanceAspect(3)]
        [SecuredOperation("AccountReconciliation.Update,Admin")]
        [CacheRemoveAspect("IAccountReconciliationService.Get")]
        public IResult Update(AccountReconciliation accountReconciliation)
        {
            _accountReconciliationDal.Update(accountReconciliation);
            return new SuccessResult(Messages.UpdatedAccounReconciliation);
        }
    }
}
