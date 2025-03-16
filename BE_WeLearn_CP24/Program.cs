using API.Extension.Auth;
using API.SignalRHub;
using DataLayer.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RepoLayer.Implemention;
using RepoLayer.Interface;
using ServiceLayer.DbSeeding;
using ServiceLayer.Services.Implementation;
using ServiceLayer.Services.Interface;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;
bool IsInMemory = configuration["ConnectionStrings:InMemory"].ToLower() == "true";

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtAuthService(configuration);
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
    options.AddJwtAuthUi(configuration);
});
#region dbContext
builder.Services.AddDbContext<WeLearnContext>(options =>
{
    options.EnableSensitiveDataLogging();
    //options.EnableRetryOnFailure();
    //options.EnableRetryOnFailure
    if (IsInMemory)
    {
        options.UseInMemoryDatabase("WeLearn");
    }
    else
    {
        Console.WriteLine(configuration.GetConnectionString("Default"));
        options.UseSqlServer(configuration.GetConnectionString("Default"), o =>
        {
           o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    }
});
#endregion
#region service and repo
//builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddScoped<IRepoWrapper, RepoWrapper>();
builder.Services.AddScoped<IServiceWrapper, ServiceWrapper>();
//builder.Services.AddScoped<IAutoMailService, AutoMailService>();
#endregion
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#region AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion
#region validator
//builder.Services.AddScoped<IValidatorWrapper, ValidatorWrapper>();
#endregion
#region cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("allcors", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(hostName => true));

});
#endregion
builder.Services.AddSignalR();

var app = builder.Build();
if (IsInMemory)
{
    Console.WriteLine("++++++++++================+++++++++++++++InMemory++++++++++++=============++++++++++");
    //app.SeedInMemoryDb();
}
app.SeedInMemoryDb(IsInMemory);
// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseCors("allcors");
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.OAuthClientId(configuration["Authentication:Google:Web:client_id"]);
    options.InjectJavascript("/googleSwaggerUi.js");

});

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GroupHub>("hubs/grouphub");
app.MapHub<DrawHub>("hubs/drawhub");
app.MapHub<MeetingHub>("hubs/meetinghub");

app.MapControllers();

//using (StringWriter stringWriter = new StringWriter())
//{
//    Console.SetOut(stringWriter);


//    string allConsoleOutput = stringWriter.ToString();
//}

app.Run();
