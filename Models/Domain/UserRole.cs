using System.ComponentModel.DataAnnotations.Schema;

namespace Citizen_E_Tax_API.Models.Domain
{
    public class UserRole
    {
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
