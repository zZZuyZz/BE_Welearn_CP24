using API.Extension.ClaimsPrinciple;
using API.SignalRHub;
using API.SwaggerOption.Const;
using API.SwaggerOption.Descriptions;
using API.SwaggerOption.Endpoints;
using APIExtension.Validator;
using AutoMapper;
using DataLayer.DbObject;
using DataLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMembersController : ControllerBase
    {
        private readonly IServiceWrapper services;
        //private readonly IValidatorWrapper validators;
        //private readonly IMapper mapper;
        private readonly IHubContext<GroupHub> groupHub;

        public GroupMembersController(
            IServiceWrapper services,
            //IValidatorWrapper validators, 
            //IMapper mapper, 
            IHubContext<GroupHub> groupHub
        )
        {
            this.services = services;
            //this.validators = validators;
            //this.mapper = mapper;
            this.groupHub = groupHub;
        }

        //GET: api/GroupMember/Group/{groupId}
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.GetJoinMembersForGroup
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Group/{groupId}")]
        public async Task<IActionResult> GetJoinMembersForGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isJoined = await services.Groups.IsStudentJoiningGroupAsync(studentId, groupId);
                if (!isJoined)
                {
                    valResult.Add("Bạn không phải là thành viên nhóm này", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                IQueryable<AccountProfileDto> mapped = services.GroupMembers.GetMembersJoinForGroup(groupId);
                if (mapped == null || !mapped.Any())
                {
                    valResult.Add("Nhóm không có thành viên", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }

                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //GET: api/GroupMember/Invite/Group/groupId
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.GetInviteForGroup
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Invite/Group/{groupId}")]
        public async Task<IActionResult> GetInviteForGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(studentId, groupId);
                if (!isLead)
                {
                    valResult.Add("You are not this group's leader", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                IQueryable<JoinInviteForGroupGetDto> mapped = services.GroupMembers.GetJoinInviteForGroup(groupId);
                if (mapped == null || !mapped.Any())
                {
                    valResult.Add("Nhóm không có lời mời", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }

                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //GET: api/GroupMember/Request/Group/groupId
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.GetRequestForGroup
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Request/Group/{groupId}")]
        public async Task<IActionResult> GetRequestForGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(studentId, groupId);
                if (!isLead)
                {
                    valResult.Add("You are not this group's leader", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                IQueryable<JoinRequestForGroupGetDto> mapped = services.GroupMembers.GetJoinRequestForGroup(groupId);
                if (mapped == null || !mapped.Any())
                {
                    valResult.Add("Nhóm không có yêu cầu vào nhóm", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }

                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //Post: api/GroupMember/Invite
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.CreateInvite
            , Description = GroupMembersDescriptions.CreateInvite
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpPost("Invite")]
        public async Task<IActionResult> CreateInvite(GroupMemberInviteCreateDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(studentId, dto.GroupId);
                if (!isLead)
                {
                    valResult.Add("You are not this group's leader", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                #region unused code
                //if (await services.Groups.IsStudentJoiningGroupAsync(dto.AccountId, dto.GroupId))
                //{
                //    //validatorResult.Failures.Add("Học sinh đã tham gia nhóm này");
                //    return BadRequest(new { Message = "Học sinh đã tham gia nhóm này" });
                //}
                //if (await services.Groups.IsStudentInvitedToGroupAsync(dto.AccountId, dto.GroupId))
                //{
                //    //validatorResult.Failures.Add("Học sinh đã được mời tham gia nhóm này từ trước");
                //    GroupMemberInviteGetDto inviteGetDto = mapper.Map<GroupMemberInviteGetDto>( 
                //        await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                //    return BadRequest(new { Message = "Học sinh đã được mời tham gia nhóm này từ trước", Previous = inviteGetDto });
                //}
                //if (await services.Groups.IsStudentRequestingToGroupAsync(dto.AccountId, dto.GroupId))
                //{
                //    //validatorResult.Failures.Add("Học sinh đã yêu cầu tham gia nhóm này từ trước");
                //    GroupMemberRequestGetDto requestGetDto = mapper.Map<GroupMemberRequestGetDto>(
                //        await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                //    return BadRequest(new { Message = "Học sinh đã yêu cầu tham gia nhóm này từ trước", Previous = requestGetDto });
                //}
                //if (await services.Groups.IsStudentDeclinedToGroupAsync(dto.AccountId, dto.GroupId))
                //{
                //    //validatorResult.Failures.Add("Học sinh đã từ chối/bị từ chối tham gia nhóm này từ trước");
                //    GroupMemberGetDto getDto = mapper.Map<GroupMemberGetDto>(
                //        await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                //    return BadRequest(new { 
                //        Message = "Học sinh đã từ chối/bị từ chối tham gia nhóm này từ trước. Hãy đợi tới tháng sau để thử lại", 
                //        Previous = getDto 
                //    });
                //}
                #endregion
                GroupMemberGetDto exsited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMemberGetDto>(dto.AccountId, dto.GroupId);
                if (exsited != null)
                {
                    if (!exsited.IsActive)
                    {
                        //GroupMemberGetDto getDto = mapper.Map<GroupMemberGetDto>(
                        //          await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //GroupMemberGetDto getDto = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId);
                        valResult.Add("Học sinh đã từ chối/bị từ chối tham gia nhóm này từ trước. Hãy đợi tới tháng sau để thử lại", ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    switch (exsited.MemberRole)
                    {
                        case GroupMemberRole.Leader:
                            {
                                valResult.Add("Học sinh đã tham gia nhóm này", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                        case GroupMemberRole.Member:
                            {
                                valResult.Add("Học sinh đã tham gia nhóm này", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                        #region old code
                        //Fix later
                        //case GroupMemberState.Inviting:
                        //    {
                        //        GroupMemberInviteGetDto inviteGetDto = mapper.Map<GroupMemberInviteGetDto>(
                        //            await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //        return BadRequest(new { Message = "Học sinh đã được mời tham gia nhóm này từ trước", Previous = inviteGetDto });
                        //    }
                        //case GroupMemberState.Requesting:
                        //    {
                        //        GroupMemberRequestGetDto requestGetDto = mapper.Map<GroupMemberRequestGetDto>(
                        //            await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //        return BadRequest(new { Message = "Học sinh đã yêu cầu tham gia nhóm này từ trước", Previous = requestGetDto });
                        //    }
                        //case GroupMemberRole.Banned:
                        //    {

                        //    }
                        #endregion
                        default:
                            {
                                //GroupMemberGetDto getDto = mapper.Map<GroupMemberGetDto>(
                                //    await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                                valResult.Add("Học sinh đã từ chối/bị từ chối tham gia nhóm này từ trước. Hãy đợi tới tháng sau để thử lại", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                    }
                }
                JoinInviteForGroupGetDto exsitedInvite = await services.GroupMembers.GetInviteOfStudentAndGroupAsync<JoinInviteForGroupGetDto>(dto.AccountId, dto.GroupId);
                if (exsitedInvite != null)
                {
                    valResult.Add("Học sinh đã được mời tham gia nhóm này từ trước", ValidateErrType.Role);
                    return BadRequest(valResult);

                }
                Request exsitedRequest = await services.GroupMembers.GetRequestOfStudentAndGroupAsync(dto.AccountId, dto.GroupId);
                if (exsitedRequest != null)
                {
                    valResult.Add("Học sinh đã yêu cầu tham gia nhóm này từ trước", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                await valResult.ValidateParamsAsync(services, dto, studentId);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                await services.GroupMembers.CreateJoinInvite(dto);

                await groupHub.Clients.Group("accId" + dto.AccountId.ToString()).SendAsync(GroupHub.OnReloadSelfInfoMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //Put: api/GroupMember/Request/{requestId}/Accept"
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.AcceptRequest
            , Description = GroupMembersDescriptions.AcceptRequest
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpPut("Request/{requestId}/Accept")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                Request existedRequest = await services.GroupMembers.GetRequestByIdAsync(requestId);
                if (existedRequest == null)
                {
                    valResult.Add("Yêu cầu tham gia không tồn tại", ValidateErrType.NotFound);
                    return BadRequest(valResult);
                }
                if (existedRequest.State == RequestStateEnum.Approved)
                {
                    valResult.Add("Yêu cầu tham gia đã được chấp nhận", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedRequest.State == RequestStateEnum.Decline)
                {
                    valResult.Add("Yêu cầu tham gia đã bị từ chối", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                GroupMember existedMember = await services.GroupMembers
                    .GetGroupMemberOfStudentAndGroupAsync<GroupMember>(existedRequest.AccountId, existedRequest.GroupId);
                if (existedMember != null)
                {
                    if (!existedMember.IsActive)
                    {
                        valResult.Add("Học sinh đã bị đuổi khỏi nhóm", ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    valResult.Add("Học sinh đã tham gia nhóm nhóm", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (!await services.Groups.IsStudentLeadingGroupAsync(studentId, existedRequest.GroupId))
                {
                    valResult.Add("Bạn không phải trưởng nhóm này", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                //if (existed.State != GroupMemberState.Requesting)
                //{
                //    return BadRequest("Đây không phải yêu cầu");
                //}
                await services.GroupMembers.AcceptOrDeclineRequestAsync(existedRequest, true);
                
                await groupHub.Clients.Group(existedRequest.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                await groupHub.Clients.Group("accId" + existedRequest.AccountId.ToString()).SendAsync(GroupHub.OnReloadSelfInfoMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }


        //Put: api/GroupMember/Request/{requestId}/Decline"
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.DeclineRequest
            , Description = GroupMembersDescriptions.DeclineRequest
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpPut("Request/{requestId}/Decline")]
        public async Task<IActionResult> DeclineRequest(int requestId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                Request existedRequest = await services.GroupMembers.GetRequestByIdAsync(requestId);
                if (existedRequest == null)
                {
                    valResult.Add("Yêu cầu tham gia không tồn tại", ValidateErrType.NotFound);
                    return BadRequest(valResult);
                }
                if (existedRequest.State == RequestStateEnum.Approved)
                {
                    valResult.Add("Yêu cầu tham gia đã được chấp nhận", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedRequest.State == RequestStateEnum.Decline)
                {
                    valResult.Add("Yêu cầu tham gia đã bị từ chối", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                GroupMember existedMember = await services.GroupMembers
                    .GetGroupMemberOfStudentAndGroupAsync<GroupMember>(existedRequest.AccountId, existedRequest.GroupId);
                if (existedMember != null)
                {
                    if (!existedMember.IsActive)
                    {
                        valResult.Add("Học sinh đã bị đuổi khỏi nhóm", ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    valResult.Add("Học sinh đã tham gia nhóm nhóm", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (!await services.Groups.IsStudentLeadingGroupAsync(studentId, existedRequest.GroupId))
                {
                    valResult.Add("Bạn không phải trưởng nhóm này", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                //if (existed.State != GroupMemberState.Requesting)
                //{
                //    return BadRequest("Đây không phải yêu cầu");
                //}
                await services.GroupMembers.AcceptOrDeclineRequestAsync(existedRequest, false);
                await groupHub.Clients.Group(existedRequest.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //GET: api/GroupMember/Invite/Student/{studentId}
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.GetInviteForStudent
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Invite/Student")]
        public async Task<IActionResult> GetInviteForStudent()
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                IQueryable<JoinInviteForStudentGetDto> mapped = services.GroupMembers.GetJoinInviteForStudent(studentId);
                if (mapped == null || !mapped.Any())
                {
                    valResult.Add("Người dùng không có lời mời", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }

                return base.Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //GET: api/GroupMember/Request/Student/{studentId}
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.GetRequestForStudent
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Request/Student")]
        public async Task<IActionResult> GetRequestForStudent()
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                IQueryable<JoinRequestForStudentGetDto> mapped = services.GroupMembers.GetJoinRequestForStudent(studentId);
                if (mapped == null || !mapped.Any())
                {
                    valResult.Add("Người dùng không có yêu cầu", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }

                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //POST: api/GroupMember/Request
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.CreateRequest
            , Description = GroupMembersDescriptions.CreateRequest
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpPost("Request")]
        public async Task<IActionResult> CreateRequest(GroupMemberRequestCreateDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(studentId, dto.GroupId);
                if (studentId != dto.AccountId)
                {
                    valResult.Add("Bạn không thể yêu cầu tham gia dùm người khác", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                GroupMemberGetDto exsitedGroupMember = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMemberGetDto>(dto.AccountId, dto.GroupId);
                if (exsitedGroupMember != null)
                {
                    if (!exsitedGroupMember.IsActive)
                    {
                        valResult.Add("Bạn đã từ chối/bị từ chối tham gia nhóm này từ trước. Hãy đợi tới tháng sau để thử lại", ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    switch (exsitedGroupMember.MemberRole)
                    {
                        case GroupMemberRole.Leader:
                            {
                                valResult.Add("Bạn đã tham gia nhóm này", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                        case GroupMemberRole.Member:
                            {
                                valResult.Add("Bạn đã tham gia nhóm này", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                        #region old code
                        //Fix later
                        //case GroupMemberState.Inviting:
                        //    {
                        //        GroupMemberInviteGetDto inviteGetDto = mapper.Map<GroupMemberInviteGetDto>(
                        //            await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //        return BadRequest(new { Message = "Bạn đã được mời tham gia nhóm này từ trước", Previous = inviteGetDto });
                        //    }
                        //case GroupMemberState.Requesting:
                        //    {
                        //        GroupMemberRequestGetDto requestGetDto = mapper.Map<GroupMemberRequestGetDto>(
                        //            await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //        return BadRequest(new { Message = "Bạn đã yêu cầu tham gia nhóm này từ trước", Previous = requestGetDto });
                        //    }
                        //case GroupMemberRole.Banned:
                        //    {
                        //        GroupMemberGetDto getDto = mapper.Map<GroupMemberGetDto>(
                        //            await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync(dto.AccountId, dto.GroupId));
                        //        return BadRequest(new
                        //        {
                        //            Message = "Bạn đã từ chối/bị từ chối tham gia nhóm này từ trước. Hãy đợi tới tháng sau để thử lại",
                        //            Previous = getDto
                        //        });
                        //    }
                        #endregion
                        default:
                            {
                                valResult.Add("Bạn đã có liên quan đến nhóm", ValidateErrType.Role);
                                return BadRequest(valResult);
                            }
                    }
                }
                JoinInviteForGroupGetDto exsitedInvite = await services.GroupMembers.GetInviteOfStudentAndGroupAsync<JoinInviteForGroupGetDto>(dto.AccountId, dto.GroupId);
                if (exsitedInvite != null)
                {
                    valResult.Add("Bạn đã được mời tham gia nhóm này từ trước", ValidateErrType.Role);
                    return BadRequest(valResult);

                }
                Request exsitedRequest = await services.GroupMembers.GetRequestOfStudentAndGroupAsync(dto.AccountId, dto.GroupId);
                if (exsitedRequest != null)
                {
                    valResult.Add("Bạn đã yêu cầu tham gia nhóm này từ trước", ValidateErrType.Role);
                    return BadRequest(valResult);
                }

                await valResult.ValidateParamsAsync(services, dto);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                await services.GroupMembers.CreateJoinRequest(dto);
                await groupHub.Clients.Group(dto.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //Put: api/GroupMember/Invite/{inviteId}/Accept"
        [SwaggerOperation(
            Summary = GroupMembersEndpoints.AcceptInvite
            , Description = GroupMembersDescriptions.AcceptInvite
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpPut("Invite/{inviteId}/Accept")]
        public async Task<IActionResult> AcceptInvite(int inviteId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                Invite existedInvite = await services.GroupMembers.GetInviteByIdAsync(inviteId);
                if (existedInvite == null || existedInvite.Group.IsBanned == true)
                {
                    valResult.Add("Lời mời tham gia không tồn tại", ValidateErrType.NotFound);
                    return BadRequest(valResult);
                }
                if (existedInvite.State == RequestStateEnum.Approved)
                {
                        valResult.Add("Lời mời tham gia đã được chấp nhận", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedInvite.State == RequestStateEnum.Decline)
                {
                    valResult.Add("Lời mời tham gia đã bị từ chối",ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                GroupMember existedMember = await services.GroupMembers
                    .GetGroupMemberOfStudentAndGroupAsync<GroupMember>(existedInvite.AccountId, existedInvite.GroupId);
                if (existedMember != null)
                {
                    if (!existedMember.IsActive)
                    {
                        valResult.Add("Học sinh đã bị đuổi khỏi nhóm",ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    valResult.Add("Học sinh đã tham gia nhóm nhóm", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedInvite.AccountId != studentId)
                {
                    valResult.Add("Đây không phải lời mời cho bạn", ValidateErrType.Unauthorized);
                    return BadRequest(valResult);
                }
                await services.GroupMembers.AcceptOrDeclineInviteAsync(existedInvite, true);
                await groupHub.Clients.Group(existedInvite.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }


        //Put: api/GroupMember/Invite/{inviteId}/Decline"
        [SwaggerOperation(
           Summary = GroupMembersEndpoints.DeclineInvite
           , Description = GroupMembersDescriptions.DeclineInvite
       )]
        [Authorize(Roles = Actor.Student)]
        [HttpPut("Invite/{inviteId}/Decline")]
        public async Task<IActionResult> DeclineInvite(int inviteId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                Invite existedInvite = await services.GroupMembers.GetInviteByIdAsync(inviteId);
                if (existedInvite == null)
                {
                    valResult.Add("Lời mời tham gia không tồn tại", ValidateErrType.NotFound);
                    return BadRequest(valResult);
                }
                if (existedInvite.State == RequestStateEnum.Approved)
                {
                    valResult.Add("Lời mời tham gia đã được chấp nhận", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedInvite.State == RequestStateEnum.Decline)
                {
                    valResult.Add("Lời mời tham gia đã bị từ chối", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                GroupMember existedMember = await services.GroupMembers
                    .GetGroupMemberOfStudentAndGroupAsync<GroupMember>(existedInvite.AccountId, existedInvite.GroupId);
                if (existedMember != null)
                {
                    if (!existedMember.IsActive)
                    {
                        valResult.Add("Học sinh đã bị đuổi khỏi nhóm", ValidateErrType.Role);
                        return BadRequest(valResult);
                    }
                    valResult.Add("Học sinh đã tham gia nhóm nhóm", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (existedInvite.AccountId != studentId)
                {
                    valResult.Add("Đây không phải lời mời cho bạn", ValidateErrType.Unauthorized);
                    return BadRequest(valResult);
                }
                await services.GroupMembers.AcceptOrDeclineInviteAsync(existedInvite, false);
                await groupHub.Clients.Group(existedInvite.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
        //Delete
        //: api/GroupMember/Invite/{inviteId}/Decline"
        [SwaggerOperation(
           Summary = GroupMembersEndpoints.BanMember
           , Description = GroupMembersDescriptions.BanMember
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpDelete("Group/{groupId}/Account/{banAccId}")]
        public async Task<IActionResult> BanMember(int groupId, int banAccId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(studentId, groupId);
                if (!isLead)
                {
                    valResult.Add("You are not this group's leader", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                if (studentId == banAccId)
                {
                    valResult.Add("Bạn không thể đuổi chính mình", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                GroupMember exited = await services.GroupMembers.GetGroupMemberOfStudentAndGroupAsync<GroupMember>(banAccId, groupId);
                if (exited == null)
                {
                    valResult.Add("Học sinh không có tham gia nhóm này", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                if (!exited.IsActive)
                {
                    valResult.Add("Học sinh đã bị đuổi khỏi nhóm này", ValidateErrType.Role);
                    return BadRequest(valResult);
                }
                await services.GroupMembers.BanUserFromGroupAsync(exited);
                await groupHub.Clients.Group("accId"+ banAccId.ToString()).SendAsync(GroupHub.OnReloadSelfInfoMsg);
                await groupHub.Clients.Group(groupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
        [SwaggerOperation(
           Summary = GroupMembersEndpoints.LeaveGroup
           , Description = GroupMembersDescriptions.LeaveGroup
       )]
        [Authorize(Roles = Actor.Student)]
        [HttpPut("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var accountId = HttpContext.User.GetUserId();
                var username = HttpContext.User.GetEmail();
                bool isLead = await services.Groups.IsStudentLeadingGroupAsync(accountId, groupId);
                if (isLead)
                {
                    valResult.Add("Bạn là nhóm trưởng của nhóm này, không thể rời nhóm", ValidateErrType.Role);
                    return Unauthorized(valResult);
                }
                var check = await services.GroupMembers.LeaveGroup(accountId, groupId);
                await groupHub.Clients.Group("accId" + accountId.ToString()).SendAsync(GroupHub.OnReloadSelfInfoMsg);
                await groupHub.Clients.Group(groupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg, $"Student {username} leaves group");
                return Ok(check);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
        private async Task<bool> GroupMemberExists(int id)
        {
            return (await services.GroupMembers.AnyAsync(id));
        }
        private async Task<bool> InviteExists(int studentId, int groupId)
        {
            return (await services.GroupMembers.AnyInviteAsync(studentId, groupId));
        }
        private async Task<bool> RequestExists(int studentId, int groupId)
        {
            return (await services.GroupMembers.AnyRequestAsync(studentId, groupId));
        }
    }
}
