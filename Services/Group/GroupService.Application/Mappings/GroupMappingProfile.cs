using GroupService.Application.Dto;
using GroupService.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Shared.Dto;

namespace GroupService.Application.Mappings
{

    public class GroupMappingProfile : Profile
    {
        public GroupMappingProfile()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<CreateGroupDto, Group>();
            CreateMap<UpdateGroupDto, Group>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());

            CreateMap<GroupMember, GroupMemberDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
            CreateMap<GroupMemberDto, GroupMember>()
    .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<GroupRole>(src.Role, true)))
    .ForMember(dest => dest.Group, opt => opt.Ignore());

            CreateMap<AddGroupMemberDto, GroupMember>();
            CreateMap<UpdateGroupMemberDto, GroupMember>()
                .ForMember(dest => dest.GroupId, opt => opt.Ignore()) 
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<GroupListing, GroupListingDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateGroupListingDto, GroupListing>();
            CreateMap<UpdateGroupListingDto, GroupListing>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) 
                .ForMember(dest => dest.Group, opt => opt.Ignore());

            CreateMap<RoomApplication, RoomApplicationDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<RoomApplicationDto, RoomApplication>()
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ApplicationStatus>(src.Status)));
            CreateMap<CreateRoomApplicationDto, RoomApplication>();
            CreateMap<UpdateRoomApplicationDto, RoomApplication>()
                .ForMember(dest => dest.ApplicantUserId, opt => opt.Ignore()) 
                .ForMember(dest => dest.ListingId, opt => opt.Ignore());
        }
    }
}
