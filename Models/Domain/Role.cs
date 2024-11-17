using System.ComponentModel.DataAnnotations;

namespace Citizen_E_Tax_API.Models.Domain
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserRole> Users { get; set; } = [];
    }
}
