using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class TrueQuizBotDbContext : DbContext
    {
        public TrueQuizBotDbContext(DbContextOptions options) : base(options)
        {
        }

        public const string DefaultSchemaName = "TQB";
        
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.HasDefaultSchema(DefaultSchemaName);
        }
        
        public User AddUser(User user)
        {
            Users.Add(user);
            base.SaveChanges();
            return user;
        }
        
        public User GetUser(string userId)
        {
            return Users.Single(u => u.UserId == userId)
                   // ReSharper disable once ConstantNullCoalescingCondition
                   ?? AddUser(new User(userId));
        }
    }
}