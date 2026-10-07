using Microsoft.EntityFrameworkCore;
using ParkingSlotManagement.Models;

namespace ParkingSlotManagement.Data
{
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
        {
        }

        public DbSet<ParkingSlot> ParkingSlots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkingSlot>()
                .HasIndex(s => s.SlotNumber)
                .IsUnique();

            var seed = new List<ParkingSlot>();

            for (int i = 1; i <= 6; i++)
                seed.Add(new ParkingSlot { Id = i, SlotNumber = "A" + i, SlotType = "Car" });

            for (int i = 1; i <= 4; i++)
                seed.Add(new ParkingSlot { Id = 6 + i, SlotNumber = "B" + i, SlotType = "Bike" });

            modelBuilder.Entity<ParkingSlot>().HasData(seed);
        }
    }
}
