namespace Backend.DTOs;

public class AvailabilityDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "single-day" | "range"
    public DateTime? Day { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<int>? DaysOfWeek { get; set; } // np. [1, 2, 3] jako dni tygodnia
    public List<TimeRangeDto> TimeRanges { get; set; } = new();
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class TimeRangeDto
{
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
}