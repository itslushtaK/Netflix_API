using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Configuration
{
    public class ProducentConfiguration : IEntityTypeConfiguration<Producent>
    {
        public void Configure(EntityTypeBuilder<Producent> builder)
        {
            builder.HasKey(producent => producent.Id);

            builder.Property(producent => producent.FirstName)
                .HasColumnName("FirstName")
                .IsRequired();

            builder.Property(producent => producent.LastName)
                .HasColumnName("LastName")
                .IsRequired();

            builder.Property(producent => producent.Photo)
                .HasColumnName("Photo")
                .IsRequired(false);

            builder.Property(producent => producent.Country)
                .HasColumnName("Country")
                .IsRequired(false);

            builder.Property(producent => producent.CreatedAt)
                .HasColumnName("Created At")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(producent => producent.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(producent => producent.CreatedBy)
                .HasColumnName("CreatedBy")
                .IsRequired(false);

            builder.Property(producent => producent.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .IsRequired(false);

            Relationship(builder);
            builder.ToTable("Producents");
        }

        public void Relationship(EntityTypeBuilder<Producent> builder)
        {
            builder.HasMany(producent => producent.Movies)
                .WithOne(movie => movie.Producent)
                .HasForeignKey(movie => movie.ProducentId);
        }
    }
}
