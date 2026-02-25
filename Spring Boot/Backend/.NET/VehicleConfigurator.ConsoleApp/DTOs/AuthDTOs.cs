namespace VehicleConfigurator.ConsoleApp.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AuthName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyStNo { get; set; } = string.Empty;
        public string CompanyVatNo { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string AuthTel { get; set; } = string.Empty;
        // Other fields as necessary
        public string Add1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
