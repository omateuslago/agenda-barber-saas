namespace BarberSaaS.API.Models;

    public class BarberShop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }

        public ICollection<Barber> Barbers { get; set; } = new List<Barber>();
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }