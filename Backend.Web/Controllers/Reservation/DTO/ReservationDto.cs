namespace Backend.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public string? Details { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsReserved { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
}