using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using ShopApplication_DataAccess.Data;
using ShopApplication_DataAccess.Data.Repository;
using ShopApplication_DataAccess.Data.Repository.IRepository;
using ShopApplication_DataAccess.Data.Services;
using ShopApplication_DataAccess.Data.Services.IServices;
using ShopApplication_DataAccess.Initializer;
using ShopApplication_Utility;
using ShopApplication_Utility.BrainTree;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add DI data from appsettings.json for braintree
builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("Braintree"));

// add DI for BrainTreeGate
builder.Services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

// add DI for Db
builder.Services.AddDbContext<ContextDb>(options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

// add repository for every table in Db
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

// add services
builder.Services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// add DI for Initialize
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// add email sender DI
builder.Services.AddScoped<IEmailSender, EmailSenderSMTP>();

// add identification for user and roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>().
    AddDefaultTokenProviders().
    AddDefaultUI().
    AddEntityFrameworkStores<ContextDb>();

// add facebook authentication
builder.Services.AddAuthentication().AddFacebook(options =>
{
    var configuration = builder.Configuration;
    options.AppId = configuration["Authentication:Facebook:AppId"] ?? string.Empty;
    options.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? string.Empty;
});

// Logging information in console and file
builder.Host.UseSerilog((context, configuration) => // where we are writing logs.
{
    configuration
        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} | {Level:u} | {SourceContext} | {Message}{NewLine}{Exception}")
        .WriteTo.File(new CompactJsonFormatter() ,"logs/log.txt", rollingInterval: RollingInterval.Day);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// use swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// using initialize when starting app
// adding before UseRouting, because first HTTP request can start before Db will be ready
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();