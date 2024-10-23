using HealthcareHospitalManagementSystem.Services;
using HealthcareHospitalManagementSystem.Infrastructure;
using Healthcare_Hospital_Management_System.Infrastructure;
using Xkcd;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICustomLogger, Logger>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IXkcdClient, XkcdClient>();
builder.Services.AddHttpClient<DrugClient>();

builder.Services.AddControllers();

builder.Services.AddAuthorization();

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