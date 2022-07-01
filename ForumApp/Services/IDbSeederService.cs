using ForumApp.Store;
using ForumApp.Store.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApp.Seeder
{

    public interface IDbSeederService
    {
        Task SeedAdminUser();
    }
    public class DbSeederService : IDbSeederService
    {
        private ApplicationDbContext _context;

        public DbSeederService(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task SeedAdminUser()
        {
            var admin = new IdentityUser
            {
                UserName = "myemail@email.com",
                NormalizedUserName = "MYEMAIL@EMAIL.COM",
                Email = "myemail@email.com",
                NormalizedEmail = "MYEMAIL@EMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var moderatorFirst = new IdentityUser
            {
                UserName = "moderator1@email.com",
                NormalizedUserName = "MODERATOR1@EMAIL.COM",
                Email = "moderator1@email.com",
                NormalizedEmail = "MODERATOR1@EMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var moderatorSecond = new IdentityUser
            {
                UserName = "moderator2@email.com",
                NormalizedUserName = "MODERATOR2@EMAIL.COM",
                Email = "moderator2@email.com",
                NormalizedEmail = "MODERATOR2@EMAIL.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var moderatorsToAdd = new List<IdentityUser> { moderatorFirst, moderatorSecond};
            var roleStore = new RoleStore<IdentityRole>(_context);

            foreach (var role in Roles.AllRoles)
            {
                if (!_context.Roles.Any(r => r.Name == role))
                {
                    await roleStore.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper() });
                }
            }
            
            
            if (!_context.Users.Any(u => u.UserName == admin.UserName))
            {
                var password = new PasswordHasher<IdentityUser>();
                var hashed = password.HashPassword(admin, "P@ssw0rd");
                admin.PasswordHash = hashed;
                var userStore = new UserStore<IdentityUser>(_context);
                await userStore.CreateAsync(admin);
                await userStore.AddToRoleAsync(admin, Roles.Admin);
            }
            foreach(var user in moderatorsToAdd)
            {
                if (!_context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<IdentityUser>();
                    var hashed = password.HashPassword(user, "P@ssw0rd");
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<IdentityUser>(_context);
                    await userStore.CreateAsync(user);
                    await userStore.AddToRoleAsync(user, Roles.Moderator);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
    
