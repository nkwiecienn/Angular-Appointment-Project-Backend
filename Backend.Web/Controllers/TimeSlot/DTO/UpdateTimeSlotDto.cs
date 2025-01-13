namespace Backend.DTOs;

public class UpdateTimeSlotDto
{ 
    public bool IsReserved { get; set; }
    public bool IsPast { get; set; }
    public int? ReservationId { get; set; }
}