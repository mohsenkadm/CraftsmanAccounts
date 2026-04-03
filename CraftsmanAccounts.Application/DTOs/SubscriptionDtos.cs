// كائنات نقل بيانات الاشتراكات - أنواع الاشتراكات وإدارتها
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.DTOs;

// عرض نوع اشتراك
public record SubscriptionTypeDto(int Id, string Name, decimal Amount, int DurationInDays, string Details, bool IsActive);

// طلب إنشاء نوع اشتراك جديد
public record CreateSubscriptionTypeRequest(string Name, decimal Amount, int DurationInDays, string Details, bool IsActive = true);

// طلب تحديث نوع اشتراك
public record UpdateSubscriptionTypeRequest(string Name, decimal Amount, int DurationInDays, string Details, bool IsActive);

// عرض اشتراك مستخدم
public record UserSubscriptionDto(int Id, int UserId, string UserName, int SubscriptionTypeId, string SubscriptionTypeName, decimal Amount, DateTime StartDate, DateTime EndDate, SubscriptionStatus Status, bool IsPaid);

// طلب إنشاء اشتراك مستخدم
public record CreateUserSubscriptionRequest(int SubscriptionTypeId);
