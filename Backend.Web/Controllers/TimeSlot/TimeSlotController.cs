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

    // POST: api/TimeSlot
    [HttpPost]
    public async Task<ActionResult<TimeSlot>> CreateTimeSlot(CreateTimeSlotDto createDto)
    {
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
}
