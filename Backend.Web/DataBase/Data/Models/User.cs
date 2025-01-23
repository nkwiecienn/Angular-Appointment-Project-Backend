using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public required string AccountId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public DateTime CreationDateTime { get; set; }
    
    [Required]
    public required int Role { get; set; }
    
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Absence> Absences { get; set; } = new List<Absence>();
    public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
}