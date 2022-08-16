using QnA.Models.Data.Shared;
using System.ComponentModel.DataAnnotations.Schema;
namespace QnA.Models.Data
{
    public class UserRole : Table
    {
        [ForeignKey("Role")]
        public virtual int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [ForeignKey("User")]
        public virtual int UserId { get; set; }
        public virtual User User { get; set; }

    }
}
