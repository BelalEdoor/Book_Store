using BookShopping_Ecommerce.Data;
using BookShopping_Ecommerce.Repositories;
using BookShopping_Ecommerce.Shared;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Identity («Œ «— Ê«Õœ ›ﬁÿ)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IHomeRepository, HomeRepostiry>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<IUserOrderRepository, UserOrderRepository>();
builder.Services.AddTransient<IStockRepository, StockRepository >();
builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<IReportRepository, ReportRepository>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// «” œ⁄«¡ Seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // «” œ⁄«¡ «·œ« « ”ÌÌœ— „⁄ await
    await DbSeeder.DbSeederData(services);
}

app.Run();
