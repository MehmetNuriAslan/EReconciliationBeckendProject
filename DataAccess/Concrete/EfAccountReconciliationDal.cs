using Core.Entities.Concrete;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Dtos;

namespace DataAccess.Concrete
{
    public class EfAccountReconciliationDal : EfEntityFrameworkBase<AccountReconciliation, ContextDb>, IAccountReconciliationDal
    {
        public List<AccountReconciliationDto> GetAllDto(int companyId)
        {
            using (var contex = new ContextDb())
            {
                var result = from reconciliation in contex.AccountReconciliations.Where(s=>s.CompanyId==companyId)
                             join company in contex.Companies on reconciliation.CompanyId equals company.Id
                             join account in contex.CurrencyAccounts on reconciliation.CurencyAccountId equals account.Id
                             join currency in contex.Currencies on reconciliation.CurencyId equals currency.Id
                             select new AccountReconciliationDto
                             {
                                 CompanyId = companyId,
                                 CurencyAccountId = account.Id,
                                 AccountIdentityNumber = account.IdentityNumber,
                                 AccountName = account.Name,
                                 AccountTaxDepartment = account.TaxDepartment,
                                 AccountTaxIdNumber = account.TaxIdNumber,
                                 CompanyIdentityNumber = company.IdentityNumber,
                                 CompanyName = company.Name,
                                 CompanyTaxDepartment = company.TaxDepartment,
                                 CompanyTaxIdNumber = company.TaxIdNumber,
                                 CurencyCredit = reconciliation.CurencyCredit,
                                 CurencyDebit = reconciliation.CurencyDebit,
                                 CurencyId = reconciliation.CurencyId,
                                 EmailReadDate = reconciliation.EmailReadDate,
                                 EndingDate = reconciliation.EndingDate,
                                 Guid = reconciliation.Guid,
                                 Id = reconciliation.Id,
                                 IsEmailRead = reconciliation.IsEmailRead,
                                 IsResultSucceed = reconciliation.IsResultSucceed,
                                 IsSendEmail = reconciliation.IsSendEmail,
                                 ResultDate = reconciliation.ResultDate,
                                 ResultNote = reconciliation.ResultNote,
                                 SendEmailDate = reconciliation.SendEmailDate,
                                 StartingDate = reconciliation.StartingDate,
                                 CurencyCode=currency.Code,
                                 AccountEmail=account.Email
                             };
                return result.ToList();
            }
        }
    }
}
