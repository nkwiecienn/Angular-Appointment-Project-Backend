using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Date { get; set; }

    [Required]
    public string StartTime { get; set; }

    [Required]
    public string EndTime { get; set; }

    [Required]
    public int Length { get; set; }

    [Required]
    public required string Type { get; set; } 

    [Required]
    public required string Gender { get; set; } 

    [Required]
    public int Age { get; set; }

    public string? Details { get; set; }

    [Required]
    public bool IsCanceled { get; set; }

    [Required]
    public bool IsReserved { get; set; }
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    
    [Required]
    public int DoctorId { get; set; }
    [ForeignKey(nameof(DoctorId))]
    public User Doctor { get; set; } = null!;
}