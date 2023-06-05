using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using trempApplication.Properties.Interfaces;
using trempApplication.Properties.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions( options =>
{ 
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
}); ;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//new


builder.Services.AddScoped<IPassenger, PassengerService>();
builder.Services.AddScoped<ICar, CarService>();
builder.Services.AddScoped<IRide, RideService>();
builder.Services.AddScoped<IUser, UserService>();



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
