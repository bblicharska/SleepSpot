using AutoMapper;
using PropertyService.Application.Dto;
using PropertyService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PropertyService.Application.Mappings
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Property, CreatePropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images ?? new List<PropertyImage>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            CreateMap<UploadPropertyImageDto, PropertyImage>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
        .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
        .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.File.FileName))
        .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
        .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
        .ForMember(dest => dest.Property, opt => opt.Ignore());

            CreateMap<CreatePropertyDto, Property>()
    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
    .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms));

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images ?? new List<PropertyImage>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            CreateMap<Property, UpdatePropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images ?? new List<PropertyImage>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            CreateMap<UpdatePropertyDto, Property>()
     .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
         src.Images ?? new List<ImageUpdateDto>()));

            CreateMap<Room, RoomDto>();
            CreateMap<CreateRoomDto, Room>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));
            CreateMap<UploadPropertyImageDto, PropertyImage>();
            CreateMap<UpdateRoomDto, Room>();
            CreateMap<Room, CreateRoomDto>();
            CreateMap<Room, UpdateRoomDto>();

            CreateMap<PropertyImage, PropertyImageDto>();
            CreateMap<PropertyImageDto, PropertyImage>();

            CreateMap<ImageUpdateDto, PropertyImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.OriginalFileName, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyId, opt => opt.Ignore());
        }
    }
}
