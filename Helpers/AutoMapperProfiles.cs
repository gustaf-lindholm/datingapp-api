using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.models;

namespace DatingApp.API.Helpers
{
    // profiles for the source and destination of the mappings
    public class AutoMapperProfiles : Profile
    {
        // specify source and destination
        public AutoMapperProfiles()
        {

            // for a specific user
            // get the photourl-property and where the information for that property comes from
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => 
                src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
        }

    }
}