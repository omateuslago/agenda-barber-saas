using System.ComponentModel.DataAnnotations;

namespace BarberSaaS.API.DTOs.Barber;

    public class CreateBarberDto
    {
        [Required(ErrorMessage = "O nome do barbeiro é obrigatório.")]
        [MaxLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID da barbearia é obrigatório.")]
        public int BarberShopId { get; set; }
    }