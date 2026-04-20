using System.ComponentModel.DataAnnotations;

namespace BarberSaaS.API.DTOs.Appointment;

    public class UpdateAppointmentStatusDto
    {
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;
    }