namespace Citizen_E_Tax_API.Models.DTOs
{
    public class RegisterTx
    {
        public string FullName { get; set; }
        public string Password {  get; set; }
        public  string Email { get; set; }
        public  string PhoneNumber { get; set; }
        public  string Address { get; set; }
        public  decimal MonthlyIncome { get; set; }
        public string IdentificationNumber { get; set; }
        public  IFormFile IdentificationDocument { get; set; }
    }
}
