using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class TrueQuizBotDbContext : DbContext
    {
        private const string DefaultSchemaName = "TQB";
        
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.HasDefaultSchema(DefaultSchemaName);
        }
        
        public User AddUser(User user)
        {
            Users.Add(user);
            base.SaveChangesAsync();
            return user;
        }
        
        public User GetUser(string userId)
        {
            return Users.FirstOrDefault(u => string.Equals(u.UserId, userId, StringComparison.OrdinalIgnoreCase))
                   // ReSharper disable once ConstantNullCoalescingCondition
                   ?? AddUser(new User(userId));
        }
    }
}