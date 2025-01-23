namespace Backend.DTOs;

public class CreateAvailabilityDto
{
    public string Type { get; set; } = string.Empty;
    public string? Day { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public List<int>? DaysOfWeek { get; set; }
    public List<TimeRangeDto> TimeRanges { get; set; } = new();
    public int UserId { get; set; }
}