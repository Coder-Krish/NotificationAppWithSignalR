using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NotificationWithSignalR.DataAccess;
using NotificationWithSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", config => {         
          config.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()  
                .SetIsOriginAllowed(origin => true);
                
            })); 

builder.Services.AddSignalR();
 var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(connectionString));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<MyDbContext>();

        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }

    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CorsPolicy");  
app.MapControllers();
// app.MapHub<BroadcastHub>("/notify");
app.MapHub<EmployeeHub>("/employee");
app.MapHub<NotificationHub>("/notify");

// app.UseSpa(spa =>
// {   
    
//     spa.Options.SourcePath = "Client/dist/";
//      if (app.Environment.IsDevelopment())
//                 {
//                     spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
//                 }
// });

app.Run();
