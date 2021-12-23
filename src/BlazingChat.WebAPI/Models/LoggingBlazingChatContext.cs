using Microsoft.EntityFrameworkCore;

namespace BlazingChat.WebAPI.Models
{
    public partial class LoggingBlazingChatContext : DbContext
    {
        public LoggingBlazingChatContext()
        {
        }

        public LoggingBlazingChatContext(DbContextOptions<LoggingBlazingChatContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.LogId).HasColumnName("log_id");

                entity.Property(e => e.CreatedDate).HasColumnName("created_date");

                entity.Property(e => e.ExceptionMessage).HasColumnName("exception_message");

                entity.Property(e => e.LogLevel).HasColumnName("log_level");

                entity.Property(e => e.Source).HasColumnName("source");

                entity.Property(e => e.StackTrace).HasColumnName("stack_trace");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
