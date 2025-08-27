using BookShopping_Ecommerce.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShopping_Ecommerce.Data
{
    public class DbSeeder
    {
        public static async Task DbSeederData(IServiceProvider service)
        {
            try
            {
                var context = service.GetRequiredService<ApplicationDbContext>();

                // تأكد من وجود المهاجرات وطبقها
                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await context.Database.MigrateAsync();
                }

                var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
                var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

                // إنشاء Role: Admin إذا مش موجود
                if (!await roleMgr.RoleExistsAsync(Roles.Admin.ToString()))
                {
                    await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                }

                // إنشاء Role: User إذا مش موجود
                if (!await roleMgr.RoleExistsAsync(Roles.User.ToString()))
                {
                    await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
                }

                // إنشاء حساب أدمن إذا مش موجود
                var adminEmail = "admin@gmail.com";
                var adminUser = await userMgr.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var admin = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userMgr.CreateAsync(admin, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeder error: {ex.Message}");
            }
        }
    }
}
