using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrueQuizBot.Infrastructure.EntityFramework.Configurations
{
    public class PersonalDataConfiguration : IEntityTypeConfiguration<PersonalData>
    {
        public void Configure(EntityTypeBuilder<PersonalData> builder)
        {
            builder.ToTable(nameof(PersonalData), TrueQuizBotDbContext.DefaultSchemaName);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FirstName).HasMaxLength(32);
            builder.Property(x => x.LastName).HasMaxLength(128);
            builder.Property(x => x.Email).HasMaxLength(64);
            builder.Property(x => x.PhoneNumber).HasMaxLength(16);
        }

        
    }
}