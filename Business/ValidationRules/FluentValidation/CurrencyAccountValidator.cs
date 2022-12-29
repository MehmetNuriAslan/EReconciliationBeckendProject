using Entities.Concrete;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ValidationRules.FluentValidation
{
    internal class CurrencyAccountValidator : AbstractValidator<CurrencyAccount>
    {
        public CurrencyAccountValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Firma Adı Boş Olamaz.");
            RuleFor(p => p.Name).MinimumLength(4).WithMessage("Firma Adı en az 4 karakter olmalıdır.");
            RuleFor(p => p.Address).NotEmpty().WithMessage("Firma Adresi Adı Boş Olamaz.");
            RuleFor(p => p.Address).MinimumLength(4).WithMessage("Firma Adresi Adı en az 4 karakter olmalıdır.");
        }
    }
}
