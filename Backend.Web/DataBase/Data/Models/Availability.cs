using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class Availability
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required string Type { get; set; } // "single-day" | "range"

    public DateTime? Day { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? DaysOfWeek { get; set; } // Przechowywane jako string (np. "1,2,3")

    public string? TimeRanges { get; set; } // JSON string do przechowywania zakresów czasowych

    // Relacja do użytkownika
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}