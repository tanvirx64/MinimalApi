using CouponApi.Models.DTO;
using FluentValidation;

namespace CouponApi.Validations
{
    public class UpdateCouponValidation : AbstractValidator<CouponUpdateDTO>
    {
        public UpdateCouponValidation() {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
