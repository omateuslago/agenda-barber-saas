using System.ComponentModel.DataAnnotations;

namespace BarberSaaS.API.DTOs.Service;

    public class CreateServiceDto
    {
        [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
        [MaxLength(120, ErrorMessage = "O nome deve ter no máximo 120 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 999999.99, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Price { get; set; }

        [Range(1, 1440, ErrorMessage = "A duração deve ser entre 1 e 1440 minutos.")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "O ID da barbearia é obrigatório.")]
        public int BarberShopId { get; set; }
    }