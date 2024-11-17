using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using Document = Citizen_E_Tax_API.Models.Domain.Document;

namespace Citizen_E_Tax_API.Models.Domain
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password {  get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [Column(TypeName = "decimal(18,2)")] 
        public decimal MonthlyIncome { get; set; }
        public string TaxIdentificationNumber { get; set; } // Generate 10DigitNumber
        public string IdentificationNumber { get; set; }
        public bool Isverified { get; set; }

        public List<Document> Documents { get; set; } 
        public List<Payment> payments { get; set; } 

        public List<UserRole> Roles { get; set; }

    }
}
