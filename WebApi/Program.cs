using WebApi.HelperServices.Abstract;
using WebApi.HelperServices;
using WebApi.Services;
using WebApi.Services.Abstract;
using WebApi.Configuration.MongoDb;
using System.Configuration;
using WebApi.Context;

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

app.Run();