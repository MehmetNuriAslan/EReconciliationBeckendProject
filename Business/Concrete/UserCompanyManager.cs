using Business.Abstract;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserCompanyManager: IUserCompanyService
    {
        private readonly IUserCompanyDal userCompanyDal;
        public UserCompanyManager(IUserCompanyDal userCompanyDal)
        {
            this.userCompanyDal = userCompanyDal;   
        }
    }
}
