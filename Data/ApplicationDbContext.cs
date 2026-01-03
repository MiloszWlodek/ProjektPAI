using Microsoft.EntityFrameworkCore;
using ProjektPAI.Models;
using System.Collections.Generic;

namespace ProjektPAI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Building> Buildings { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<MovementPoint> MovementPoints { get; set; }
    public DbSet<Direction> Directions { get; set; }
}
