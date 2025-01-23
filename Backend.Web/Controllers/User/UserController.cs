using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DataContext _context;

    public UserController(DataContext context)
    {
        _context = context;
    }

[HttpGet("{id}/Availabilities")]
public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetUserAvailabilities(int id)
{
    try 
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == id);
        if (!userExists)
        {
            return NotFound();
        }

        var availabilities = await _context.Availabilities
            .Where(a => a.UserId == id)
            .Include(a => a.User)
            .ToListAsync();

        var result = availabilities.Select(a => {
            try 
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
                            ?? new List<TimeRangeDto>()
                        : new List<TimeRangeDto>(),
                    UserId = a.UserId,
                    UserName = $"{a.User.FirstName} {a.User.LastName}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing availability {a.Id}: {ex.Message}");
                return null;
            }
        })
        .Where(dto => dto != null)
        .ToList();

        return Ok(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetUserAvailabilities: {ex.Message}");
        return StatusCode(500, "An error occurred while processing your request.");
    }
}

// GET: api/User/{id}/Absences
[HttpGet("{id}/Absences")]
public async Task<ActionResult<IEnumerable<AbsenceDto>>> GetUserAbsences(int id)
{
    var userExists = await _context.Users.AnyAsync(u => u.Id == id);
    if (!userExists)
    {
        return NotFound();
    }

    var absences = await _context.Absences
        .Where(a => a.UserId == id)
        .Include(a => a.User)
        .ToListAsync();

    var result = absences.Select(a => new AbsenceDto
    {
        Id = a.Id,
        Day = a.Day,
        UserId = a.UserId,
        UserName = $"{a.User.FirstName} {a.User.LastName}"
    });

    return Ok(result);
}

// GET: api/User/{id}/Reservations
[HttpGet("{id}/Reservations")]
public async Task<ActionResult<IEnumerable<ReservationDto>>> GetUserReservations(int id)
{
    var userExists = await _context.Users.AnyAsync(u => u.Id == id);
    if (!userExists)
    {
        return NotFound();
    }

    var reservations = await _context.Reservations
        .Where(r => r.UserId == id)
        .Include(r => r.User) // Include the User entity
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

// GET: api/User/role/{role}
[HttpGet("role/{role}")]
public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(int role)
{
    var users = await _context.Users
        .Where(u => u.Role == role)
        .Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Role = u.Role
        })
        .ToListAsync();

    if (users == null || !users.Any())
    {
        return NotFound($"Nie znaleziono użytkowników z rolą {role}.");
    }

    return Ok(users);
}

}
