using API.Extension.ClaimsPrinciple;
using API.SwaggerOption.Const;
using API.SwaggerOption.Endpoints;
using APIExtension.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IServiceWrapper services;

        public SchedulesController(IServiceWrapper services)
        {
            this.services = services;
        }

        //GET: api/Schedules/Group/groupId
        [SwaggerOperation(
            Summary = SchedulesEndpoints.GetSchedulesForGroup
        )]
        [Authorize(Roles = Actor.Student)]
        [HttpGet("Group/{groupId}")]
        public async Task<IActionResult> GetSchedulesForGroup(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int studentId = HttpContext.User.GetUserId();
                bool isJoined = await services.Groups.IsStudentJoiningGroupAsync(studentId, groupId);
                if (!isJoined)
                {
                    return Unauthorized("Bạn không phải là thành viên nhóm này");
                }
                IQueryable<ScheduleGetDto> mapped = services.Meetings.GetSchedulesForGroup(groupId);
                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
    }
}
