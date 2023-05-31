using FluentValidation;
using TunahanAliOzturk.CouponAPI.Models.Dto;

namespace TunahanAliOzturk.CouponAPI.Validation
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDto>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
