using System.ComponentModel.DataAnnotations;

namespace ParkingSlotManagement.Models
{
    public class ParkingSlot
    {
        public int Id { get; set; }

        [Required]
        public string SlotNumber { get; set; }

        [Required]
        public string SlotType { get; set; }

        public bool IsOccupied { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime? ParkedAt { get; set; }
    }

    public class CreateSlotRequest
    {
        [Required]
        public string SlotNumber { get; set; }

        [Required]
        public string SlotType { get; set; }
    }

    public class ParkRequest
    {
        [Required]
        public string VehicleNumber { get; set; }
    }
}
