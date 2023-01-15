using Business.Abstract;
using Business.BusinessAspect;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class TermsAndConditionManager : ITermsAndConditionService
    {
        private readonly ITermsAndConditionDal _termsAndConditionDal;

        public TermsAndConditionManager(ITermsAndConditionDal termsAndConditionDal)
        {
            _termsAndConditionDal = termsAndConditionDal;
        }

        public IDataResult<TermsAndCondition> Get()
        {
            return new SuccessDataResult<TermsAndCondition>(_termsAndConditionDal.Getlist().FirstOrDefault());
        }

        [SecuredOperation("Admin")]
        public IResult Update(TermsAndCondition termsAndCondition)
        {
            var result = _termsAndConditionDal.Getlist().FirstOrDefault();
            if (result != null)
            {
                result.Description = termsAndCondition.Description;
            }
            else
            {
                _termsAndConditionDal.Add(termsAndCondition);
            }

            return new SuccessResult(Messages.UpdatedTermsAndCondition);
        }
    }
}
