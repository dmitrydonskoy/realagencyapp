using Microsoft.EntityFrameworkCore;
using RealAgencyModels;
using RealAgencyModels.BusinessLogic;
using System.Windows.Forms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<AreaInfoService>();
builder.Services.AddScoped<BidService>();
builder.Services.AddScoped<CooperationService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<RealStateService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContext<RealAgencyDBContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
