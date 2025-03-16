﻿using DataLayer.DbObject;
using DataLayer.Enums;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface.Auth
{
    public interface IAuthService
    {
        /// <summary>
        ///    LoginAsync by username or mail and password
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>jwtToken: String</returns>
        public Task<LoginInfoDto> LoginAsync(LoginModel loginModel);
        /// <summary>
        /// LoginAsync by google
        /// </summary>
        /// <param name="googleIdToken"></param>
        /// <returns>jwtToken: String</returns>
        public Task<LoginInfoDto> LoginWithGoogleIdToken(string googleIdToken, bool rememberMe);
        public Task<LoginInfoDto> LoginWithGoogleAccessToken(string googleAccessToken, bool rememberMe);
        //public Task<string> GenerateJwtAsync(Account logined, bool rememberMe = true);
        public Task Register(Account register, RoleNameEnum role);
    }
}
