using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopApplication_DataAccess.Data;
using ShopApplication_Models;
using ShopApplication_Utility;

namespace ShopApplication_DataAccess.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ContextDb _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public DbInitializer(ContextDb context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public void Initialize()
    {
        try
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        // adding roles if there isn't admin role
        // getAwaiter.GerResult it's like await in async method
        if (!_roleManager.RoleExistsAsync(Constants.AdminRole).GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new IdentityRole(Constants.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constants.UserRole)).GetAwaiter().GetResult();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Role already exists!");
            Console.ResetColor();
            return;
        }

        // adding the first user as Admin
        _userManager.CreateAsync(new ApplicationUser()
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            EmailConfirmed = true,
            FullName = "Admin Tester",
            PhoneNumber = "+48111111111",
        }, "Qwerty123$").GetAwaiter().GetResult();

        // get user from db and add admin role for him
        var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@admin.com");
        if (user != null) _userManager.AddToRoleAsync(user, Constants.AdminRole).GetAwaiter().GetResult();
    }
}