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

namespace DataAccess.Concrete
{
    public class EfAccountReconciliationDal : EfEntityFrameworkBase<AccountReconciliation, ContextDb>, IAccountReconciliationDal
    {
    }
}
