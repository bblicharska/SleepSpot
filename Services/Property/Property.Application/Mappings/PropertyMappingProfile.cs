using AutoMapper;
using PropertyService.Application.Dto;
using PropertyService.Domain.Models;
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
               src.Images.Select(i => i.ImageUrl).ToList()  // Mapping to a list of image URLs
           ));

            // Mapping from CreatePropertyDto -> Property
            CreateMap<CreatePropertyDto, Property>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images.Select(url => new PropertyImage { Id = Guid.NewGuid(), ImageUrl = url }).ToList()
                ));

            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images.Select(i => i.ImageUrl).ToList()  // Mapping ImageUrl (or file name, etc.)
                ));

            CreateMap<Property, UpdatePropertyDto>()
           .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
               src.Images.Select(i => i.ImageUrl).ToList()  // Mapping to a list of image URLs
           ));

            CreateMap<UpdatePropertyDto, Property>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                src.Images.Select(url => new PropertyImage { Id = Guid.NewGuid(), ImageUrl = url }).ToList()
            ));

            CreateMap<Property, DeletePropertyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
