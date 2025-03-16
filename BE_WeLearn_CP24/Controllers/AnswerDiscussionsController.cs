using API.Extension.ClaimsPrinciple;
using API.SignalRHub;
using API.SwaggerOption.Const;
using APIExtension.Validator;
using DataLayer.DbObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;

namespace API.Controllers
{
    public class AnswerDiscussionsController : Controller
    {
        private readonly IServiceWrapper services;
        private readonly IHubContext<GroupHub> groupHub;
        //private readonly IMapper mapper;
        //private readonly IValidatorWrapper validators;

        public AnswerDiscussionsController(
            //IValidatorWrapper validators,
            IServiceWrapper services,
            IHubContext<GroupHub> groupHub)
        {
            this.services = services;
            this.groupHub = groupHub;
            //this.validators = validators;
        }

        [Authorize(Roles = Actor.Student)]
        [HttpGet("api/AnswerDiscussion/Get")]
        public async Task<IActionResult> GetAnswerDiscussionByDiscussionId(int discussionId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var discussions = await services.AnswersDiscussions.GetAnswerDiscussionByDiscussionId(discussionId);
                return Ok(discussions);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [Authorize(Roles = Actor.Student)]
        [HttpPost("api/AnswerDiscussion/Upload")]
        public async Task<IActionResult> UploadAnswerDiscussion(int accountId, int discussionId, UploadAnswerDiscussionDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                AnswerDiscussionDto ansDis = await services.AnswersDiscussions.UploadAnswerDiscussion(accountId, discussionId, dto);
                DiscussionDto discussion = await services.Discussions.GetDiscussionById(ansDis.DiscussionId);
                await groupHub.Clients.Group(discussion.GroupId.ToString()).SendAsync(GroupHub.OnReloadDicussionMsg, $"{HttpContext.User.GetUsername()} replies in disscusion {discussion.Question}");
                return Ok(ansDis);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [Authorize(Roles = Actor.Student)]
        [HttpPut("api/AnswerDiscussion/Update")]
        public async Task<IActionResult> UpdateAnswerDiscussion(int answerDiscussionId, UploadAnswerDiscussionDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                var accountId = HttpContext.User.GetUserId();
                var getdiscussion = await services.AnswersDiscussions.GetAnswerDiscussionById(answerDiscussionId);

                if(accountId != getdiscussion.AccountId)
                {
                    valResult.Add("Không thể thay đổi answerDiscussion của người khác", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                else
                {
                    var ansDis = await services.AnswersDiscussions.UpdateAnswerDiscussion(answerDiscussionId, dto);
                    DiscussionDto discussion = await services.Discussions.GetDiscussionById(ansDis.DiscussionId);
                    await groupHub.Clients.Group(discussion.GroupId.ToString()).SendAsync(GroupHub.OnReloadDicussionMsg, $"{HttpContext.User.GetUsername()} replies in disscusion {discussion.Question}");
                    return Ok(ansDis);
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
