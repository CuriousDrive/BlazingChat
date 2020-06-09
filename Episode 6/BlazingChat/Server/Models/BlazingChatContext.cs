using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazingChat.Server.Models
{
    public partial class BlazingChatContext : DbContext
    {
        public BlazingChatContext()
        {
        }

        public BlazingChatContext(DbContextOptions<BlazingChatContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Name=BlazingChat");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.AboutMe).HasColumnName("about_me");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("DATE");

                entity.Property(e => e.DarkTheme).HasColumnName("dark_theme");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnName("date_of_birth")
                    .HasColumnType("DATETIME");

                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasColumnName("email_address");

                entity.Property(e => e.FirstName).HasColumnName("first_name");

                entity.Property(e => e.LastName).HasColumnName("last_name");

                entity.Property(e => e.Notifications).HasColumnName("notifications");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.ProfilePictureUrl).HasColumnName("profile_picture_url");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasColumnName("source");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
