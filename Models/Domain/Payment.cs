using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen_E_Tax_API.Models.Domain
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string PaymentMonth {  get; set; }  

        public string PaymentReference { get; set; }


    }
}
