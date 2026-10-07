using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingSlotManagement.Data;
using ParkingSlotManagement.Models;

namespace ParkingSlotManagement.Controllers
{
    [ApiController]
    [Route("api/slots")]
    public class SlotsController : ControllerBase
    {
        private readonly ParkingDbContext _context;

        public SlotsController(ParkingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots()
        {
            var slots = await _context.ParkingSlots.ToListAsync();
            return Ok(slots);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable([FromQuery] string type)
        {
            var slots = _context.ParkingSlots.Where(s => !s.IsOccupied);

            if (!string.IsNullOrEmpty(type))
            {
                if (type != "Car" && type != "Bike")
                    return BadRequest("Type must be Car or Bike.");

                slots = slots.Where(s => s.SlotType == type);
            }

            return Ok(await slots.ToListAsync());
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var slots = await _context.ParkingSlots.ToListAsync();

            return Ok(new
            {
                cars = CountByType(slots, "Car"),
                bikes = CountByType(slots, "Bike")
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlot(CreateSlotRequest request)
        {
            if (request.SlotType != "Car" && request.SlotType != "Bike")
                return BadRequest("Slot type must be Car or Bike.");

            var alreadyExists = await _context.ParkingSlots
                .AnyAsync(s => s.SlotNumber == request.SlotNumber);

            if (alreadyExists)
                return BadRequest("Slot number already exists.");

            var slot = new ParkingSlot
            {
                SlotNumber = request.SlotNumber,
                SlotType = request.SlotType
            };

            _context.ParkingSlots.Add(slot);
            await _context.SaveChangesAsync();

            return Created($"/api/slots/{slot.Id}", slot);
        }

        [HttpPost("{id}/park")]
        public async Task<IActionResult> Park(int id, ParkRequest request)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
                return NotFound();

            if (slot.IsOccupied)
                return BadRequest("Slot is already occupied.");

            slot.IsOccupied = true;
            slot.VehicleNumber = request.VehicleNumber;
            slot.ParkedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(slot);
        }

        [HttpPost("{id}/exit")]
        public async Task<IActionResult> Exit(int id)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
                return NotFound();

            if (!slot.IsOccupied)
                return BadRequest("Slot is already empty.");

            var hours = 1;
            if (slot.ParkedAt != null)
            {
                hours = (int)Math.Ceiling((DateTime.Now - slot.ParkedAt.Value).TotalHours);
                if (hours < 1)
                    hours = 1;
            }

            var rate = slot.SlotType == "Car" ? 20 : 10;

            var result = new
            {
                vehicleNumber = slot.VehicleNumber,
                hoursParked = hours,
                fee = hours * rate
            };

            slot.IsOccupied = false;
            slot.VehicleNumber = null;
            slot.ParkedAt = null;

            await _context.SaveChangesAsync();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var slot = await _context.ParkingSlots.FindAsync(id);
            if (slot == null)
                return NotFound();

            if (slot.IsOccupied)
                return BadRequest("Cannot delete an occupied slot.");

            _context.ParkingSlots.Remove(slot);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private static object CountByType(List<ParkingSlot> slots, string type)
        {
            var list = slots.Where(s => s.SlotType == type).ToList();
            return new
            {
                total = list.Count,
                occupied = list.Count(s => s.IsOccupied),
                free = list.Count(s => !s.IsOccupied)
            };
        }
    }
}
