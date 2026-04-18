namespace BarberSaaS.API.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }

        public int BarberShopId { get; set; }
        public BarberShop? BarberShop { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}
