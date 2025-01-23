using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
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
            .Include(r => r.Doctor)
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
            UserName = $"{r.User.FirstName} {r.User.LastName}",
            DoctorId = r.DoctorId,
            DoctorName = $"{r.Doctor.FirstName} {r.Doctor.LastName}"
        });

        return Ok(result);
    }

    // GET: api/Reservations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetReservation(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.User)
            .Include(r => r.Doctor)
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
            UserName = $"{reservation.User.FirstName} {reservation.User.LastName}",
            DoctorId = reservation.DoctorId,
            DoctorName = $"{reservation.Doctor.FirstName} {reservation.Doctor.LastName}"
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
            UserId = createDto.UserId,
            DoctorId = createDto.DoctorId
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
    
    [HttpPut("reserveAll")]
    public async Task<IActionResult> ReserveAllPending([FromBody] List<int> reservationIds)
    {
        try
        {
            var reservations = await _context.Reservations
                .Where(r => reservationIds.Contains(r.Id))
                .ToListAsync();

            foreach (var reservation in reservations)
            {
                reservation.IsReserved = true;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound("Reservation not found.");
        }

        reservation.IsCanceled = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Reservations/doctor/{doctorId}
    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetDoctorReservations(int doctorId)
    {
        var doctorExists = await _context.Users.AnyAsync(u => u.Id == doctorId);
        if (!doctorExists)
        {
            return NotFound("Nie znaleziono doktora o podanym ID.");
        }

        var reservations = await _context.Reservations
            .Include(r => r.User)      // ładuje dane użytkownika
            .Include(r => r.Doctor)    // ładuje dane doktora
            .Where(r => r.DoctorId == doctorId)
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
            UserName = $"{r.User.FirstName} {r.User.LastName}",
            DoctorId = r.DoctorId,
            DoctorName = $"{r.Doctor.FirstName} {r.Doctor.LastName}"
        });

        return Ok(result);
    }
}
