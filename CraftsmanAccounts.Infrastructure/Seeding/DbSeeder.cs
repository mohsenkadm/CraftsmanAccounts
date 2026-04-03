// بذر قاعدة البيانات - إنشاء بيانات أولية للمديرين وأنواع الاشتراكات
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Infrastructure.Data;

namespace CraftsmanAccounts.Infrastructure.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        if (!await context.Admins.AnyAsync())
        {
            context.Admins.AddRange(
                new Admin { Username = "admin", PasswordHash = HashPassword("admin123"), DisplayName = "المدير العام" },
                new Admin { Username = "moderator", PasswordHash = HashPassword("mod123"), DisplayName = "المشرف" }
            );
        }

        if (!await context.SubscriptionTypes.AnyAsync())
        {
            context.SubscriptionTypes.AddRange(
                new SubscriptionType { Name = "الاشتراك الشهري", Amount = 25000, DurationInDays = 30, Details = "اشتراك شهري يتضمن جميع الميزات الأساسية", IsActive = true },
                new SubscriptionType { Name = "الاشتراك ربع السنوي", Amount = 60000, DurationInDays = 90, Details = "اشتراك لمدة ثلاثة أشهر بسعر مخفض", IsActive = true },
                new SubscriptionType { Name = "الاشتراك نصف السنوي", Amount = 100000, DurationInDays = 180, Details = "اشتراك لمدة ستة أشهر مع خصم كبير", IsActive = true },
                new SubscriptionType { Name = "الاشتراك السنوي", Amount = 180000, DurationInDays = 365, Details = "اشتراك سنوي بأفضل سعر مع جميع الميزات", IsActive = true }
            );
        }

        await context.SaveChangesAsync();
    }

    // Simple hash for seeding - in production use a proper hasher
    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
