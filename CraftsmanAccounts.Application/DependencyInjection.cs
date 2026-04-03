// تسجيل خدمات طبقة التطبيق - جميع الخدمات الأساسية والإدارية والمالية
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CraftsmanAccounts.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IReceiptService, ReceiptService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
        services.AddScoped<IWorkerDailyService, WorkerDailyService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IAccountEntryService, AccountEntryService>();
        services.AddScoped<ISubscriptionTypeService, SubscriptionTypeService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IUserSubscriptionManagementService, UserSubscriptionManagementService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        return services;
    }
}
