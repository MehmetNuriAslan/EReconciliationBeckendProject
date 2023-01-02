using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfBaBsReconciliationDal : EfEntityFrameworkBase<BaBsReconciliation, ContextDb>, IBaBsReconciliationDal
    {
        public List<BaBsReconciliationDto> GetAllDto(int companyId)
        {
            using (var contex = new ContextDb())
            {
                var result = from reconciliation in contex.BaBsReconciliations.Where(s => s.CompanyId == companyId)
                             join company in contex.Companies on reconciliation.CompanyId equals company.Id
                             join account in contex.CurrencyAccounts on reconciliation.CurencyAccountId equals account.Id 
                             select new BaBsReconciliationDto
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
                                 EmailReadDate = reconciliation.EmailReadDate,
                                 Guid = reconciliation.Guid,
                                 Id = reconciliation.Id,
                                 IsEmailRead = reconciliation.IsEmailRead,
                                 IsResultSucceed = reconciliation.IsResultSucceed,
                                 IsSendEmail = reconciliation.IsSendEmail,
                                 ResultDate = reconciliation.ResultDate,
                                 ResultNote = reconciliation.ResultNote,
                                 SendEmailDate = reconciliation.SendEmailDate,
                                 CurencyCode = "TL",
                                 AccountEmail = account.Email,
                                 Mounth=reconciliation.Mounth,
                                 Year= reconciliation.Year,
                                 Quantity= reconciliation.Quantity,
                                 Total= reconciliation.Total,
                                 Type = reconciliation.Type
                             };
                return result.ToList();
            }
        }
    }
}
