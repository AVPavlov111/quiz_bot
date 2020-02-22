using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrueQuizBot.Infrastructure.EntityFramework.Configurations
{
    public class PersonalDataConfiguration : IEntityTypeConfiguration<PersonalData>
    {
        public void Configure(EntityTypeBuilder<PersonalData> builder)
        {
            builder.HasKey(x => x.PhoneNumber);
        }
    }
}