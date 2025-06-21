using CritiQuest2.Server.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CritiQuest2.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<UserProgression> UserProgressions { get; set; }
        public DbSet<UserStats> UserStats { get; set; }
        public DbSet<Philosopher> Philosophers { get; set; }
        public DbSet<OwnedPhilosopher> OwnedPhilosophers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonProgress> LessonProgress { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<AchievementProgress> AchievementProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User Progression - One to One
            builder.Entity<UserProgression>()
                .HasOne(up => up.User)
                .WithOne(u => u.Progression)
                .HasForeignKey<UserProgression>(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User Stats - One to One  
            builder.Entity<UserStats>()
                .HasOne(us => us.User)
                .WithOne(u => u.Stats)
                .HasForeignKey<UserStats>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Owned Philosophers - Many to Many through OwnedPhilosopher
            builder.Entity<OwnedPhilosopher>()
                .HasOne(op => op.User)
                .WithMany(u => u.PhilosopherCollection)
                .HasForeignKey(op => op.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OwnedPhilosopher>()
                .HasOne(op => op.Philosopher)
                .WithMany(p => p.OwnedBy)
                .HasForeignKey(op => op.PhilosopherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lesson Progress
            builder.Entity<LessonProgress>()
                .HasOne(lp => lp.User)
                .WithMany(u => u.LessonProgress)
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LessonProgress>()
                .HasOne(lp => lp.Lesson)
                .WithMany(l => l.UserProgress)
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Achievement Progress
            builder.Entity<AchievementProgress>()
                .HasOne(ap => ap.User)
                .WithMany(u => u.AchievementProgress)
                .HasForeignKey(ap => ap.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AchievementProgress>()
                .HasOne(ap => ap.Achievement)
                .WithMany(a => a.UserProgress)
                .HasForeignKey(ap => ap.AchievementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<OwnedPhilosopher>()
                .HasIndex(op => new { op.UserId, op.PhilosopherId })
                .IsUnique();

            builder.Entity<LessonProgress>()
                .HasIndex(lp => new { lp.UserId, lp.LessonId })
                .IsUnique();

            builder.Entity<AchievementProgress>()
                .HasIndex(ap => new { ap.UserId, ap.AchievementId })
                .IsUnique();

            // Configure enum conversions
            builder.Entity<Philosopher>()
                .Property(p => p.Rarity)
                .HasConversion<int>();

            builder.Entity<Lesson>()
                .Property(l => l.Difficulty)
                .HasConversion<int>();

            // Seed data will be added later
        }
    }
}
