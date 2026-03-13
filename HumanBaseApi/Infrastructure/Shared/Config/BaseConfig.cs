using Domain.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Shared.Config
{
    public class BaseConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            ConfigureKey(builder);

            builder.Property(x => x.Id)
               .IsRequired()
               .HasColumnName("ID")
               .ValueGeneratedOnAdd()
               .IsConcurrencyToken();

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnName("CREATED_AT");

            builder.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasColumnName("UPDATED_AT");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasColumnName("ST_ACTIVE");

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasColumnName("ST_IS_DELETED");
        }

        public virtual void ConfigureKey(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
