﻿using Core.DataAccess.EntityFramework;
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
    public class EfAccountReconciliationDetailDal : EfEntityFrameworkBase<AccountReconciliationDetail, ContextDb>, IAccountReconciliationDetailDal
    {
    }
}
