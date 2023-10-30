using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.data
{
    public class Seed{
      

        public static async Task SeedUsers(UserManager<Appuser> userManager, RoleManager<AppRole> roleManager){
            if(await userManager.Users.AnyAsync()) return;

            var UserData = await File.ReadAllTextAsync("data/UserSeedData.json");

            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            var users = JsonSerializer.Deserialize<List<Appuser>>(UserData);

            var roles = new List<AppRole>{
                new AppRole{Name="Member"},
                new AppRole{Name="Admin"},
                new AppRole{Name="Moderator"}

            };

            foreach(var role in roles){
                await roleManager.CreateAsync(role);
            }
            foreach(var user in users){
                

                user.UserName = user.UserName.ToLower();

    

                await userManager.CreateAsync(user, "Pa$$w0rd");

                await userManager.AddToRoleAsync(user, "Member");


            }

            var admin = new Appuser{
                UserName="admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddToRoleAsync(admin, "Moderator");



        }

    }
}