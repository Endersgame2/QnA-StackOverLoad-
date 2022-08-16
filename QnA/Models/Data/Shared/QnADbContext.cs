using System.Data.Entity;

namespace QnA.Models.Data.Shared
{
    public class QnADbContext : DbContext
    {
        public QnADbContext() : base("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=QnA")
        {
            Database.SetInitializer(new QnADbInitializer());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<QuestionTag> QuestionTags { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}
