// كائنات نقل بيانات المشاريع - إنشاء وتعديل المشاريع وتعيين العمال وإسناد المعدات
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات المشروع
public record ProjectDto(int Id, string Name, decimal TotalAmount, decimal PaidAmount, string Address, DateTime? StartDate, DateTime? EndDate, bool IsActive, int ClientId, string ClientName, ClientType ClientType);

// عرض تفاصيل المشروع الكاملة مع جميع البيانات المرتبطة
public record ProjectDetailDto(
    int Id, string Name, decimal TotalAmount, decimal PaidAmount, string Address,
    DateTime? StartDate, DateTime? EndDate, bool IsActive,
    int ClientId, string ClientName, ClientType ClientType,
    List<ProjectWorkerDto> Workers,
    List<ProjectEquipmentDto> Equipments,
    List<PaymentDto> Payments,
    List<ReceiptDto> Receipts,
    List<WorkerDailyDto> WorkerDailies,
    List<AccountEntryDto> AccountEntries);

// طلب إنشاء مشروع جديد
public record CreateProjectRequest(string Name, decimal TotalAmount, string Address, int ClientId, DateTime? StartDate, DateTime? EndDate);

// طلب تحديث مشروع
public record UpdateProjectRequest(string Name, decimal TotalAmount, string Address, int ClientId, DateTime? StartDate, DateTime? EndDate, bool IsActive);

// طلب تعيين عمال للمشروع
public record AssignWorkersRequest(List<ProjectWorkerItem> Workers);

// عنصر عامل مشروع - معرف العامل والأجرة اليومية
public record ProjectWorkerItem(int WorkerId, decimal DailyRate);

// عرض بيانات عامل المشروع
public record ProjectWorkerDto(int Id, int WorkerId, string WorkerName, decimal DailyRate);
