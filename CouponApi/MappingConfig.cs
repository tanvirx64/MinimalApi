using AutoMapper;
using CouponApi.Models;
using CouponApi.Models.DTO;

namespace CouponApi
{
    public class MappingConfig : Profile
    {
        public MappingConfig() {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
