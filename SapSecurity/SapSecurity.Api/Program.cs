using SapSecurity.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString =
    "Data Source=localhost;Initial Catalog=SapSecurity_Db;User Id=Sa;password=rasoul3744;Trusted_Connection=false;MultipleActiveResultSets=true;Encrypt=False;";

builder.Services.SetupServices(connectionString);


builder.Services.AddCors(option =>
{
    option.AddPolicy("MyPolicy", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
        policy.SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();




//var connectionHub = app.Services.GetRequiredService<IConnectionHub>();
//connectionHub.Setup();
//connectionHub.RunRegisterUserSocketAsync();
//connectionHub.RunRegisterSensorLogSocketUdpAsync();


app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();

app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();


app.Run();