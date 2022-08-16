using System.Data.Entity;

namespace QnA.Models.Data.Shared
{
    public class QnADbInitializer : DropCreateDatabaseIfModelChanges<QnADbContext>
    {
        protected override void Seed(QnADbContext context)
        {
            var roleUser = new Role() { Name = "User" };
            var roleAdmin = new Role() { Name = "Admin" };
            context.Roles.AddRange(new List<Role>
            {
                roleUser,
                roleAdmin
            });
            context.Tags.AddRange(new List<Tag>
            {
                new Tag() { Name = "tag 1" },
                new Tag() { Name = "tag 2" },
                new Tag() { Name = "tag 3" },
                new Tag() { Name = "tag 4" },
            });
            var user1 = new User()
            {
                Name = "user1",
                Email = "test1@test1.com",
                PasswordHash = "$HASH|V1$10000$6UIWMvLJirqW2UhgHjbXLHd+siuXiTkEAL5DJXlfRN5Ws+fR" // password
            };
            var user2 = new User()
            {
                Name = "user2",
                Email = "test2@test2.com",
                PasswordHash = "$HASH|V1$10000$DImBmRFezS/3O0Kv/g8QJ6cKGOQJzbcf2u00Hsve/CWGuSGe" // 01010101
            };
            var user3 = new User()
            {
                Name = "admin",
                Email = "admin@qna.com",
                PasswordHash = "$HASH|V1$10000$n/cSSFSuo01Ar1o4E4Po3tTOulHjepE/SEGEjs+VDitMqFc4" // 1111
            };
            context.Users.AddRange(new List<User>() 
            { 
                user1,
                user2,
                user3
            });
            context.SaveChanges();
            context.UserRoles.AddRange(new List<UserRole>()
            {
                new UserRole()
                {
                    UserId = user1.Id,
                    RoleId = roleUser.Id
                },
                new UserRole()
                {
                    UserId = user2.Id,
                    RoleId = roleUser.Id
                },
                new UserRole()
                {
                    UserId = user3.Id,
                    RoleId = roleUser.Id
                },
                new UserRole()
                {
                    UserId = user3.Id,
                    RoleId = roleAdmin.Id
                }
            });
            context.SaveChanges();
        }
    }
}