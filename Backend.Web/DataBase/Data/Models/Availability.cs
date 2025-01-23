using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class Availability
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required string Type { get; set; } 

    public string? Day { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public string? DaysOfWeek { get; set; } 

    public string? TimeRanges { get; set; } 
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}