using TestSource.Service;
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IFlightService, FlightService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy => { policy.WithOrigins("http://localhost:5001/api"); });
});

var app = builder.Build();
app.UseCors(myAllowSpecificOrigins);
app.MapControllerRoute(
    name: "default",
    pattern: "api/");

app.Run();