//using Google.Apis.Auth;
//using Google.Apis.Oauth2.v2;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using DataLayer.DbObject;
using DataLayer.Enums;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Newtonsoft.Json;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Auth;
using ServiceLayer.Services.Interface.Mail;
using ServiceLayer.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ServiceLayer.Services.Implementation.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IRepoWrapper repos;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private JwtSecurityTokenHandler jwtHandler;
        private readonly IAutoMailService mailService;

        public AuthService(IRepoWrapper repos, IConfiguration configuration, IMapper mapper, IAutoMailService mailService)
        {
            this.repos = repos;
            this.configuration = configuration;
            jwtHandler = new JwtSecurityTokenHandler();
            this.mapper = mapper;
            this.mailService = mailService;
        }

        public async Task<LoginInfoDto> LoginAsync(LoginModel login)
        {
            if (login.UsernameOrEmail == configuration["Admin:Username"])
            {
                if (login.Password == configuration["Admin:Password"].CustomHash())
                {
                    LoginInfoDto loginInfoDto = new LoginInfoDto()
                    {
                        Username= login.UsernameOrEmail ,
                        Email= login.UsernameOrEmail,
                        FullName = login.UsernameOrEmail ,
                        RoleName = "Admin",
                        LeadGroups= new List<GroupGetListDto>(), 
                        JoinGroups= new List<GroupGetListDto>(),
                    };
                    loginInfoDto.Token = await GenerateAdminJwtAsync(loginInfoDto);
                    return loginInfoDto;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Account account = await repos.Accounts.GetByUsernameOrEmailAndPasswordAsync(login.UsernameOrEmail, login.Password);
                if (account == null)
                {
                    return null;
                }
                LoginInfoDto loginInfoDto = mapper.Map<LoginInfoDto>(account);
                loginInfoDto.Token = await GenerateJwtAsync(account, login.RememberMe);
                return loginInfoDto;
            }
        }

        public async Task<LoginInfoDto> LoginWithGoogleIdToken(string googleIdToken, bool rememberMe)
        {
            //JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(googleIdToken))
            {
                throw new Exception("Invalidated googleIdToken");
            }


            var payload = GoogleJsonWebSignature.ValidateAsync(googleIdToken, new GoogleJsonWebSignature.ValidationSettings()).Result;
            //return repos.Accounts.GetByUsernameAsync(payload.Email);
            Account account = await repos.Accounts.GetByEmailAsync(payload.Email);
            if (account == null)
            {
                account = new Account()
               {
                    Email = payload.Email,
                    Username = payload.Email,
                    FullName = payload.GivenName,
                    IsBanned = false,
                    DateOfBirth = DateTime.UtcNow.AddYears(-18),
                    Password = "123456789".CustomHash() ,
                    ImagePath = payload.Picture,
                    Phone = "0000000000",
                    RoleId = (int)RoleNameEnum.Student
               };
                await repos.Accounts.CreateAsync(account);
                await mailService.SendNewPasswordMailAsync(payload.Email);
            }
            LoginInfoDto loginInfoDto = mapper.Map<LoginInfoDto>(account);
            loginInfoDto.Token = await GenerateJwtAsync(account, rememberMe);
            return loginInfoDto;
        }

        public async Task<LoginInfoDto> LoginWithGoogleAccessToken(string googleAccessToken, bool rememberMe)
        {
            var userInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo";
            var hc = new HttpClient();
            hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", googleAccessToken);
            var response = hc.GetAsync(userInfoUrl).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw (new Exception(JsonConvert.SerializeObject(response, Formatting.Indented)));
                
            }
            var userInfoString = response.Content.ReadAsStringAsync().Result;
            GoogleUser userInfo = JsonConvert.DeserializeObject<GoogleUser>(userInfoString);
            Account account = await repos.Accounts.GetByEmailAsync(userInfo.Email);
            if (account == null)
            {
                return null;
            }
            LoginInfoDto loginInfoDto = mapper.Map<LoginInfoDto>(account);
            loginInfoDto.Token = await GenerateJwtAsync(account, rememberMe);
            return loginInfoDto;
        }

        private async Task<string> GenerateJwtAsync(Account logined, bool rememberMe)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, logined.Id.ToString()),
                new Claim(ClaimTypes.Name, logined.Username),
                new Claim(ClaimTypes.Email, logined.Email),
                new Claim(ClaimTypes.Role, "Student"),
                //new Claim(ClaimTypes.Role, logined.Role.Name),
                //new Claim("Email", logined.Email),           Student
                //new Claim("Role", logined.Role.Name)
            };
            var issuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(configuration["Authentication:JwtToken:TokenKey"]));
            var credential = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:JwtToken:Issuer"],
                configuration["Authentication:JwtToken:Audience"],
                claims,
                expires: rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(1),
                signingCredentials: credential
            );
            return jwtHandler.WriteToken(jwtSecurityToken);
        }

        private async Task<string> GenerateAdminJwtAsync(LoginInfoDto logined)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, logined.Id.ToString()),
                new Claim(ClaimTypes.Name, logined.Username),
                new Claim(ClaimTypes.Email, logined.Email),
                new Claim(ClaimTypes.Role, logined.RoleName),
                //new Claim("Email", logined.Email),
                //new Claim("Role", logined.Role.Name)
            };
            var issuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(configuration["Authentication:JwtToken:TokenKey"]));
            var credential = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:JwtToken:Issuer"],
                configuration["Authentication:JwtToken:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential
            );
            return jwtHandler.WriteToken(jwtSecurityToken);
        }

        public async Task Register(Account register, RoleNameEnum role)
        {
            register.RoleId = (int)role;
            await repos.Accounts.CreateAsync(register);
        }
    }
}
