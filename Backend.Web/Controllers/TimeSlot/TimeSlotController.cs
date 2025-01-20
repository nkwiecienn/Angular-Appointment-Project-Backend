using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeSlotController : ControllerBase
{
    private readonly DataContext _context;

    public TimeSlotController(DataContext context)
    {
        _context = context;
    }

    // GET: api/TimeSlot
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetTimeSlots()
    {
        var timeSlots = await _context.TimeSlots.ToListAsync();

        var result = timeSlots.Select(slot => new TimeSlotDto
        {
            Id = slot.Id,
            Date = slot.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            IsReserved = slot.IsReserved,
            IsPast = slot.IsPast,
            ReservationId = slot.ReservationId
        });

        return Ok(result);
    }

    // GET: api/TimeSlot/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TimeSlotDto>> GetTimeSlot(int id)
    {
        var slot = await _context.TimeSlots.FindAsync(id);

        if (slot == null)
        {
            return NotFound();
        }

        var result = new TimeSlotDto
        {
            Id = slot.Id,
            Date = slot.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            IsReserved = slot.IsReserved,
            IsPast = slot.IsPast,
            ReservationId = slot.ReservationId
        };

        return Ok(result);
    }
    
    [HttpGet("byDate")]
    public async Task<ActionResult<IEnumerable<TimeSlotDto>>> GetTimeSlotsByDate([FromQuery] string date)
    {
        var timeSlots = await _context.TimeSlots
            .Where(slot => slot.Date == date)
            .ToListAsync();

        return Ok(timeSlots);
    }

    // POST: api/TimeSlot
    [HttpPost]
    public async Task<ActionResult<TimeSlot>> CreateTimeSlot(CreateTimeSlotDto createDto)
    {
        // Sprawdzenie, czy slot już istnieje
        var existingSlot = await _context.TimeSlots
            .FirstOrDefaultAsync(slot =>
                slot.Date == createDto.Date &&
                slot.StartTime == createDto.StartTime &&
                slot.EndTime == createDto.EndTime);

        if (existingSlot != null)
        {
            return Conflict("Slot already exists.");
        }

        var slot = new TimeSlot
        {
            Date = createDto.Date,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime,
            IsReserved = createDto.IsReserved,
            IsPast = createDto.IsPast,
            ReservationId = createDto.ReservationId
        };

        _context.TimeSlots.Add(slot);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTimeSlot), new { id = slot.Id }, slot);
    }


    // PUT: api/TimeSlot/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimeSlot(int id, UpdateTimeSlotDto updateDto)
    {
        var slot = await _context.TimeSlots.FindAsync(id);
        if (slot == null)
        {
            return NotFound();
        }
        
        slot.IsReserved = updateDto.IsReserved;
        slot.IsPast = updateDto.IsPast;
        slot.ReservationId = updateDto.ReservationId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/TimeSlot/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimeSlot(int id)
    {
        var slot = await _context.TimeSlots.FindAsync(id);
        if (slot == null)
        {
            return NotFound();
        }

        _context.TimeSlots.Remove(slot);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPut("{id}/reserve")]
    public async Task<IActionResult> ReserveSlot(int id, [FromBody] int reservationId)
    {
        var slot = await _context.TimeSlots.FindAsync(id);

        if (slot == null)
        {
            return NotFound("Slot not found.");
        }

        if (slot.IsReserved)
        {
            return Conflict("Slot is already reserved.");
        }

        var reservation = await _context.Reservations.FindAsync(reservationId);

        if (reservation == null)
        {
            return NotFound("Reservation not found.");
        }

        slot.IsReserved = true;
        slot.ReservationId = reservationId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/release")]
    public async Task<IActionResult> ReleaseSlot(int id)
    {
        var slot = await _context.TimeSlots.FindAsync(id);

        if (slot == null)
        {
            return NotFound("Slot not found.");
        }

        if (!slot.IsReserved)
        {
            return BadRequest("Slot is not reserved.");
        }

        slot.IsReserved = false;
        slot.ReservationId = null;

        await _context.SaveChangesAsync();
        return NoContent();
    }

}
