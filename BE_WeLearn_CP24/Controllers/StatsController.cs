using API.Extension.ClaimsPrinciple;
using API.SwaggerOption.Const;
using API.SwaggerOption.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;
using Swashbuckle.AspNetCore.Annotations;
using API.SwaggerOption.Descriptions;
using APIExtension.Validator;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IServiceWrapper services;

        public StatsController(IServiceWrapper services)
        {
            this.services = services;
        }

        [SwaggerOperation(
            Summary = StatsEndpoints.GetStatForAccountInMonthNew
            , Description = StatsDescriptions.GetStatForAccountInMonthNew
        )]
        [HttpGet("Account/{studentId}/{month}")]
        [Authorize(Roles = Actor.Student_Parent)]
        public async Task<IActionResult> GetStatForAccountInMonth(int studentId, DateTime month)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (studentId < 0)
                {
                    return NotFound();
                }
                if (HttpContext.User.IsInRole(Actor.Student) && HttpContext.User.GetUserId() != studentId)
                {
                    return Unauthorized("Bạn không thể xem dữ liệu của học sinh khác");
                }
                var mappedStat = await services.Stats.GetStatForAccountInMonth(studentId, month);
                return Ok(mappedStat);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = StatsEndpoints.GetStatForGroupInMonthNew
            , Description = StatsDescriptions.GetStatForGroupInMonthNew
        )]
        [HttpGet("Group/{groupId}/{month}")]
        [Authorize(Roles = Actor.Student_Parent)]
        public async Task<IActionResult> GetStatForGroupInMonth(int groupId, DateTime month)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (groupId < 0)
                {
                    return NotFound();
                }
                //if (HttpContext.User.GetUserId())
                //{
                //    return Unauthorized("Bạn không thể xem dữ liệu của học sinh khác");
                //}
                var mappedStat = await services.Stats.GetStatForGroupInMonth(groupId, month);
                return Ok(mappedStat);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = StatsEndpoints.GetStatForStudent
            , Description = StatsDescriptions.GetStatForStudent
        )]
        [HttpGet("ForStudentInMonths/{studentId}")]
        [Authorize(Roles = Actor.Student_Parent)]
        public async Task<IActionResult> GetStatForStudentInMonths(int studentId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (studentId < 0)
                {
                    return NotFound();
                }
                if (HttpContext.User.IsInRole(Actor.Student) && HttpContext.User.GetUserId() != studentId)
                {
                    return Unauthorized("Bạn không thể xem dữ liệu của học sinh khác");
                }
                var mappedStat = await services.Stats.GetStatsForStudent(studentId);
                return Ok(mappedStat);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.Message);
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = StatsEndpoints.GetStatForGroup
            , Description = StatsDescriptions.GetStatForGroup
        )]
        [HttpGet("ForGroupInMonths")]
        [Authorize(Roles = Actor.Student_Parent)]
        public async Task<IActionResult> GetStatForGroupinMonths(int groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (groupId < 0)
                {
                    return NotFound();
                }
                var mappedStat = await services.Stats.GetStatsForGroup(groupId);
                return Ok(mappedStat);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.Message);
                return BadRequest(valResult);
            }
        }
    }
}
