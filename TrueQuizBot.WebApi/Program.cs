using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrueQuizBot.Infrastructure.EntityFramework;

namespace TrueQuizBot.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    using var context = services.GetRequiredService<TrueQuizBotDbContext>();
                    context.Database.Migrate();
                }

                host.Run();
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
