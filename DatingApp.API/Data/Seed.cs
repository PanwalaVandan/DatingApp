using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        // private readonly DataContext _context;
        // public Seed(DataContext context)
        // {
        //     _context = context;
        // }

        public static void SeedUsers(UserManager<User> userManager, 
        RoleManager<Role> roleManager)
        {
            // _context.Users.RemoveRange(_context.Users);
            // _context.SaveChanges();

            //Seed Users
            if(!userManager.Users.Any()) {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            // Creating Roles
            var roles = new List<Role>
            {
                new Role{Name = "Member"},
                new Role{Name = "Admin"},
                new Role{Name = "Moderator"},
                new Role{Name = "VIP"},
            };

            foreach(var role in roles) {
                roleManager.CreateAsync(role).Wait();
            }
            foreach(var user in users)
            {
                //create the password hash
                // byte[] passwordHash, passwordsalt;
                // CreatePasswordHash("password", out passwordHash, out passwordsalt);

                // // user.PasswordHash = passwordHash;
                // // user.PasswordSalt = passwordsalt;
                // user.UserName = user.UserName.ToLower();
                
                // context.Users.Add(user);

                userManager.CreateAsync(user, "password").Wait();
                userManager.AddToRoleAsync(user, "Member");
            }

            var adminUser = new User
            {
                UserName = "Admin"
            };

            var result = userManager.CreateAsync(adminUser, "password").Result;

            if(result.Succeeded){
                var admin = userManager.FindByNameAsync("Admin").Result;
                userManager.AddToRolesAsync(admin, new [] {"Admin", "Moderator"});

            }
            //Adds the Data to the Database
            // context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //code for creating passwordSalt and passwordHash
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}