using QnA.Models.Data.Shared;
namespace QnA.Models.Data
{
    public class Role : Table
    {
        public virtual string Name { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
