﻿namespace Backend.DTOs;

public class CreateTimeSlotDto
{
    public string Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool IsReserved { get; set; }
    public bool IsPast { get; set; }
    public int? ReservationId { get; set; }
}