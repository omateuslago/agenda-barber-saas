namespace BarberSaaS.API.Models
{
    public class Barber
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public int BarberShopId { get; set; }
        public BarberShop? BarberShop { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}
