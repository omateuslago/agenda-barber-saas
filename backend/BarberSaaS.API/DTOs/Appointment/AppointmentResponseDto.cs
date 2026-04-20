namespace BarberSaaS.API.DTOs.Appointment;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime StartsAt { get; set; }
    public string Status { get; set; } = string.Empty;

    public int BarberId { get; set; }
    public string BarberName { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public int ServiceDurationMinutes { get; set; }
}