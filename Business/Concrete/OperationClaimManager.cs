using Business.Abstract;
using Business.BusinessAspect;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OperationClaimManager:IOperationClaimService
    {
        private readonly IOperationClaimDal _operationClaimDal;
        public OperationClaimManager(IOperationClaimDal operationClaimDal)
        {
            _operationClaimDal = operationClaimDal;
        }

        //[SecuredOperation("Admin")]
        public IResult Add(OperationClaim operationClaim)
        {
            _operationClaimDal.Add(operationClaim);
            return new SuccessResult(Messages.AddedOperationClaim);
        }

        [SecuredOperation("Admin")]
        public IResult Delete(OperationClaim operationClaim)
        {
            _operationClaimDal.Delete(operationClaim);
            return new SuccessResult(Messages.DeletedOperationClaim);
        }

        [SecuredOperation("Admin")]
        public IDataResult<OperationClaim> GetById(int id)
        {
            return new SuccessDataResult<OperationClaim>(_operationClaimDal.Get(s => s.Id == id));
        }

        //[SecuredOperation("Admin,OperationClaim.GetList")]
        public IDataResult<List<OperationClaim>> GetList()
        {
            return new SuccessDataResult<List<OperationClaim>>(_operationClaimDal.Getlist());
        }

        [SecuredOperation("Admin")]
        public IResult Update(OperationClaim operationClaim)
        {
            _operationClaimDal.Update(operationClaim);
            return new SuccessResult(Messages.UpdatedOperationClaim);
        }

        public IDataResult<OperationClaim> Get(Expression<Func<OperationClaim, bool>> filter)
        {
            return new SuccessDataResult<OperationClaim>(_operationClaimDal.Get(filter));
        }
    }
}
