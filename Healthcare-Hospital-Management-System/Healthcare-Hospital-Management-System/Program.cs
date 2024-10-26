using Healthcare_Hospital_Management_System.Infrastructure;
using Healthcare_Hospital_Management_System.Models;
using Healthcare_Hospital_Management_System.Services;
using HealthcareHospitalManagementSystem.Infrastructure;
using HealthcareHospitalManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HealthcareDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDrugService, DrugService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICustomLogger, Logger>();

builder.Services.AddHttpClient();
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