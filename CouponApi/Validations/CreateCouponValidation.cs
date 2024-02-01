using CouponApi.Models.DTO;
using FluentValidation;

namespace CouponApi.Validations
{
    public class CreateCouponValidation : AbstractValidator<CouponCreateDTO>
    {
        public CreateCouponValidation() {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
