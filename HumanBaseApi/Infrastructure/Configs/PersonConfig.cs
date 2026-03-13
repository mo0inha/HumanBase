using Domain.Entities;
using Infrastructure.Shared.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs
{
    public class PersonConfig : BaseConfig<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasColumnName("DS_NAME")
                .HasMaxLength(400)
                .IsRequired()
                ;

            builder.Property(x => x.BirthDate)
                .HasColumnName("DT_BIRTHDAY")
                .IsRequired();
        }
    }
}
