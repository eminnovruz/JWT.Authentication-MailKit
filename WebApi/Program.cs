using WebApi.Services;
using WebApi.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<IMailService, MailService>();

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