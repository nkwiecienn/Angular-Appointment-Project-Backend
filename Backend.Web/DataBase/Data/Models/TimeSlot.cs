using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class TimeSlot
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
    public bool IsReserved { get; set; }

    [Required]
    public bool IsPast { get; set; }

    // Relacja do rezerwacji
    public int? ReservationId { get; set; }
    [ForeignKey(nameof(ReservationId))]
    public Reservation? Reservation { get; set; }
}