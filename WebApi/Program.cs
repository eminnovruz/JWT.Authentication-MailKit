using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Use service extensions
builder.Services.ConfigureAppServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp"); // Apply CORS policy

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
