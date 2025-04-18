﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataBase.Data;
using Backend.DataBase.Data.Models;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class AbsenceController : ControllerBase
{
    
    private readonly DataContext _context;

    public AbsenceController(DataContext context)
    {
        _context = context;
    }

    // GET: api/Absence
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AbsenceDto>>> GetAbsences()
    {
        var absences = await _context.Absences
            .Include(a => a.User)
            .Select(a => new AbsenceDto
            {
                Id = a.Id,
                Day = a.Day,
                UserId = a.UserId,
                UserName = $"{a.User.FirstName} {a.User.LastName}"
            })
            .ToListAsync();

        return Ok(absences);
    }

    // GET: api/Absence/{id}
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<AbsenceDto>> GetAbsence(int id)
    {
        var absence = await _context.Absences
            .Include(a => a.User)
            .Where(a => a.Id == id)
            .Select(a => new AbsenceDto
            {
                Id = a.Id,
                Day = a.Day,
                UserId = a.UserId,
                UserName = $"{a.User.FirstName} {a.User.LastName}"
            })
            .FirstOrDefaultAsync();

        if (absence == null)
        {
            return NotFound();
        }

        return Ok(absence);
    }
    
    
    // POST: api/Absence
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AbsenceDto>> CreateAbsence(CreateAbsenceDto createDto)
    {
        var user = await _context.Users.FindAsync(createDto.UserId);
        if (user == null)
        {
            return BadRequest("Użytkownik nie istnieje.");
        }
        
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try 
            {
                var absence = new Absence
                {
                    Day = createDto.Day,
                    UserId = createDto.UserId
                };

                _context.Absences.Add(absence);
                
                var reservationsToCancel = await _context.Reservations
                    .Where(r => r.DoctorId == createDto.UserId && r.Date == createDto.Day)
                    .ToListAsync();

                foreach (var reservation in reservationsToCancel)
                {
                    reservation.IsCanceled = true;
                }

                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                var absenceDto = new AbsenceDto
                {
                    Id = absence.Id,
                    Day = absence.Day,
                    UserId = absence.UserId,
                    UserName = $"{user.FirstName} {user.LastName}"
                };

                return CreatedAtAction(nameof(GetAbsence), new { id = absence.Id }, absenceDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Błąd podczas tworzenia nieobecności: {ex.Message}");
            }
        }
    }

    // PUT: api/Absence/{id}
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAbsence(int id, UpdateAbsenceDto updateDto)
    {
        var absence = await _context.Absences.FindAsync(id);
        if (absence == null)
        {
            return NotFound();
        }
        
        var user = await _context.Users.FindAsync(updateDto.UserId);
        if (user == null)
        {
            return BadRequest("Użytkownik nie istnieje.");
        }

        absence.Day = updateDto.Day;
        absence.UserId = updateDto.UserId;

        _context.Entry(absence).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AbsenceExists(id))
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

    // DELETE: api/Absence/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAbsence(int id)
    {
        var absence = await _context.Absences.FindAsync(id);
        if (absence == null)
        {
            return NotFound();
        }

        _context.Absences.Remove(absence);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AbsenceExists(int id)
    {
        return _context.Absences.Any(e => e.Id == id);
    }
}
