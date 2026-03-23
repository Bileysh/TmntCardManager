using Microsoft.AspNetCore.Identity;
using TmntCardManager.Models;

namespace TmntCardManager.Services;

public class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        if (await roleManager.FindByNameAsync("Admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
        }
        if (await roleManager.FindByNameAsync("User") == null)
        {
            await roleManager.CreateAsync(new IdentityRole<int>("User"));
        }

        var admin = await userManager.FindByEmailAsync("admin@admin.com");

        if (admin == null)
        {
            admin = new User 
            { 
                Email = "admin@admin.com", 
                UserName = "admin@admin.com", 
                EmailConfirmed = true 
            };
            IdentityResult result = await userManager.CreateAsync(admin, "123");
    
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
        else if (!admin.EmailConfirmed)
        {
            admin.EmailConfirmed = true;
            await userManager.UpdateAsync(admin);
        }
    }
}