using LABTOOLS.API.Data;
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager($"/{builder.Environment.EnvironmentName}/LabTools-API/AppSettings", TimeSpan.FromMinutes(5));

// configure strongly typed settings object
var appSettings = new AppSettings(builder.Configuration);

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
//builder.Services.AddScoped<LinkBuilder>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.SeedData(appSettings);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
