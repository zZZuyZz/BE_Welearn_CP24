using APIExtension.Validator;
using AutoMapper;
using DataLayer.DbObject;
using DataLayer.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceLayer.DTOs;
using ServiceLayer.Utils;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http.Headers;
using System.Security.Claims;
using API.SwaggerOption.Endpoints;
using API.SwaggerOption.Descriptions;
using API.Extension.HttpContext;
using API.SwaggerOption.Const;
using ServiceLayer.Services.Interface;
using Firebase.Storage;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceWrapper services;
        //private readonly IMapper mapper;
        //private readonly IValidatorWrapper validators;
        private readonly IConfiguration _config;
        public AuthController(
            IServiceWrapper services,
            IConfiguration config
        //IMapper mapper, 
        //IValidatorWrapper validators
        )
        {
            this.services = services;
            this._config = config;
            //this.mapper = mapper;
            //this.validators = validators;
        }



        [SwaggerOperation(
            Summary = AuthEndpoints.Login,
            Description = AuthDescriptions.Login
        )]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel loginModel)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                LoginInfoDto logined = await services.Auth.LoginAsync(loginModel);
                if (logined is null)
                {
                    valResult.Add("Sai username hoặc password");
                    return Unauthorized(valResult);
                }
                if (logined.IsBanned == true)
                {
                    valResult.Add("Tài khoản đã bị vô hiệu");
                    return Unauthorized(valResult);
                }
                //string token = await services.Auth.GenerateJwtAsync(logined, loginModel.RememberMe);
                //return base.Ok(new { token = token, Id = logined.Id, Username = logined.Username, Email = logined.Email, Role = logined.Role.Name });
                return Ok(logined);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
            Summary = AuthEndpoints.LoginWithGGIdToken,
            Description = AuthDescriptions.LoginWithGGId
        )]
        //[CustomGoogleIdTokenAuthFilter]
        [HttpPost("Login/Google/Id-Token")]
        public async Task<IActionResult> LoginWithGoogleIdTokenAsync(string? idToken, bool rememberMe = true)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                //var idToken = await HttpContext.GetTokenAsync("access_token");
                if (idToken is null || idToken.Length == 0)
                {
                    idToken = HttpContext.GetGoogleIdToken();
                }
                LoginInfoDto logined = await services.Auth.LoginWithGoogleIdToken(idToken, rememberMe);
                if (logined is null)
                {
                    valResult.Add("Bạn chưa đăng kí tài khoản");
                    return Unauthorized(valResult);
                }
                //string token = await services.Auth.GenerateJwtAsync(logined, rememberMe);
                //return Ok(new { token = token, Id = logined.Id, Username = logined.Username, Email = logined.Email, Role = logined.Role.Name });
                return Ok( logined);

            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
           Summary = AuthEndpoints.LoginWithGGAcessToken,
           Description = AuthDescriptions.LoginWithGGAcessToken
       )]
        //[CustomGoogleIdTokenAuthFilter]
        [HttpPost("Login/Google/Access-Token")]
        public async Task<IActionResult> LoginWithGoogleAccessTokenAsync(string? accessToken, bool rememberMe = true)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (accessToken is null || accessToken.Length == 0)
                {
                    accessToken = HttpContext.GetGoogleAccessToken();
                }
                //var userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
                //var hc = new HttpClient();
                //hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //var response = hc.GetAsync(userInfoUrl).Result;
                //if (!response.IsSuccessStatusCode)
                //{
                //    valResult.Add(JsonConvert.SerializeObject(response, Formatting.Indented));
                //    return BadRequest(valResult);
                //}
                //var userInfoString = response.Content.ReadAsStringAsync().Result;
                //GoogleUser userInfo = JsonConvert.DeserializeObject<GoogleUser>(userInfoString);
                //Account logined = await services.Accounts.GetAccountByEmailAsync(userInfo.Email);
                LoginInfoDto logined = await services.Auth.LoginWithGoogleAccessToken(accessToken, rememberMe);
                if (logined is null)
                {
                    valResult.Add("Bạn chưa đăng kí tài khoản");
                    return Unauthorized(valResult);
                }
                //return Ok(await services.Auth.GenerateJwtAsync(logined, rememberMe));
                //string token = await services.Auth.GenerateJwtAsync(logined, rememberMe);
                //return base.Ok(new { token = token, Id = logined.Id, Username = logined.Username, Email = logined.Email, Role = logined.Role.Name });
                return Ok(logined);
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }
        //public partial class GoogleUser
        //{
        //    [JsonProperty("id")]
        //    public string Id { get; set; }

        //    [JsonProperty("email")]
        //    public string Email { get; set; }

        //    [JsonProperty("verified_email")]
        //    public bool VerifiedEmail { get; set; }

        //    [JsonProperty("name")]
        //    public string Name { get; set; }

        //    [JsonProperty("given_name")]
        //    public string GivenName { get; set; }

        //    [JsonProperty("family_name")]
        //    public string FamilyName { get; set; }

        //    [JsonProperty("picture")]
        //    public Uri Picture { get; set; }

        //    [JsonProperty("locale")]
        //    public string Locale { get; set; }

        //}
        [SwaggerOperation(
        Summary = AuthEndpoints.StudentRegister,
                Description = AuthDescriptions.StudentRegister
            )]
        [HttpPost("Register/Student")]
        public async Task<IActionResult> StudentRegister(StudentRegisterDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                await valResult.ValidateParams(services, dto);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                var path = "Images/default.jpg"; // Path to image

                // Initialize Firebase storage
                var storage = new FirebaseStorage(firebaseBucket);

                // Get the download URL for the image
                string downloadUrl = await storage.Child(path).GetDownloadUrlAsync();

                //Account register = mapper.Map<Account>(dto);
                Account register = new Account()
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password,
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    DateOfBirth = dto.DateOfBirth,
                    ImagePath = downloadUrl,
                };
                register.Password = register.Password.CustomHash();
                if (dto.IsStudent)
                {
                    await services.Auth.Register(register, RoleNameEnum.Student);
                }
                else
                {
                    await services.Auth.Register(register, RoleNameEnum.Parent);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
        Summary = AuthEndpoints.ParentRegister,
            Description = AuthDescriptions.ParentRegister
        )]
        [HttpPost("Register/Parent")]
        public async Task<IActionResult> ParentRegister(ParentRegisterDto dto)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                await valResult.ValidateParams(services, dto);
                if (!valResult.IsValid)
                {
                    return BadRequest(valResult);
                }
                //Account register = mapper.Map<Account>(dto);
                Account register = new Account()
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = dto.Password,
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    DateOfBirth = dto.DateOfBirth,
                };
                await services.Auth.Register(register, RoleNameEnum.Student);
                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        [SwaggerOperation(
           Summary = AuthEndpoints.StudentRegisterWithGgAccessToken,
           Description = AuthDescriptions.StudentRegisterWithGgAccessToken
       )]
        [HttpPost("Register/Google")]
        public async Task<IActionResult> StudentRegisterWithGgAccessToken(string? accessToken, bool isStudent = true)
        {
            ValidatorResult valResult = new ValidatorResult();
            try
            {
                if (accessToken is null || accessToken.Length == 0)
                {
                    accessToken = HttpContext.GetGoogleAccessToken();
                }
                var userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
                var hc = new HttpClient();
                hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = hc.GetAsync(userInfoUrl).Result;
                var userInfoString = response.Content.ReadAsStringAsync().Result;
                GoogleUser userInfo = JsonConvert.DeserializeObject<GoogleUser>(userInfoString);
                Account existed = await services.Accounts.GetAccountByEmailAsync(userInfo.Email);
                if (existed is not null)
                {
                    valResult.Add("Bạn đã đăng kí tài khoản rồi");
                    return Unauthorized(valResult);
                }
                Account register = new Account()
                {
                    Username = userInfo.Email,
                    Email = userInfo.Email,
                    Password = PasswordUtil.RandomPassword(9),
                    FullName = userInfo.Name,

                };
                await services.Auth.Register(register, isStudent ? RoleNameEnum.Student : RoleNameEnum.Parent);

                return Ok();
            }
            catch (Exception ex)
            {
                valResult.Add(ex.ToString());
                return BadRequest(valResult);
            }
        }

        //[SwaggerOperation(
        //    Summary = AuthEndpoints.ParentRegisterWithGgAccessToken,
        //    Description = AuthDescriptions.ParentRegisterWithGgAccessToken
        //)]
        //[HttpPost("Register/Parent/Google")]
        //public async Task<IActionResult> ParentRegisterWithGgAccessToken()
        //{
        //    //if (dto.Password != dto.ConfirmPassword)
        //    //{
        //    //    return BadRequest("Xác nhận password không thành công");
        //    //}
        //    //Account register = mapper.Map<Account>(dto);
        //    //await services.Auth.Register(register, RoleNameEnum.Student);
        //    //return Ok(await services.Accounts.GetAccountByUserNameAsync(dto.Username));
        //    return BadRequest();
        //}
        /// ////////////////////////////////////////////////////////////////////////////////////////
        [Tags(Actor.Test)]
        [SwaggerOperation(
           Summary = AuthEndpoints.GetTokens
       )]
        [HttpGet("TestAuth/Tokens")]
        public async Task<IActionResult> GetTokens()
        {
            var accerssToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var headerToken = HttpContext.Request.Headers.Where(x => x.Key == "Authorization").Select(x => x.Value);
            return Ok(new
            {
                Access_Token = accerssToken,
                Id_Token = idToken,
                Header_Token = headerToken,
            });
        }

        [Tags(Actor.Test)]
        [SwaggerOperation(
          Summary = AuthEndpoints.GetRoleData
      )]
        [Authorize]
        [HttpGet("TestAuth/LoginData")]
        public async Task<IActionResult> GetRoleData()
        {
            string? roles1 = String.Empty;
            foreach (var roleClaim in User.Claims.Where(role => role.Type.Contains("role", StringComparison.CurrentCultureIgnoreCase)))
            {
                roles1 += $" {roleClaim.Value}";
            }
            string emails = String.Empty;
            IEnumerable<Claim> claims = User.Claims;
            IEnumerable<Claim> emailClaims = claims.Where<Claim>(role => role.Type.Contains("mail", StringComparison.CurrentCultureIgnoreCase));
            foreach (var emailClaim in emailClaims)
            {
                emails += $" {emailClaim.Value}";
            }
            return Ok(new
            {
                roles = string.IsNullOrEmpty(roles1) ? "No role" : roles1,
                emails = string.IsNullOrEmpty(emails) ? "No mail" : emails
            });
        }

        [Tags(Actor.Test)]
        [SwaggerOperation(
         Summary = AuthEndpoints.GetStudentData
        )]
        [Authorize(Roles = "Student")]
        [HttpGet("TestAuth/Student")]
        public async Task<IActionResult> GetStudentData()
        {
            return Ok("You're student ");
        }

        [Tags(Actor.Test)]
        [SwaggerOperation(
         Summary = AuthEndpoints.GetUserData
        )]
        [Authorize(Roles = "Parent")]
        [HttpGet("TestAuth/Parent")]
        public async Task<IActionResult> GetUserData()
        {
            return Ok("You're a parent ");
        }
    }
}
