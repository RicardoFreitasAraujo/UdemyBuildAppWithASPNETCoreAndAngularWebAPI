using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.Helpers;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            this.CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoURL, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBith.CalculateAge());
                });
            this.CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoURL, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBith.CalculateAge());
                });
            this.CreateMap<Photo, PhotosForDetailedDto>();
            this.CreateMap<UserForUpdateDto, User>();
            this.CreateMap<Photo, PhotoReturnDto>();
            this.CreateMap<Photo, PhotoForCreationDto>().ReverseMap();
            this.CreateMap<User, UserForRegisterDto>().ReverseMap();
        }
    }
}
