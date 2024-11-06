using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Configuration
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.HasKey(actor => actor.Id);

            builder.Property(actor => actor.FirstName)
                .HasColumnName("FirstName")
                .IsRequired();

            builder.Property(actor => actor.LastName)
                .HasColumnName("LastName")
                .IsRequired();

            builder.Property(actor => actor.Age)
                .HasColumnName("Age")
                .IsRequired(false);

            builder.Property(actor => actor.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired()
                .HasColumnType("datetime2"); // Use datetime2 for SQL Server compatibility

            builder.Property(actor => actor.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired(false)
                .HasColumnType("datetime2"); // Use datetime2 for SQL Server compatibility

            builder.Property(actor => actor.CreatedBy)
                .HasColumnName("CreatedBy")
                .IsRequired(false);

            builder.Property(actor => actor.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .IsRequired(false);

            builder.Property(actor => actor.Photo)
                .HasColumnName("Photo")
                .IsRequired(false);

            builder.Property(actor => actor.Country)
                .HasColumnName("Country")
                .IsRequired();

            Relationships(builder);

            builder.ToTable("Actor");
        }

        public void Relationships(EntityTypeBuilder<Actor> builder)
        {
            builder.HasMany(actor => actor.ActorMovies)
                .WithOne(actorMovie => actorMovie.Actor)
                .HasForeignKey(actorMovie => actorMovie.ActorId);
        }
    }
}
