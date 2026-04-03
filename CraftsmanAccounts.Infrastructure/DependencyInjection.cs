// تسجيل خدمات طبقة البنية التحتية - قاعدة البيانات ووحدة العمل وخدمة إرسال OTP
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Interfaces;
using CraftsmanAccounts.Infrastructure.Data;
using CraftsmanAccounts.Infrastructure.Repositories;
using CraftsmanAccounts.Infrastructure.Services;

namespace CraftsmanAccounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // تسجيل خدمة إرسال رمز التحقق عبر واتساب
        services.AddHttpClient<IOtpSenderService, OtpSenderService>(client =>
        {
            client.BaseAddress = new Uri(configuration["SmsGateway:BaseUrl"] ?? "https://gateway.standingtech.com");
            var apiKey = configuration["SmsGateway:ApiKey"] ?? "";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
