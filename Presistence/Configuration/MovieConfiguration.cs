using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Configuration
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(movie => movie.Id);

            builder.Property(movie => movie.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(movie => movie.Description)
                .HasColumnName("Description")
                .IsRequired();

            builder.Property(movie => movie.LikeCount)
                .HasColumnName("LikeCount")
                .IsRequired();
            builder.Property(movie => movie.Rating)
              .HasColumnName("Rating")
              .IsRequired(false);

            builder.Property(movie => movie.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2")
                .IsRequired();

            builder.Property(movie => movie.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2")
                .IsRequired(false);

            builder.Property(movie => movie.CreatedBy)
                .HasColumnName("CreatedBy")
                .IsRequired(false);

            builder.Property(movie => movie.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .IsRequired(false);

            builder.Property(movie => movie.Video)
                .HasColumnName("VideoUrl")
                .IsRequired(false);

            builder.Property(movie => movie.VideoPublicId)
                .HasColumnName("Video_Public_Id")
                .IsRequired(false);

            builder.Property(movie => movie.Category)
                .HasColumnName("Category")
                .IsRequired();

            builder.Property(movie => movie.LikeCount)
                .HasColumnName("LikeCount")
                .IsRequired(false);

            builder.Property(movie => movie.ProducentId)
                .HasColumnName("ProducentId")
                .IsRequired();

            Relationships(builder);
            builder.ToTable("Movies");
        }

        public void Relationships(EntityTypeBuilder<Movie> builder)
        {
            builder.HasMany(movie => movie.ActorMovies)
                .WithOne(actorMovie => actorMovie.Movie)
                .HasForeignKey(actorMovie => actorMovie.MovieId);

            builder.HasOne(movie => movie.Producent)
                .WithMany(producent => producent.Movies)
                .HasForeignKey(movie => movie.ProducentId);
        }
    }
}
