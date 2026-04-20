using System.ComponentModel.DataAnnotations;

namespace BarberSaaS.API.DTOs.Appointment;

    public class CreateAppointmentDto
    {
        [Required]
        [MaxLength(120)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        public DateTime StartsAt { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public int ServiceId { get; set; }
    }