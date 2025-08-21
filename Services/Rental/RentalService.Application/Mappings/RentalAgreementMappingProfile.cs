using AutoMapper;
using RentalService.Application.Dto;
using RentalService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalService.Application.Mappings
{
    public class RentalAgreementMappingProfile : Profile
    {
        public RentalAgreementMappingProfile()
        {
            CreateMap<RentalAgreement, RentalAgreementDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateRentalAgreementDto, RentalAgreement>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateRentalAgreementDto, RentalAgreement>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseStatus(src.Status)));

        }

        private static RentalAgreementStatus ParseStatus(string status)
        {
            return Enum.TryParse<RentalAgreementStatus>(status, out var parsed)
                ? parsed
                : RentalAgreementStatus.Active;
        }

    }
}
