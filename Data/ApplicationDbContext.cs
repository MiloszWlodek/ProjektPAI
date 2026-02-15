using InfoInfo.Models;
using InfoInfo.Models.Campus;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfoInfo.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<Opinion> Opinions { get; set; }

        public DbSet<Building> Buildings { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<MovementPoint> MovementPoints { get; set; }
        public DbSet<Direction> Directions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Text>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Texts)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Text>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Texts)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opinion>()
                .HasOne(o => o.Text)
                .WithMany(t => t.Opinions)
                .HasForeignKey(o => o.TextId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Opinion>()
                .HasOne(o => o.Author)
                .WithMany(u => u.Opinions)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Building>()
                .HasMany(b => b.Floors)
                .WithOne(f => f.Building)
                .HasForeignKey(f => f.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Floor>()
                .HasMany(f => f.Rooms)
                .WithOne(r => r.Floor)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Floor>()
                .HasMany(f => f.MovementPoints)
                .WithOne(p => p.Floor)
                .HasForeignKey(p => p.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.EntryPoint)
                .WithMany()
                .HasForeignKey(r => r.EntryPointId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Direction>()
                .HasOne(d => d.FromPoint)
                .WithMany(p => p.FromDirections)
                .HasForeignKey(d => d.FromPointId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Direction>()
                .HasOne(d => d.ToPoint)
                .WithMany(p => p.ToDirections)
                .HasForeignKey(d => d.ToPointId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Direction>()
                .HasOne(d => d.ToRoom)
                .WithMany()
                .HasForeignKey(d => d.ToRoomId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
