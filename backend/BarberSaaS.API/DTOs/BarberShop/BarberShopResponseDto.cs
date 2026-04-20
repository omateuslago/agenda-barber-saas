namespace BarberSaaS.API.DTOs.BarberShop;

    public class BarberShopResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }