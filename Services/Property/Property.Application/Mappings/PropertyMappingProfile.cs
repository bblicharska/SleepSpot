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
            // Property -> CreatePropertyDto
            CreateMap<Property, CreatePropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            // CreatePropertyDto -> Property
            CreateMap<CreatePropertyDto, Property>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(url => new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url
                    }).ToList() : new List<PropertyImage>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<CreateRoomDto>()));

            // Property -> PropertyDto
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            // Property -> UpdatePropertyDto
            CreateMap<Property, UpdatePropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<Room>()));

            // UpdatePropertyDto -> Property
            CreateMap<UpdatePropertyDto, Property>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null ? src.Images.Select(url => new PropertyImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = url
                    }).ToList() : new List<PropertyImage>()))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src =>
                    src.Rooms ?? new List<UpdateRoomDto>()));

            // Room <-> DTOs
            CreateMap<Room, RoomDto>();
            CreateMap<CreateRoomDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
            CreateMap<UpdateRoomDto, Room>();
            CreateMap<Room, CreateRoomDto>();
            CreateMap<Room, UpdateRoomDto>();
        }
    }
}
