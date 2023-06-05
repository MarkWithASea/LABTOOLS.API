using LABTOOLS.API.Data;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.JsonApi;
//using LABTOOLS.API.Middleware;
using LABTOOLS.API.Models;
using LABTOOLS.API.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager($"/{builder.Environment.EnvironmentName}/Labtools-API/AppSettings", TimeSpan.FromMinutes(5));

// configure strongly typed settings object
var appSettings = new AppSettings(builder.Configuration);

// named policy https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-6.0#cors-with-named-policy-and-middleware
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins(appSettings.AllowedOrigins!.Split(","))
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .AllowCredentials();
                      });
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(appSettings.ConnectionString));

// Add services to the container.
builder.Services.AddControllers();

// configure jwt authentication
var cognitoIssuer = $"https://cognito-idp.{appSettings.Region}.amazonaws.com/{appSettings.UserPoolId}";
var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
var cognitoAudience = appSettings.AppClientId;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
        {
            // get JsonWebKeySet from AWS
            string json = new HttpClient().GetStringAsync(jwtKeySetUrl).Result;

            // serialize the result
            IList<JsonWebKey> keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json)!.Keys;

            // cast the result to be the type expected by IssuerSigningKeyResolver
            return (IEnumerable<SecurityKey>)keys;
        },
        ValidIssuer = cognitoIssuer,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
    };
});

// Add Automapper. Mappings configured in Profiles
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// configure DI for application builder.Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LinkBuilder>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LABTOOLS.API", Version = "v1" });
});

var app = builder.Build();

//app.UseMiddleware<RequestBodyBufferingMiddleware>();
//app.UseMiddleware<ResponseRewindMiddleware>();

ServiceActivator.Configure(app.Services);

//app.SeedData(appSettings);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LABSIGN.API v1"));
}

//app.UseHttpsRedirection();
app.UseRouting();

// global cors policy
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
