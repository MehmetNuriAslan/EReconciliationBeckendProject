using Business.Abstract;
using Business.BusinessAspect;
using Business.Constants;
using Core.Aspects.Caching;
using Core.Aspects.Performance;
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
    public class MailTemplateManager : IMailTemplateService
    {
        private readonly IMailTemplateDal _mailTemplateDal;
        public MailTemplateManager(IMailTemplateDal mailTemplateDal)
        {
            _mailTemplateDal = mailTemplateDal;
        }


        [PerformanceAspect(3)]
        [SecuredOperation("MailTemplate.Add,Admin")]
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Add(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Add(mailTemplate);
            return new SuccessResult(Messages.MailTemplateAdded);
        }


        [PerformanceAspect(3)]
        [SecuredOperation("MailTemplate.Delete,Admin")]
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Delete(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Delete(mailTemplate);
            return new SuccessResult(Messages.MailTemplateDeleted);
        }


        [PerformanceAspect(3)]
        [CacheAspect(60)]
        public IDataResult<MailTemplate> Get(int id)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(s => s.Id == id));
        }


        [PerformanceAspect(3)]
        [SecuredOperation("MailTemplate.GetList,Admin")]
        [CacheAspect(60)]
        public IDataResult<List<MailTemplate>> GetAll(int companyId)
        {
            return new SuccessDataResult<List<MailTemplate>>(_mailTemplateDal.Getlist(s => s.CompanyId == companyId));
        }

        public IDataResult<MailTemplate> GetByCompanyId(int companyId)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(s => s.CompanyId == companyId));
        }

        [CacheAspect(60)]
        public IDataResult<MailTemplate> GetByTemplateName(string name, int companyId)
        {
            return new SuccessDataResult<MailTemplate>(_mailTemplateDal.Get(s => s.CompanyId == companyId && s.Type == name));
        }

        [PerformanceAspect(3)]
        [SecuredOperation("MailTemplate.Update,Admin")]
        [CacheRemoveAspect("IMailTemplateService.Get")]
        public IResult Update(MailTemplate mailTemplate)
        {
            _mailTemplateDal.Update(mailTemplate);
            return new SuccessResult(Messages.MailTemplateUpdated);
        }
    }
}
