using Microsoft.EntityFrameworkCore;
using WorkoutLog.Models;

namespace WorkoutLog.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Workout> Workouts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы
            modelBuilder.Entity<Workout>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Exercise).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(500);

                // Индекс для быстрого поиска по дате
                entity.HasIndex(e => e.Date);

                // Индекс для быстрого поиска по типу
                entity.HasIndex(e => e.Type);
            });
        }
    }
}