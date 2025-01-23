using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AvailabilityController : ControllerBase
{
    private readonly DataContext _context;

    public AvailabilityController(DataContext context)
    {
        _context = context;
    }

    // GET: api/Availability
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAvailabilities()
    {
        var availabilities = await _context.Availabilities
            .Include(a => a.User)
            .ToListAsync();

        var result = availabilities.Select(a =>
        {
            return new AvailabilityDto
            {
                Id = a.Id,
                Type = a.Type,
                Day = a.Type == "single-day" ? a.Day : null,
                DateFrom = a.Type == "range" ? a.DateFrom : null,
                DateTo = a.Type == "range" ? a.DateTo : null,
                DaysOfWeek = a.Type == "range" && !string.IsNullOrEmpty(a.DaysOfWeek)
                    ? JsonSerializer.Deserialize<List<int>>(a.DaysOfWeek)
                    : null,
                TimeRanges = !string.IsNullOrEmpty(a.TimeRanges)
                    ? JsonSerializer.Deserialize<List<TimeRangeDto>>(a.TimeRanges)
                    : new List<TimeRangeDto>(),
                UserId = a.UserId,
                UserName = $"{a.User.FirstName} {a.User.LastName}"
            };
        });

        return Ok(result);
    }



    // GET: api/Availability/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<AvailabilityDto>> GetAvailability(int id)
    {
        var availability = await _context.Availabilities
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (availability == null)
        {
            return NotFound();
        }
        
        var result = new AvailabilityDto
        {
            Id = availability.Id,
            Type = availability.Type,
            Day = availability.Type == "single-day" ? availability.Day : null,
            DateFrom = availability.Type == "range" ? availability.DateFrom : null,
            DateTo = availability.Type == "range" ? availability.DateTo : null,
            DaysOfWeek = availability.Type == "range" && !string.IsNullOrEmpty(availability.DaysOfWeek)
                ? JsonSerializer.Deserialize<List<int>>(availability.DaysOfWeek)
                : null,
            TimeRanges = !string.IsNullOrEmpty(availability.TimeRanges)
                ? JsonSerializer.Deserialize<List<TimeRangeDto>>(availability.TimeRanges)
                : new List<TimeRangeDto>(),
            UserId = availability.UserId,
            UserName = $"{availability.User.FirstName} {availability.User.LastName}"
        };

        return Ok(result);
    }



    // POST: api/Availability
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AvailabilityDto>> CreateAvailability(CreateAvailabilityDto createDto)
    {
        var user = await _context.Users.FindAsync(createDto.UserId);
        if (user == null)
        {
            return BadRequest("Użytkownik nie istnieje.");
        }

        var availability = new Availability
        {
            Type = createDto.Type,
            Day = createDto.Day,
            DateFrom = createDto.DateFrom,
            DateTo = createDto.DateTo,
            DaysOfWeek = createDto.DaysOfWeek != null
                ? JsonSerializer.Serialize(createDto.DaysOfWeek) 
                : null,
            TimeRanges = JsonSerializer.Serialize(createDto.TimeRanges),
            UserId = createDto.UserId
        };


        _context.Availabilities.Add(availability);
        await _context.SaveChangesAsync();

        var availabilityDto = new AvailabilityDto
        {
            Id = availability.Id,
            Type = availability.Type,
            Day = availability.Day,
            DateFrom = availability.DateFrom,
            DateTo = availability.DateTo,
            DaysOfWeek = createDto.DaysOfWeek,
            TimeRanges = createDto.TimeRanges,
            UserId = availability.UserId,
            UserName = $"{user.FirstName} {user.LastName}"
        };

        return CreatedAtAction(nameof(GetAvailability), new { id = availability.Id }, availabilityDto);
    }

    // PUT: api/Availability/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAvailability(int id, UpdateAvailabilityDto updateDto)
    {
        var availability = await _context.Availabilities.FindAsync(id);
        if (availability == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(updateDto.UserId);
        if (user == null)
        {
            return BadRequest("Użytkownik nie istnieje.");
        }

        availability.Type = updateDto.Type;
        availability.Day = updateDto.Day;
        availability.DateFrom = updateDto.DateFrom;
        availability.DateTo = updateDto.DateTo;
        availability.DaysOfWeek = updateDto.DaysOfWeek != null
            ? string.Join(",", updateDto.DaysOfWeek)
            : null;
        availability.TimeRanges = JsonSerializer.Serialize(updateDto.TimeRanges);
        availability.UserId = updateDto.UserId;

        _context.Entry(availability).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AvailabilityExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Availability/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAvailability(int id)
    {
        var availability = await _context.Availabilities.FindAsync(id);
        if (availability == null)
        {
            return NotFound();
        }

        _context.Availabilities.Remove(availability);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AvailabilityExists(int id)
    {
        return _context.Availabilities.Any(e => e.Id == id);
    }
}
