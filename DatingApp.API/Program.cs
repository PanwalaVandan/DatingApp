using System;
using DatingApp.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DatingApp.API
{
    public class Program
    {
        // public static void Main(string[] args)
        // {
        //     BuildWebHost(args).Run();
        // }

        // public static IWebHost BuildWebHost(string[] args) =>
        //     WebHost.CreateDefaultBuilder(args)
        //         .UseStartup<Startup>()
        //         .Build();

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    var context = services.GetRequiredService<DataContext>();
                    // applies any pending migrations to the database. Creates the DB if not created
                    context.Database.Migrate();
                    Seed.SeedUsers(context);
                }catch(Exception ex) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
