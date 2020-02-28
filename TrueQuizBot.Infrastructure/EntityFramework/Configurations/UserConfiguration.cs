using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrueQuizBot.Infrastructure.EntityFramework.Configurations
{
    [UsedImplicitly]
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User), TrueQuizBotDbContext.DefaultSchemaName);
            builder.HasKey(x => x.UserId);

            builder.HasMany(x => x.AnswerStatistics)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TrueLuckyPersonalData)
                .WithOne(a => a!.User)
                .HasForeignKey<PersonalData>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}