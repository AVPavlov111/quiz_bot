using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrueQuizBot.Infrastructure.EntityFramework.Configurations
{
    public class TrueLuckyPersonalDataConfiguration : IEntityTypeConfiguration<TrueLuckyPersonalData>
    {
        public void Configure(EntityTypeBuilder<TrueLuckyPersonalData> builder)
        {
            builder.ToTable(nameof(TrueLuckyPersonalData), TrueQuizBotDbContext.DefaultSchemaName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.DisplayName).HasMaxLength(256);
            builder.Property(x => x.PhoneNumber).HasMaxLength(16);
            builder.Property(x => x.CompanyName).HasMaxLength(128);
            builder.Property(x => x.Position).HasMaxLength(128);
            builder.Property(x => x.Interests).HasMaxLength(4000);
        }
    }
}