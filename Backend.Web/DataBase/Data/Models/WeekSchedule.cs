using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataBase.Data.Models;

public class WeekSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime WeekStart { get; set; }

    public ICollection<DaySchedule> Days { get; set; } = new List<DaySchedule>();
}