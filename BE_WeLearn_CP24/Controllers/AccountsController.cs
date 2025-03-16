using APIExtension.Validator;
using AutoMapper;
using DataLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Utils;
using Swashbuckle.AspNetCore.Annotations;
using API.SwaggerOption.Endpoints;
using API.SwaggerOption.Const;
using API.SwaggerOption.Descriptions;
using API.Extension.ClaimsPrinciple;
using ServiceLayer.Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IServiceWrapper services;
        //private readonly IMapper mapper;
        //private readonly IValidatorWrapper validators;
        //private readonly IAutoMailService mailService;
        private readonly IServer server;

        public AccountsController(
            IServiceWrapper services,
            //IMapper mapper, 
            //IValidatorWrapper validators, 
            /*IAutoMailService mailService,*/
            IServer server)
        {
            this.services = services;
            //this.mapper = mapper;
            //this.validators = validators;
            //this.mailService = mailService;
            this.server = server;
        }
        private static string FAIL_CONFIRM_PASSWORD_MSG => "Xác nhận mật khẩu thất bại";

        //Get: api/Accounts/search
        [SwaggerOperation(
            Summary = AccountsEndpoints.SearchStudent
            , Description = AccountsDescriptions.SearchStudent
        )]
        [Authorize(Roles = Actor.Student_Parent)]
        [HttpGet("search")]
        public async Task<IActionResult> SearchStudent(string search, int? groupId)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                bool isParent = HttpContext.User.IsInRole(Actor.Parent);
                int? parentId = isParent ? HttpContext.User.GetUserId() : null;
                //var list = services.Accounts.SearchStudents(search, groupId, parentId);//.ToList();

                //if (list == null)
                //{
                //    return NotFound();
                //}
                //var mapped = list.ProjectTo<AccountProfileDto>(mapper.ConfigurationProvider);
                var mapped = services.Accounts.SearchStudents<AccountProfileDto>(search, groupId, parentId);//.ToList();
                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        // GET: api/Accounts/5
        [SwaggerOperation(
            Summary = AccountsEndpoints.GetProfile
            , Description = AccountsDescriptions.GetProfile
        )]
        [Authorize(Roles = Actor.Student_Parent)]
        //[Authorize(Roles = "Student, Parent")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                int id = HttpContext.User.GetUserId();
                //Account user = await services.Accounts.GetProfileByIdAsync(id);

                //if (user == null)
                //{
                //    return NotFound();
                //}
                //      var mapped = mapper.Map<AccountProfileDto>(user);
                AccountProfileDto mapped = await services.Accounts.GetProfileByIdAsync<AccountProfileDto>(id);
                return Ok(mapped);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }


        // PUT: api/Accounts/5
        [SwaggerOperation(
            Summary = AccountsEndpoints.UpdatePRofile
            , Description = AccountsDescriptions.UpdateProfile
        )]
        [Authorize(Roles = "Student, Parent")]
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateProfile(int accountId,[FromForm] AccountUpdateDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (accountId != HttpContext.User.GetUserId())
                {

                    valResult.Add("Không thể thay đổi profile của người khác", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                //if (accountId != dto.Id)
                //{
                //    valResult.Add("2 Id không trùng", ValidateErrType.IdNotMatch);
                //    return BadRequest(valResult);
                //}

                var existedAccount = await services.Accounts.ExistAsync(accountId);
                if (!existedAccount)
                {
                    return NotFound();
                }
                //ValidatorResult valResult = await validators.Accounts.ValidateParams(dto);
                await valResult.ValidateParams(services, dto, accountId);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                try
                {
                    var account = await services.Accounts.UpdateAsync( accountId, dto);
                    return Ok(account);
                }
                catch (Exception ex)
                {
                    if (!await UserExists(accountId))
                    {
                        valResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        // PUT: api/Accounts/5/Password
        [SwaggerOperation(
            Summary = AccountsEndpoints.ChangePassword
            , Description = AccountsDescriptions.ChangePassword
        )]
        [Authorize(Roles = Actor.Student_Parent)]
        [HttpPut("{accountId}/Password")]
        public async Task<IActionResult> ChangePassword(int accountId, AccountChangePasswordDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (accountId != HttpContext.User.GetUserId())
                {

                    valResult.Add("Không thể thay đổi profile của người khác", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                //if (accountId != dto.Id)
                //{
                //    valResult.Add("2 Id không trùng", ValidateErrType.IdNotMatch);
                //    return BadRequest(valResult);
                //}
                //ValidatorResult valResult = await validators.Accounts.ValidateParams(dto);
                await valResult.ValidateParams(services, dto, accountId);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                //if (dto.Password != dto.ConfirmPassword)
                //{
                //    return BadRequest(FAIL_CONFIRM_PASSWORD_MSG);
                //}

                //var account = await services.Accounts.GetByIdAsync<Account>(accountId);
                //if (account == null)
                //{
                //    return NotFound();
                //}
                //if(dto.OldPassword != account.Password)
                //{
                //    return Unauthorized("Nhập mật khẩu cũ thất bại");
                //}
                try
                {
                    //account.PatchUpdate<Account, AccountChangePasswordDto>(dto);
                    //await services.Accounts.UpdateAsync(account);
                    var account = await services.Accounts.UpdatePasswordAsync(accountId, dto);
                    return Ok(account);
                }
                catch (Exception ex)
                {
                    if (!await UserExists(accountId))
                    {
                        valResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                        return NotFound(valResult);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = AccountsEndpoints.ConfirmResetPassword
            , Description = AccountsDescriptions.ConfirmResetPassword
        )]
        [HttpGet("Password/Reset")]
        public async Task<IActionResult> ConfirmResetPassword(string email)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                bool existedAccount = await services.Accounts.ExistEmailAsync(email);
                if (!existedAccount)
                {
                    valResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }
                #region old code
                //Random password
                //string newPassword = RandomPassword(9);
                //account.Password = newPassword;
                //await services.Accounts.UpdateAsync(account);

                ////string mailContent="<a href=\"localhost\"></a>" 
                //string mailContent = $"<div>Mật khẩu mới của bạn là {newPassword}</div>";
                //bool sendSuccessful = await mailService.SendEmailWithDefaultTemplateAsync(new List<String> { email }, "Reset password", mailContent, null);
                #endregion
                bool sendSuccessful = await services.Mails.SendConfirmResetPasswordMailAsync(email, server.Features.Get<IServerAddressesFeature>().Addresses.First());
                if (!sendSuccessful)
                {
                    //return BadRequest($"Something went wrong with sending mail. The new password is {newPassword}");
                    valResult.Add("Gặp lỗi khi gửi mail");
                    return BadRequest(valResult);
                }
                //return Ok($"Reset successfully, check {email} inbox for the new password {newPassword}");
                return Ok($"Reset successfully, check {email} inbox for the new password");
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = AccountsEndpoints.ConfirmResetPassNoUse
            , Description = AccountsDescriptions.ConfirmResetPassNoUse
        )]
        [HttpGet("Password/Reset/Confirm")]

        public async Task<IActionResult> ConfirmResetPassword(string email, string secret)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                //Account account = await services.Accounts.GetAccountByEmailAsync(email);
                //if (account == null)
                //{
                //    return NotFound("Tài khoản không tồn tại");
                //}
                bool existedAccount = await services.Accounts.ExistEmailAsync(email);
                if (!existedAccount)
                {
                    valResult.Add("Tài khoản không tồn tại", ValidateErrType.NotFound);
                    return NotFound(valResult);
                }
                //test secret
                string correctSecrete = DateTime.Today.ToString("yyyy-MM-dd").CustomHash();
                if (secret != correctSecrete)
                {
                    valResult.Add("Không đúng secret", ValidateErrType.Unauthorized);
                    return Unauthorized(valResult);
                }
                #region old code
                //Random password
                //string newPassword = RandomPassword(9);
                //account.Password = newPassword;
                //await services.Accounts.UpdateAsync(account);

                ////string mailContent="<a href=\"localhost\"></a>" 
                //string mailContent = $"<div>Mật khẩu mới của bạn là {newPassword}</div>";
                //bool sendSuccessful = await mailService.SendEmailWithDefaultTemplateAsync(new List<String> { email }, "Reset password", mailContent, null);
                #endregion
                bool sendSuccessful = await services.Mails.SendNewPasswordMailAsync(email);
                if (!sendSuccessful)
                {
                    //return BadRequest($"Something went wrong with sending mail. The new password is {newPassword}");
                    valResult.Add("Gặp lỗi khi gửi mail");
                    return BadRequest(valResult);
                }
                //return Ok($"Reset successfully, check {email} inbox for the new password {newPassword}");
                //return Ok();
                return Ok($"Reset successfully, check {email} inbox for the new password");
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
        #region Not copy 22/1       
        //[Authorize(Roles = Actor.Student)]
        //[SwaggerOperation(
        //    Summary = AccountsEndpoints.GetSuperviseRequest
        //)]
        //[HttpGet("Supervise/Student")]
        //public async Task<IActionResult> GetSuperViseRequestForStudent()
        //{
        //    int studentId = HttpContext.User.GetUserId();
        //    //bool isParentStudentRelated = await services.Accounts.IsParentStudentRelated(parentId, studentId);
        //    //IQueryable<Supervision> list = services.Accounts.GetWaitingSupervisionForStudent(studentId);

        //    //IQueryable<WaitingSupervisonGetDto> mapped = list.ProjectTo<WaitingSupervisonGetDto>(mapper.ConfigurationProvider);
        //    IQueryable<WaitingSupervisonGetDto> mapped = services.Accounts.GetWaitingSupervisionForStudent<WaitingSupervisonGetDto>(studentId);
        //    return Ok(mapped);
        //}

        //[SwaggerOperation(
        //    Summary = AccountsEndpoints.RequestSupervise
        //)]
        //[HttpPost("Supervise/{studentId}")]
        //public async Task<IActionResult> RequestSuperVise(int studentId)
        //{
        //    int parentId = HttpContext.User.GetUserId();
        //    //bool isParentStudentRelated = await services.Accounts.IsParentStudentRelated(parentId, studentId);
        //    Supervision parentStudentRelation = await services.Accounts.GetParentStudentRelationAsync(parentId, studentId);
        //    if (parentStudentRelation != null)
        //    {
        //        if (parentStudentRelation.State == RequestStateEnum.Approved)
        //        {
        //            return BadRequest("Học sinh đã dưới sự quản lí của bạn");
        //        }
        //        if (parentStudentRelation.State == RequestStateEnum.Decline)
        //        {
        //            return BadRequest("Học sinh đã từ chối");
        //        }
        //        //if (parentStudentRelation.State == RequestStateEnum.Waiting)
        //        //{
        //        return BadRequest("Đang chờ học sinh trả lời");
        //        //}
        //    }
        //    Supervision created = await services.Accounts.CreateSuperviseRequestAsync(parentId, studentId);
        //    return Ok(created);
        //}

        //[Authorize(Roles = Actor.Student)]
        //[SwaggerOperation(
        //    Summary = AccountsEndpoints.AcceptSupervise
        //)]
        //[HttpPut("Superise/{supervisionId}/Accept")]
        //public async Task<IActionResult> AcceptSuperVise(int supervisionId)
        //{
        //    Supervision updated = await services.Accounts.GetWaitingSupervisionByIdAsync<Supervision>(supervisionId);
        //    if (updated == null)
        //    {
        //        return NotFound();
        //    }
        //    if (updated.StudentId != HttpContext.User.GetUserId())
        //    {
        //        return Unauthorized("Lời yêu cầu này không dành cho bạn");
        //    }
        //    if (updated.State == RequestStateEnum.Approved)
        //    {
        //        return BadRequest("Bạn đã đồng ý rồi");
        //    }
        //    if (updated.State == RequestStateEnum.Decline)
        //    {
        //        return BadRequest("Bạn đã từ chối rồi");
        //    }
        //    updated.State = RequestStateEnum.Approved;
        //    await services.Accounts.UpdateSupervisionAsync(updated);
        //    return Ok(updated);
        //}

        //[SwaggerOperation(
        //   Summary = AccountsEndpoints.DeclineSupervise
        //)]
        //[HttpPut("Superise/{supervisionId}/Decline")]
        //public async Task<IActionResult> DeclineSuperVise(int supervisionId)
        //{
        //    Supervision updated = await services.Accounts.GetWaitingSupervisionByIdAsync<Supervision>(supervisionId);
        //    if (updated == null)
        //    {
        //        return NotFound();
        //    }
        //    if (updated.StudentId != HttpContext.User.GetUserId())
        //    {
        //        return Unauthorized("Lời yêu cầu này không dành cho bạn");
        //    }
        //    if (updated.State == RequestStateEnum.Approved)
        //    {
        //        return BadRequest("Bạn đã đồng ý rồi");
        //    }
        //    if (updated.State == RequestStateEnum.Decline)
        //    {
        //        return BadRequest("Bạn đã từ chối rồi");
        //    }
        //    updated.State = RequestStateEnum.Decline;
        //    await services.Accounts.UpdateSupervisionAsync(updated);
        //    return Ok(updated);
        //}

        //[SwaggerOperation(
        //   Summary = AccountsEndpoints.DeleteSupervise
        //)]
        //[HttpDelete("Superise/{supervisionId}")]
        //public async Task<IActionResult> DeleteSuperVise(int supervisionId)
        //{
        //    //Supervision delete = await services.Accounts.GetWaitingSupervisionByIdAsync(supervisionId);
        //    Supervision delete = await services.Accounts.GetSupervisionByIdAsync<Supervision>(supervisionId);
        //    if (delete == null)
        //    {
        //        return NotFound();
        //    }
        //    if (HttpContext.User.IsInRole(Actor.Student) && delete.StudentId != HttpContext.User.GetUserId())
        //    {
        //        return Unauthorized("Bạn không thể xóa phụ huynh này");
        //    }
        //    if (HttpContext.User.IsInRole(Actor.Parent) && delete.ParentId != HttpContext.User.GetUserId())
        //    {
        //        return Unauthorized("Bạn không thể xóa học sinh này");
        //    }
        //    await services.Accounts.DeleteSupervisionAsync(delete);
        //    return Ok();
        //}

        //[Authorize(Roles = Actor.Student)]
        //[SwaggerOperation(
        //    Summary = AccountsEndpoints.GetParentForStudent
        //)]
        //[HttpGet("Parents")]
        //public async Task<IActionResult> GetParentsForStudents()
        //{
        //    //int studentId = HttpContext.User.GetUserId();
        //    //IQueryable<Account> list = services.Accounts.GetParentsOfStudent(studentId);
        //    //if (list == null || !list.Any())
        //    //{
        //    //    return NotFound();
        //    //}
        //    //var mapped = list.ProjectTo<StudentGetDto>(mapper.ConfigurationProvider);
        //    //return Ok(mapped);

        //    int studentId = HttpContext.User.GetUserId();
        //    //IQueryable<Supervision> list = services.Accounts.GetAcceptedSupervisionForStudent(studentId);
        //    //IQueryable<WaitingSupervisonGetDto> mapped = list.ProjectTo<WaitingSupervisonGetDto>(mapper.ConfigurationProvider);
        //    IQueryable<WaitingSupervisonGetDto> mapped = services.Accounts.GetAcceptedSupervisionForStudent<WaitingSupervisonGetDto>(studentId);
        //    return Ok(mapped);
        //    //return Ok();
        //}

        //[Authorize(Roles = Actor.Parent)]
        //[SwaggerOperation(
        //    Summary = AccountsEndpoints.GetStudentForParent
        //)]
        //[HttpGet("Students")]
        //public async Task<IActionResult> GetStudentsForParents()
        //{
        //    //int parentId = HttpContext.User.GetUserId();
        //    //IQueryable<Account> list = services.Accounts.GetStudentsOfParent(parentId);
        //    //if (list == null || !list.Any())
        //    //{
        //    //    return NotFound();
        //    //}
        //    //var mapped = list.ProjectTo<StudentGetDto>(mapper.ConfigurationProvider);
        //    //return Ok(mapped);

        //    int parentId = HttpContext.User.GetUserId();
        //    //IQueryable<Supervision> list = services.Accounts.GetAcceptedSupervisionForParent(parentId);
        //    //IQueryable<WaitingSupervisonGetDto> mapped = list.ProjectTo<WaitingSupervisonGetDto>(mapper.ConfigurationProvider);
        //    IQueryable<WaitingSupervisonGetDto> mapped = services.Accounts.GetAcceptedSupervisionForParent<WaitingSupervisonGetDto>(parentId);
        //    return Ok(mapped);
        //    //return Ok();
        //}
        #endregion

        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        [Tags(Actor.Test)]
        [SwaggerOperation(
            Summary = AccountsEndpoints.GetAllAccount
        )]
        // GET: api/Accounts
        [HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            IQueryable<StudentGetDto> mapped = services.Accounts.GetList<StudentGetDto>();
            return Ok(mapped);
        }


        // GET: api/Accounts/5
        [Tags(Actor.Test)]
        [SwaggerOperation(
          Summary = AccountsEndpoints.GetUser
        )]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var mapped = await services.Accounts.GetByIdAsync<StudentGetDto>(id);
            return Ok(mapped);
        }

        private async Task<bool> UserExists(int id)
        {
            return await services.Accounts.ExistAsync(id);
        }

    }
}
