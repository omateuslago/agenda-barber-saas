namespace BarberSaaS.API.Models
{
    public class BarberShop
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Barber> Barbers { get; set; } = new();
        public List<Service> Services { get; set; } = new();
    }
}
