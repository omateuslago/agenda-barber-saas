namespace BarberSaaS.API.DTOs.Auth
{
    public class UserMeResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
