using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopApplication_Models;

namespace ShopApplication_DataAccess.Data;

public class ContextDb : IdentityDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ApplicationType> ApplicationTypes { get; set; }

    public DbSet<Category> Category { get; set; }

    public DbSet<OrderHeader> OrderHeader { get; set; }

    public DbSet<OrderDetail> OrderDetail { get; set; }

    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
    }
}