// كائنات نقل بيانات المستخدمين - عرض وتحديث بيانات المستخدمين
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات المستخدم
public record AppUserDto(int Id, string FullName, string Address, string PhoneNumber, bool IsActive, ApprovalStatus ApprovalStatus, DateTime CreatedAt);

// طلب تحديث بيانات المستخدم
public record UpdateUserRequest(string FullName, string Address, string PhoneNumber, bool IsActive);
