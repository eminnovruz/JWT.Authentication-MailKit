using WebApi.HelperServices.Abstract;
using WebApi.HelperServices;
using WebApi.Services;
using WebApi.Services.Abstract;
using WebApi.Configuration.MongoDb;
using System.Configuration;
using WebApi.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Configuration.JWT;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDb"));

builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPassHashService, PassHashService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.Configure<JwtConfiguration>(jwtSettings);

var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();