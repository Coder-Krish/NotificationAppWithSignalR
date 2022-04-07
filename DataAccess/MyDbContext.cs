using Microsoft.EntityFrameworkCore;
using NotificationWithSignalR.Models;

namespace NotificationWithSignalR.DataAccess
{
    public class MyDbContext : DbContext
    {  
        public MyDbContext (DbContextOptions<MyDbContext> options): base(options)  
        {  
        }  
  
        public DbSet<Employee> Employee { get; set; }  
        public DbSet<Notification> Notification { get; set; }  


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("AppDb");
        optionsBuilder.UseSqlServer(connectionString);
    }
    }  
}  