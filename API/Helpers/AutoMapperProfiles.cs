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

            CreateMap<MemberUpdateDTO, Appuser>();
            CreateMap<RegisterDTO, Appuser>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));

            CreateMap<Messages, MessageDTO>().ForMember(d=>d.SenderPhotoUrl, o=>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.isMain).Url)).ForMember(d=>d.ReceipientPhotoUrl, o=>o.MapFrom(s=>s.Receipient.Photos.FirstOrDefault(x=>x.isMain).Url));


        }
    }
}