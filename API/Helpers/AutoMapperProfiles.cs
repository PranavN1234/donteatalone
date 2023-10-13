using API.DTOs;
using API.Entities;
using AutoMapper;
using API.Extensions;

namespace API.Helpers{
    public class AutoMapperProfiles: Profile{
        public AutoMapperProfiles()
        {
            CreateMap<Appuser, MemberDTO>().
            ForMember(dest=>dest.PhotoUrl, opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.isMain).Url))
            .ForMember(dest=>dest.Age, opt=>opt.MapFrom(src=>src.DateofBirth.CalculateAge())); 
            CreateMap<Photo, PhotoDTO>();
        }
    }
}