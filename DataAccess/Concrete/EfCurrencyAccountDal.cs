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
    public class EfCurrencyAccountDal : EfEntityFrameworkBase<CurrencyAccount, ContextDb>, ICurrencyAccountDal
    {
        public bool CheckCurrencyAccountReconciliation(int currencyAccountId)
        {
            using (var contex=new ContextDb())
            {
                var reconciliations = contex.AccountReconciliations.Where(p => p.CurencyAccountId == currencyAccountId).ToList();
                if (reconciliations.Count>0)
                {
                    return false;
                }
                var babsreconciliations = contex.BaBsReconciliations.Where(p => p.CurencyAccountId == currencyAccountId).ToList();
                if (reconciliations.Count > 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
