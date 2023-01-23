using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfUserOperationClaimDal : EfEntityFrameworkBase<UserOperationClaim, ContextDb>, IUserOperationClaimDal
    {
        public List<UserOperationClaimDto> GetListDto(int userId, int companyId)
        {
            using (var contex = new ContextDb())
            {
                var result = from useroperationclaim in contex.UserOperationClaims.Where(s => s.UserId == userId && s.CompanyId == companyId)
                             join operationClaim in contex.OperationClaims on useroperationclaim.OperationClaimId equals operationClaim.Id
                             select new UserOperationClaimDto
                             {
                                 UserId = userId,
                                 CompanyId = companyId,
                                 Id= operationClaim.Id,
                                 OperationClaimId=operationClaim.Id,
                                 OperationDescription=operationClaim.Description,
                                 OperationName=operationClaim.Name
                             };
                return result.ToList();
            }
        }
    }
}
