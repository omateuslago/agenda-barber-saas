namespace BarberSaaS.API.DTOs.Barber;

    public class BarberResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BarberShopId { get; set; }
    }