namespace BarberSaaS.API.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public string Status { get; set; } = "Scheduled";

        public int BarberId { get; set; }
        public Barber? Barber { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
