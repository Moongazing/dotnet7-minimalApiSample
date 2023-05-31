using FluentValidation;
using TunahanAliOzturk.CouponAPI.Models.Dto;

namespace TunahanAliOzturk.CouponAPI.Validation
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDto>
    {
        public CouponUpdateValidation()
        {
            RuleFor(model => model.Id).NotEmpty().GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
