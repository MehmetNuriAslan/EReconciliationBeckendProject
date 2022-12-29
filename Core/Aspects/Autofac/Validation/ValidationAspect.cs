using Business.CrossCuttingConcern.Validation;
using Castle.DynamicProxy;
using Core.Utilities.Interceptor;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Aspects.Autofac.Validation
{
    public class ValidationAspect:MethodInterception
    {
        public Type _validatorType;
        public ValidationAspect(Type validatorType)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
            {
                throw new System.Exception("Hatalı Tip");
            }
            _validatorType = validatorType;
        }
        protected override void OnBefore(IInvocation invocation)
        {
            var validator=(IValidator)Activator.CreateInstance(_validatorType);
            var entitytype = _validatorType.BaseType.GetGenericArguments()[0];
            var entities = invocation.Arguments.Where(t=>t.GetType()==entitytype);
            foreach ( var entity in entities)
            {
                ValidationTool.Validate(validator,entity);
            }
        }
    }
}
