using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly DataContext _context;

    public ReservationsController(DataContext context)
    {
        _context = context;
    }

    // GET: api/Reservations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.User)
            .ToListAsync();

        var result = reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Length = r.Length,
            Type = r.Type,
            Gender = r.Gender,
            Age = r.Age,
            Details = r.Details,
            IsCanceled = r.IsCanceled,
            IsReserved = r.IsReserved,
            UserId = r.UserId,
            UserName = $"{r.User.FirstName} {r.User.LastName}"
        });

        return Ok(result);
    }

    // GET: api/Reservations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        var result = new ReservationDto
        {
            Id = reservation.Id,
            Date = reservation.Date,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            Length = reservation.Length,
            Type = reservation.Type,
            Gender = reservation.Gender,
            Age = reservation.Age,
            Details = reservation.Details,
            IsCanceled = reservation.IsCanceled,
            IsReserved = reservation.IsReserved,
            UserId = reservation.UserId,
            UserName = $"{reservation.User.FirstName} {reservation.User.LastName}"
        };

        return Ok(result);
    }

    // POST: api/Reservations
    [HttpPost]
    public async Task<ActionResult<Reservation>> CreateReservation(CreateReservationDto createDto)
    {
        var reservation = new Reservation
        {
            Date = createDto.Date,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime,
            Length = createDto.Length,
            Type = createDto.Type,
            Gender = createDto.Gender,
            Age = createDto.Age,
            Details = createDto.Details,
            IsCanceled = false,
            IsReserved = false,
            UserId = createDto.UserId
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
    }

    // PUT: api/Reservations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, UpdateReservationDto updateDto)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }
        
        reservation.IsCanceled = updateDto.IsCanceled;
        reservation.IsReserved = updateDto.IsReserved;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Reservations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
