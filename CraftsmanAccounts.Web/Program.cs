// نقطة بداية مشروع الواجهة الأمامية - لوحة تحكم المدير مع مصادقة الكوكيز والإشعارات الفورية
using Microsoft.AspNetCore.Authentication.Cookies;
using CraftsmanAccounts.Infrastructure;
using CraftsmanAccounts.Infrastructure.Seeding;
using CraftsmanAccounts.Web.Filters;
using CraftsmanAccounts.Web.Hubs;
using CraftsmanAccounts.Web.Services.Db;
using CraftsmanAccounts.Web.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// إضافة الخدمات إلى الحاوية
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PendingCountFilter>();
});
builder.Services.AddSignalR(); // إضافة خدمة الإشعارات الفورية

// البنية التحتية (قاعدة البيانات + وحدة العمل)
builder.Services.AddInfrastructure(builder.Configuration);

// إعداد المصادقة بالكوكيز
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// تسجيل الخدمات المدعومة بقاعدة البيانات
builder.Services.AddScoped<IAdminService, DbAdminService>();
builder.Services.AddScoped<IUserService, DbUserService>();
builder.Services.AddScoped<ISubscriptionTypeService, DbSubscriptionTypeService>();
builder.Services.AddScoped<IUserSubscriptionService, DbUserSubscriptionService>();

var app = builder.Build();

// تهيئة وبذر قاعدة البيانات
await DbSeeder.SeedAsync(app.Services);

// إعداد مسار معالجة الطلبات
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
