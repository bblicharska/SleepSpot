using AutoMapper;
using GroupService.Application.Dto;
using GroupService.Application.Interfaces;
using GroupService.Domain.Contracts;
using GroupService.Domain.Models;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using AutoMapper.Execution;

namespace GroupService.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupUnitOfWork _unitOfWork;
        private readonly IUserClient _userClient;
        private readonly IPropertyClient _propertyClient;
        private readonly IMapper _mapper;

        public GroupService(
            IGroupUnitOfWork unitOfWork,
            IUserClient userClient,
            IPropertyClient propertyClient,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userClient = userClient;
            _propertyClient = propertyClient;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
        {
            var groups = await _unitOfWork.GroupRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<GroupDto>>(groups);

            foreach (var dto in dtos)
            {
                var members = await _unitOfWork.GroupMemberRepository.GetByGroupIdAsync(dto.Id);
                var memberDtos = _mapper.Map<List<GroupMemberDto>>(members);

                foreach (var memberDto in memberDtos)
                {
                    try
                    {
                        memberDto.User = await _userClient.GetUserByIdAsync(memberDto.UserId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Nie udało się pobrać użytkownika {memberDto.UserId}: {ex.Message}");
                    }
                }

                dto.Members = memberDtos;
            }

            return dtos;
        }

        public async Task<GroupDto?> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (group == null) return null;

            var dto = _mapper.Map<GroupDto>(group);
            var members = await _unitOfWork.GroupMemberRepository.GetByGroupIdAsync(group.Id);
            dto.Members = _mapper.Map<List<GroupMemberDto>>(members);

            foreach (var member in dto.Members)
            {
                member.User = await _userClient.GetUserByIdAsync(member.UserId);
            }

            return dto;
        }

        public async Task<Guid> CreateGroupAsync(CreateGroupDto dto)
        {
            var invalidEmails = new List<string>();
            var membersToAdd = new List<UserDto>();

            if (dto.MemberEmails != null)
            {
                foreach (var email in dto.MemberEmails
                                         .Where(e => !string.IsNullOrWhiteSpace(e))
                                         .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var user = await _userClient.GetUserByEmailAsync(email);

                    if (user == null)
                    {
                        invalidEmails.Add(email);
                        continue;
                    }
                    if (user.Id != dto.CreatedByUserId)
                    {
                        membersToAdd.Add(user);
                    }
                }
            }

            if (invalidEmails.Any())
            {
                throw new Shared.Exceptions.ValidationException($"The following emails were not found: {string.Join(", ", invalidEmails)}");
            }

            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GroupRepository.AddAsync(group);

            var creatorMember = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = group.CreatedByUserId,
                Role = GroupRole.Admin,
                JoinedAt = DateTime.UtcNow
            };
            await _unitOfWork.GroupMemberRepository.AddAsync(creatorMember);

            foreach (var user in membersToAdd)
            {
                var member = new GroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    UserId = user.Id,
                    Role = GroupRole.Member,
                    JoinedAt = DateTime.UtcNow
                };
                await _unitOfWork.GroupMemberRepository.AddAsync(member);
            }

            await _unitOfWork.CommitAsync();

            return group.Id;
        }

        public async Task UpdateGroupAsync(Guid groupId, CreateGroupDto dto)
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (group == null) throw new KeyNotFoundException("Group not found");

            group.Name = dto.Name;
            group.Description = dto.Description;

            await _unitOfWork.GroupRepository.UpdateAsync(group);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteGroupAsync(Guid groupId)
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found");
            var listings = await _unitOfWork.GroupListingRepository.GetByGroupIdAsync(groupId);

            foreach (var listing in listings)
            {
                await _unitOfWork.GroupListingRepository.DeleteAsync(listing);
            }

            await _unitOfWork.GroupRepository.DeleteAsync(group);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<GroupDto>> GetGroupsForUserAsync(Guid userId)
        {
            var groups = await _unitOfWork.GroupRepository.GetByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<GroupDto>>(groups);

            foreach (var dto in dtos)
            {
                dto.Members = _mapper.Map<List<GroupMemberDto>>(
                    await _unitOfWork.GroupMemberRepository.GetByGroupIdAsync(dto.Id)
                );

                foreach (var member in dto.Members)
                {
                    var userInfo = await _userClient.GetUserByIdAsync(member.UserId);
                    if (userInfo != null)
                    {
                        member.User = userInfo; 
                    }
                }
            }

            return dtos;
        }

        public async Task<IEnumerable<GroupMemberDto>> GetMembersByGroupIdAsync(Guid groupId)
        {
            var members = await _unitOfWork.GroupMemberRepository.GetByGroupIdAsync(groupId);
            return _mapper.Map<IEnumerable<GroupMemberDto>>(members);
        }

        public async Task AddMemberAsync(GroupMemberDto dto)
        {
            var member = _mapper.Map<GroupMember>(dto);
            member.Id = Guid.NewGuid();
            member.JoinedAt = DateTime.UtcNow;

            await _unitOfWork.GroupMemberRepository.AddAsync(member);
            await _unitOfWork.CommitAsync();
        }
        public async Task AddMemberByEmailAsync(AddMemberByEmailRequest dto)
        {
            if (!dto.GroupId.HasValue) throw new ArgumentException("GroupId is required.", nameof(dto.GroupId));

            var groupId = dto.GroupId.Value;

            var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (group == null) throw new KeyNotFoundException("Group not found");

            var user = await _userClient.GetUserByEmailAsync(dto.Email);
            if (user == null) throw new KeyNotFoundException($"User with email {dto.Email} not found.");

            var members = group.Members ?? (await _unitOfWork.GroupMemberRepository.GetByGroupIdAsync(groupId)).ToList();
            if (members.Any(m => m.UserId == user.Id))
                throw new InvalidOperationException("User is already a member of the group.");

            var member = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                UserId = user.Id,
                Role = dto.Role,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.GroupMemberRepository.AddAsync(member);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveMemberAsync(Guid memberId)
        {
            var member = await _unitOfWork.GroupMemberRepository.GetByIdAsync(memberId);
            if (member == null) throw new KeyNotFoundException("Member not found");

            await _unitOfWork.GroupMemberRepository.RemoveAsync(member);
            await _unitOfWork.CommitAsync();
        }

        public async Task<PagedResult<GroupListingDto>> GetPagedListingsAsync(GroupListingQueryParams queryParams)
        {
            var query = await _unitOfWork.GroupListingRepository.GetFilteredListingsQueryAsync(queryParams);

            var totalCount = await query.CountAsync();

            var listings = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var mappedListings = await MapListingsWithApplicationsAsync(listings);

            return new PagedResult<GroupListingDto>
            {
                Items = mappedListings,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<GroupListingDto?> GetListingByIdAsync(Guid listingId)
        {
            var listing = await _unitOfWork.GroupListingRepository.GetByIdAsync(listingId);
            if (listing == null) return null;

            var dto = _mapper.Map<GroupListingDto>(listing);
            dto.Applications = _mapper.Map<List<RoomApplicationDto>>(await _unitOfWork.RoomApplicationRepository.GetByListingIdAsync(listingId));

            if (dto.PropertyId.HasValue)
            {
                dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
            }
            if (dto.GroupId!=null)
            {
                dto.Group = await GetGroupByIdAsync(dto.GroupId);
            }

            return dto;
        }

        public async Task<IEnumerable<GroupListingDto>> GetListingsByGroupIdAsync(Guid groupId)
        {
            var listings = await _unitOfWork.GroupListingRepository.GetByGroupIdAsync(groupId);
            return await MapListingsWithApplicationsAsync(listings);
        }

        public async Task<Guid> CreateListingAsync(CreateGroupListingDto dto)
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(dto.GroupId);
            if (group == null)
            {
                throw new Shared.Exceptions.ValidationException($"Group with ID {dto.GroupId} does not exist.");
            }

            if (dto.PropertyId.HasValue)
            {
                var propertyGuid = dto.PropertyId.Value;
                var property = await _propertyClient.GetPropertyByIdAsync(propertyGuid);
                if (property == null)
                {
                    throw new Shared.Exceptions.ValidationException($"Property with ID {propertyGuid} does not exist.");
                }
            }

            if (dto.RoomId.HasValue)
            {
                var roomGuid = dto.RoomId.Value;
                var room = await _propertyClient.GetRoomByIdAsync(roomGuid);
                if (room == null)
                {
                    throw new Shared.Exceptions.ValidationException($"Room with ID {roomGuid} does not exist.");
                }
            }
            var listing = _mapper.Map<GroupListing>(dto);
            listing.Id = Guid.NewGuid();
            listing.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GroupListingRepository.AddAsync(listing);
            await _unitOfWork.CommitAsync();

            return listing.Id;
        }

        public async Task UpdateListingAsync(Guid listingId, CreateGroupListingDto dto)
        {
            var listing = await _unitOfWork.GroupListingRepository.GetByIdAsync(listingId);
            if (listing == null) throw new KeyNotFoundException("Listing not found");

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.DesiredRoommatesCount = dto.DesiredRoommatesCount;
            listing.PropertyId = dto.PropertyId;
            listing.PreferredCity = dto.PreferredCity;
            listing.PropertyAlreadyRented = dto.PropertyAlreadyRented;
            listing.MaxBudgetPerPerson = dto.MaxBudgetPerPerson;

            await _unitOfWork.GroupListingRepository.UpdateAsync(listing);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteListingAsync(Guid listingId)
        {
            var listing = await _unitOfWork.GroupListingRepository.GetByIdAsync(listingId);
            if (listing == null) throw new KeyNotFoundException("Listing not found");

            await _unitOfWork.GroupListingRepository.DeleteAsync(listing);
            await _unitOfWork.CommitAsync();
        }
        
        public async Task<IEnumerable<RoomApplicationDto>> GetApplicationsByListingIdAsync(Guid listingId)
        {
            var applications = await _unitOfWork.RoomApplicationRepository.GetByListingIdAsync(listingId);
            var applicationDtos = new List<RoomApplicationDto>();

            foreach (var app in applications)
            {
                var userInfo = await _userClient.GetUserByIdAsync(app.ApplicantUserId);
                var dto = _mapper.Map<RoomApplicationDto>(app);
                dto.Applicant = userInfo;
                applicationDtos.Add(dto);
            }

            return applicationDtos;
        }

        public async Task<RoomApplicationDto?> GetApplicationByIdAsync(Guid applicationId)
        {
            var application = await _unitOfWork.RoomApplicationRepository.GetByIdAsync(applicationId);
            return _mapper.Map<RoomApplicationDto?>(application);
        }

        public async Task<Guid> CreateApplicationAsync(RoomApplicationDto dto)
        {
            var application = _mapper.Map<RoomApplication>(dto);
            application.Id = Guid.NewGuid();
            application.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.RoomApplicationRepository.AddAsync(application);
            await _unitOfWork.CommitAsync();

            return application.Id;
        }

        public async Task UpdateApplicationStatusAsync(Guid applicationId, string status)
        {
            var application = await _unitOfWork.RoomApplicationRepository.GetByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            if (!Enum.TryParse(status, out ApplicationStatus parsedStatus))
                throw new ArgumentException("Invalid status value");

            application.Status = parsedStatus;
            await _unitOfWork.RoomApplicationRepository.UpdateAsync(application);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteApplicationAsync(Guid applicationId)
        {
            var application = await _unitOfWork.RoomApplicationRepository.GetByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            await _unitOfWork.RoomApplicationRepository.DeleteAsync(application);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<RoomApplicationDto>> GetApplicationsByApplicantIdAsync(Guid applicantId)
        {
            var applications = await _unitOfWork.RoomApplicationRepository.GetByApplicantIdAsync(applicantId);
            var dtos = new List<RoomApplicationDto>();

            foreach (var app in applications)
            {
                UserDto userInfo = null;
                try
                {
                    userInfo = await _userClient.GetUserByIdAsync(app.ApplicantUserId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch user {app.ApplicantUserId}: {ex.Message}");
                }

                var dto = _mapper.Map<RoomApplicationDto>(app);
                dto.Applicant = userInfo;
                dtos.Add(dto);
            }

            return dtos;
        }

        private async Task<IEnumerable<GroupListingDto>> MapListingsWithApplicationsAsync(IEnumerable<GroupListing> listings)
        {
            var dtos = _mapper.Map<List<GroupListingDto>>(listings);

            foreach (var dto in dtos)
            {
                dto.Applications = _mapper.Map<List<RoomApplicationDto>>(
                    await _unitOfWork.RoomApplicationRepository.GetByListingIdAsync(dto.Id));

                if (dto.PropertyId.HasValue)
                {
                    dto.Property = await _propertyClient.GetPropertyByIdAsync(dto.PropertyId.Value);
                }
            }

            return dtos;
        }

    }
}
