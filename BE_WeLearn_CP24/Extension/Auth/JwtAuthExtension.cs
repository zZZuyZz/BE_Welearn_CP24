using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace API.Extension.Auth
{
    public static class JwtAuthExtension
    {
        private const string SecurityId = "Jwt Bearer";

        public static IServiceCollection AddJwtAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                #region google id token
                //o.IncludeErrorDetails = true;
                //o.SecurityTokenValidators.Clear();
                //o.SecurityTokenValidators.Add(new GoogleTokenValidator());
                #endregion
                #region 2 phase auth
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    // Remember to set to true on production
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Authentication:JwtToken:TokenKey"])),
                    ClockSkew = TimeSpan.Zero,
                    //ValidIssuer = configuration["Authentication:JwtToken:Issuer"],
                    ValidAudience = configuration["Authentication:JwtToken:Audience"]
                };
                #region auth event (removed)
                //o.Events = new JwtBearerEvents
                //{
                //    OnChallenge = async context =>
                //    {
                //        // Call this to skip the default logic and avoid using the default response
                //        context.HandleResponse();

                //        var httpContext = context.HttpContext;
                //        const int statusCode = StatusCodes.Status401Unauthorized;

                //        var routeData = httpContext.GetRouteData();
                //        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

                //        var factory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                //        var problemDetails = factory.CreateProblemDetails(httpContext, statusCode);

                //        var result = new ObjectResult(problemDetails) { StatusCode = statusCode };
                //        await result.ExecuteResultAsync(actionContext);
                //    }
                //};
                #endregion
                #endregion
            })
            //.AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = configuration["Authentication:Google:Web:client_id"];
            //    googleOptions.ClientSecret = configuration["Authentication:Google:Web:client_secret"];
            //}); 
            .AddIdTokenBasedGoogle(configuration);
            return services;
        }
        private static AuthenticationBuilder AddIdTokenBasedGoogle(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            return builder.AddGoogle(googleOptions =>
             {
                 googleOptions.ClientId = configuration["Authentication:Google:Web:client_id"];
                 googleOptions.ClientSecret = configuration["Authentication:Google:Web:client_secret"];
             });
        }
        public static SwaggerGenOptions AddJwtAuthUi(this SwaggerGenOptions options, IConfiguration configuration)
        {
            options.AddSecurityDefinition(SecurityId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,  //ko xài .ApiKey vì .http sẽ ko cần gõ
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization using the Bearer scheme. \"Bearer\" is not needed.Just paste the jwt"
            }
                );
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SecurityId
                            }
                        },
                        new string[]{}
                    }
                }
            );
            //options.AddSecurityDefinition("GoogleOAuth2", new OpenApiSecurityScheme
            //{
            //    Name = "Google Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.OAuth2,
            //    Description = "Google Authorization",
            //    Flows = new OpenApiOAuthFlows
            //    {
            //        Implicit = new OpenApiOAuthFlow
            //        {
            //            AuthorizationUrl = new Uri(configuration["Authentication:Google:Web:auth_uri"]/*, UriKind.Relative*/),
            //            Scopes = new Dictionary<string, string>
            //                {
            //                    { "openid", "Allow this app to get some basic account info" },
            //                    //{ "writeAccess", "Access write operations" }
            //                }
            //        }
            //    }
            //});
            //options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = "googleOAuth2"
            //            }
            //        },
            //        new string[]{
            //            "openid", 
            //            //"WriteAccess" 
            //        }
            //    }
            //});
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(configuration["Authentication:Google:Web:auth_uri"]/*"https://accounts.google.com/o/oauth2/v2/auth"*/),
                        Scopes = new Dictionary<string, string> {
                    { "openid", "Allow this app to get some basic account info" },
                    { "email", "email" },
                    { "profile", "profile" }
                },

                        TokenUrl = new Uri(configuration["Authentication:Google:Web:token_uri"])
                    }
                },
                Extensions = new Dictionary<string, IOpenApiExtension>
        {
            {"x-tokenName", new OpenApiString("id_token")}
        },
            };

            options.AddSecurityDefinition("abc", securityScheme);

            var securityRequirements = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "abc"
                }
            },
            new List<string> {"openid", "email", "profile"}
        }
    };

            options.AddSecurityRequirement(securityRequirements);
            return options;
        }
    }
}
