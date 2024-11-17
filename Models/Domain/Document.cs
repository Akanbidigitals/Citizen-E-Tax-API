using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen_E_Tax_API.Models.Domain
{
    public class Document
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.Now.Date;
    }
}
