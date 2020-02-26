using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class TrueQuizBotDbContext : DbContext
    {
        public TrueQuizBotDbContext(DbContextOptions options) : base(options)
        {
        }

        public const string DefaultSchemaName = "TQB";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.HasDefaultSchema(DefaultSchemaName);
        }
        
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            Database.BeginTransaction(isolationLevel);
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync();
            Database.CurrentTransaction.Commit();
        }

        public async Task<User?> FindUser(string userId)
        {
            return await Set<User?>()
                .Include(u => u!.AnswerStatistics)
                .Include(u => u!.PersonalData)
                .SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUser(string userId)
        {
            return await Set<User>()
                .Include(u => u.AnswerStatistics)
                .Include(u => u.PersonalData)
                .SingleAsync(u => u.UserId == userId);
        }
    }
}