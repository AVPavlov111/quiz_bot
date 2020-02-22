using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TrueQuizBot.Infrastructure.EntityFramework;

namespace TrueQuizBot.Infrastructure
{
    public class TrueQuizBotDbContextFactory : IDesignTimeDbContextFactory<TrueQuizBotDbContext>
    {
        private const string DefaultConnectionString =
            "Data Source=127.0.0.1;Initial Catalog=TrueQuizBot;User Id=sa; Password=2wsx2WSX;";
        public const string DefaultSchemaName = "TQB";
        public static DbContextOptions<TrueQuizBotDbContext> GetSqlServerOptions([CanBeNull]string connectionString)
        {
            return new DbContextOptionsBuilder<TrueQuizBotDbContext>()
                .UseSqlServer(connectionString ?? DefaultConnectionString, x =>
                {
                    //x.MigrationsAssembly("TrueQuizBot.WebApi");
                    x.MigrationsHistoryTable("__EFMigrationsHistory", TrueQuizBotDbContext.DefaultSchemaName);
                })
                .Options;
        }

        public TrueQuizBotDbContext CreateDbContext(string[] args)
        {
           return new TrueQuizBotDbContext(GetSqlServerOptions(null));
        }
    }
}