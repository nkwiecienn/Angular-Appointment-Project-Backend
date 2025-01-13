using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class DaySchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public string DayOfWeek { get; set; } = null!; // np. "Poniedziałek"

    [Required]
    public int ReservedCount { get; set; }

    public ICollection<TimeSlot> Slots { get; set; } = new List<TimeSlot>();

    // Relacja do tygodniowego harmonogramu
    [Required]
    public int WeekScheduleId { get; set; }
    [ForeignKey(nameof(WeekScheduleId))]
    public WeekSchedule WeekSchedule { get; set; } = null!;
}