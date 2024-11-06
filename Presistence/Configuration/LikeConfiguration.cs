using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Presistence.Configuration
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(like => new { like.MovieId, like.UserId });

            builder.Property(like => like.MovieId)
                .HasColumnName("Movie_Id")
                .IsRequired();

            builder.Property(like => like.UserId)
                .HasColumnName("User_Id")
                .HasColumnType("nvarchar(450)")
                .IsRequired();

            Relationships(builder);
            builder.ToTable("LikeMovie");
        }

        public void Relationships(EntityTypeBuilder<Like> builder)
        {
            builder.HasOne(like => like.Movie)
                .WithMany(movie => movie.Likes)
                .HasForeignKey(like => like.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
