using AutoMapper;
using TunahanAliOzturk.CouponAPI.Models;
using TunahanAliOzturk.CouponAPI.Models.Dto;

namespace TunahanAliOzturk.CouponAPI.Mapping
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreateDto>().ReverseMap();
            CreateMap<Coupon, CouponDto>().ReverseMap();
        }
    }
}
