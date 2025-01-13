using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public int Length { get; set; }

    [Required]
    public required string Type { get; set; } // "konsultacja", "badanie", etc.

    [Required]
    public required string PatientName { get; set; }

    [Required]
    public required string PatientSurname { get; set; }

    [Required]
    public required string Gender { get; set; } // 'kobieta', 'mężczyzna', `inny`

    [Required]
    public int Age { get; set; }

    public string? Details { get; set; }

    [Required]
    public bool IsCanceled { get; set; }

    [Required]
    public bool IsReserved { get; set; }

    // Relacja do użytkownika
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}