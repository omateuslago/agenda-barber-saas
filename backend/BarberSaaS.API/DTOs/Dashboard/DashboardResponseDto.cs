namespace BarberSaaS.API.DTOs.Dashboard
{
    public class DashboardResponseDto
    {
        public int TotalBarbers { get; set; }
        public int TotalServices { get; set; }
        public int AppointmentsToday { get; set; }
        public int AppointmentsThisMonth { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }
}
