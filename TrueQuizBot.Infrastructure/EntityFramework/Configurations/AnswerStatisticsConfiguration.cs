using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrueQuizBot.Infrastructure.EntityFramework.Configurations
{
    public class AnswerStatisticsConfiguration : IEntityTypeConfiguration<AnswerStatistic>
    {
        public void Configure(EntityTypeBuilder<AnswerStatistic> builder)
        {
            builder.ToTable(nameof(AnswerStatistic), TrueQuizBotDbContext.DefaultSchemaName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}