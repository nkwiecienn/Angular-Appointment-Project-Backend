public class UpdateReservationDto
{
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public string? Details { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsReserved { get; set; }
}