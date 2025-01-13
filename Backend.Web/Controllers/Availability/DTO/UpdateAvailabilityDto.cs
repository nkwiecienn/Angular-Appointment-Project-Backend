namespace Backend.DTOs;

public class UpdateAvailabilityDto
{
    public string Type { get; set; } = string.Empty; // "single-day" | "range"
    public DateTime? Day { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<int>? DaysOfWeek { get; set; }
    public List<TimeRangeDto> TimeRanges { get; set; } = new();
    public int UserId { get; set; }
}