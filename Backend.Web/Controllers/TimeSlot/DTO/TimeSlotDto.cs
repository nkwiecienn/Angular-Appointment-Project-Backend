namespace Backend.DTOs;

public class TimeSlotDto
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool IsReserved { get; set; }
    public bool IsPast { get; set; }
    public int? ReservationId { get; set; }
}