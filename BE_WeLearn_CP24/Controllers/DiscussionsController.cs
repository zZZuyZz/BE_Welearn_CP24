using API.Extension.ClaimsPrinciple;
using API.SignalRHub;
using API.SwaggerOption.Const;
using APIExtension.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;

namespace API.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly IServiceWrapper services;
        private readonly IHubContext<GroupHub> groupHub;
        //private readonly IMapper mapper;
        //private readonly IValidatorWrapper validators;

        public DiscussionsController(
            IServiceWrapper services,
            IHubContext<GroupHub> groupHub
        ){
            this.services = services;
            this.groupHub = groupHub;
            //this.validators = validators;
        }

        [Authorize(Roles = Actor.Student)]
        [HttpGet("api/Discussion/Get/{groupId}")]
        public async Task<IActionResult> GetDiscussionInGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {

                var discussions = await services.Discussions.GetDiscussionsByGroupId(groupId);
                return Ok(discussions);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [Authorize(Roles = Actor.Student)]
        [HttpGet("api/Discussion/GetByDiscussionId/")]
        public async Task<IActionResult> GetDiscussionDetail(int dicussionId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var discussions = await services.Discussions.GetDiscussionById(dicussionId);
                return Ok(discussions);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }


        [Authorize(Roles = Actor.Student)]
        [HttpPost("api/Discussion/Upload")]
        public async Task<IActionResult> UploadDiscussion(int accountId, int groupId, [FromForm] UploadDiscussionDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var discussion = await services.Discussions.UploadDiscussion(accountId, groupId, dto);
                await groupHub.Clients.Group(groupId.ToString()).SendAsync(GroupHub.OnReloadDicussionMsg, $"{HttpContext.User.GetUsername()} post new disscusion {dto.Question}");
                return Ok(discussion);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //[Authorize(Roles = Actor.Student)]
       
        [HttpPost("api/Discussion/Upload/File")]
        public async Task<IActionResult> UploadDiscussionFile([FromForm] FileInput file)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var url = await services.Discussions.UploadDiscussionFile(file.file);
                return Ok(url);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [Authorize(Roles = Actor.Student)]
        [HttpPut("api/Discussion/Update")]
        public async Task<IActionResult> UpdateDiscussion(int discussionId, UploadDiscussionDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var accountId = HttpContext.User.GetUserId();
                var getdiscussion = await services.Discussions.GetDiscussionById(discussionId);

                if(accountId != getdiscussion.AccountId)
                {
                    valResult.Add("Không thể thay đổi discussion của người khác", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                else
                {
                    var discussion = await services.Discussions.UpdateDiscussion(discussionId, dto);
                    await groupHub.Clients.Group(discussion.GroupId.ToString()).SendAsync(GroupHub.OnReloadDicussionMsg, $"{HttpContext.User.GetUsername()} post new disscusion {dto.Question}");
                    return Ok(discussion);
                }
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
    }
}
