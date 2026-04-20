using System.ComponentModel.DataAnnotations;

namespace BarberSaaS.API.DTOs.BarberShop;

    public class UpdateBarberShopDto
    {
        [Required(ErrorMessage = "O nome da barbearia é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone da barbearia é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres.")]
        public string Phone { get; set; } = string.Empty;
    }