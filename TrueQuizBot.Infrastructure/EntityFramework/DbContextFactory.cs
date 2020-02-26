using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class DbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public TrueQuizBotDbContext GetContext()
        {
            return _serviceProvider.GetRequiredService<TrueQuizBotDbContext>();
        }
        
        public async Task RunInTransaction(Func<TrueQuizBotDbContext, Task> func)
        {
            await using var database = GetContext();
            database.BeginTransaction(IsolationLevel.ReadCommitted);
            await func(database);
        }
        
        public async Task<TResponse> RunInTransaction<TResponse>(Func<TrueQuizBotDbContext, Task<TResponse>> func)
        {
            await using var database = GetContext();
            database.BeginTransaction(IsolationLevel.ReadCommitted);
            var result = await func(database);
            return result;
        }
    }
}