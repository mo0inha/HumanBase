using Domain.Entities;
using Domain.Entities.EType;
using Infrastructure.Shared.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configs
{
    public class TransitionConfig : BaseConfig<AppTransaction>
    {
        public override void Configure(EntityTypeBuilder<AppTransaction> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Description)
                .HasColumnName("DS_DESCRIPTION")
                .HasMaxLength(200)
                .IsRequired()
                ;

            builder.Property(x => x.Value)
                .HasColumnName("NR_VALUE")
                .IsRequired();

            builder
              .Property(x => x.TypeFinancial)
              .HasColumnName("ST_TYPE_FINANCIAL")
              .HasConversion(x => (byte)x, x => (ETypeFinancial)x)
              .IsRequired();

            builder.Property(x => x.CategoryId)
                .HasColumnName("ID_CATEGORY")
                .IsRequired();

            builder.Property(x => x.PersonId)
                .HasColumnName("ID_PERSON")
                .IsRequired();
        }
    }
}
