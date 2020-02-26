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

        public async Task<User> GetOrCreateUser(string userId)
        {
            var user = await Set<User?>()
                .Include(u => u!.AnswerStatistics)
                .Include(u => u!.PersonalData)
                .SingleOrDefaultAsync(u => u!.UserId == userId);

            if (user != null)
            {
                return user;
            }
            
            user = new User(userId);
            Add(user);
            return user;
        }
    }
}