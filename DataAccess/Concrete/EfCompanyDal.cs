using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfCompanyDal : EfEntityFrameworkBase<Company, ContextDb>, ICompanyDal
    {
        public UserCompany GetCompany(int userId)
        {
            using (var contex = new ContextDb())
            {
                var result = contex.UserCompanies.FirstOrDefault(s => s.UserId == userId);
                return result;
            }
        }

        public void UserCompanyAdd(int userId, int companyId)
        {
            using (var context = new ContextDb())
            {
                UserCompany userCompany = new UserCompany
                {
                    UserId = userId,
                    CompanyId = companyId,
                    AddedAt = DateTime.Now,
                    IsActive = true,
                };
                context.UserCompanies.Add(userCompany);
                context.SaveChanges();
            }
        }
    }
}
