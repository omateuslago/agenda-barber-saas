namespace BarberSaaS.API.DTOs.Service;

    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public int BarberShopId { get; set; }
    }