using GroupService.Application.Dto;
using GroupService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Exceptions;

namespace GroupAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var id = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetGroupById), new { id }, id);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            await _groupService.DeleteGroupAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetGroupsForUser(Guid userId)
        {
            var groups = await _groupService.GetGroupsForUserAsync(userId);
            return Ok(groups);
        }

        [HttpGet("{groupId:guid}/members")]
        public async Task<IActionResult> GetMembersByGroupId(Guid groupId)
        {
            var members = await _groupService.GetMembersByGroupIdAsync(groupId);
            return Ok(members);
        }

        [HttpPost("members/by-email")]
        public async Task<IActionResult> AddMemberByEmail([FromBody] AddMemberByEmailRequest dto)
        {
            await _groupService.AddMemberByEmailAsync(dto);
            return NoContent();
        }

        [HttpDelete("members/{memberId:guid}")]
        public async Task<IActionResult> RemoveMember(Guid memberId)
        {
            await _groupService.RemoveMemberAsync(memberId);
            return NoContent();
        }

        [HttpGet("listings")]
        public async Task<IActionResult> GetAllListings([FromQuery] GroupListingQueryParams queryParams)
        {
            var result = await _groupService.GetPagedListingsAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("listings/{listingId:guid}")]
        public async Task<IActionResult> GetListingById(Guid listingId)
        {
            var listing = await _groupService.GetListingByIdAsync(listingId);
            if (listing == null) return NotFound();
            return Ok(listing);
        }

        [HttpGet("{groupId:guid}/listings")]
        public async Task<IActionResult> GetListingsByGroupId(Guid groupId)
        {
            var listings = await _groupService.GetListingsByGroupIdAsync(groupId);
            return Ok(listings);
        }

        [HttpPost("listings")]
        public async Task<IActionResult> CreateListing([FromBody] CreateGroupListingDto dto)
        {
            var id = await _groupService.CreateListingAsync(dto);
            return CreatedAtAction(nameof(GetListingById), new { listingId = id }, id);
        }

        [HttpDelete("listings/{listingId:guid}")]
        public async Task<IActionResult> DeleteListing(Guid listingId)
        {
            await _groupService.DeleteListingAsync(listingId);
            return NoContent();
        }

        [HttpGet("applications/listing/{listingId:guid}")]
        public async Task<IActionResult> GetApplicationsByListingId(Guid listingId)
        {
            var applications = await _groupService.GetApplicationsByListingIdAsync(listingId);
            return Ok(applications);
        }

        [HttpGet("applications/{applicationId:guid}")]
        public async Task<IActionResult> GetApplicationById(Guid applicationId)
        {
            var app = await _groupService.GetApplicationByIdAsync(applicationId);
            if (app == null) return NotFound();
            return Ok(app);
        }

        [HttpPost("applications")]
        public async Task<IActionResult> CreateApplication([FromBody] RoomApplicationDto dto)
        {
            var id = await _groupService.CreateApplicationAsync(dto);
            return CreatedAtAction(nameof(GetApplicationById), new { applicationId = id }, id);
        }

        [HttpGet("applications/applicant/{applicantId:guid}")]
        public async Task<IActionResult> GetApplicationsByApplicantId(Guid applicantId)
        {
            var applications = await _groupService.GetApplicationsByApplicantIdAsync(applicantId);
            return Ok(applications);
        }

        [HttpPut("applications/{applicationId:guid}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, [FromBody] string status)
        {
            await _groupService.UpdateApplicationStatusAsync(applicationId, status);
            return NoContent();
        }

        [HttpDelete("applications/{applicationId:guid}")]
        public async Task<IActionResult> DeleteApplication(Guid applicationId)
        {
            await _groupService.DeleteApplicationAsync(applicationId);
            return NoContent();
        }
    }
}
