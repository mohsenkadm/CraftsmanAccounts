// كائنات نقل بيانات المعدات - إضافة المعدات وتسجيل التلف وتتبع الكمية
using CraftsmanAccounts.Application.Common;

namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات المعدة مع الكمية المتاحة
public record EquipmentDto(int Id, string Name, string PurchasedFrom, decimal Amount, int Quantity, int AvailableQuantity, bool IsDamaged, DateTime CreatedAt);

// طلب إنشاء معدة جديدة مع الكمية
public record CreateEquipmentRequest(string Name, string PurchasedFrom, decimal Amount, int Quantity = 1);

// عرض معدة المشروع
public record ProjectEquipmentDto(int Id, int EquipmentId, string EquipmentName, int Quantity);

// طلب إسناد معدات للمشروع
public record AssignEquipmentRequest(List<ProjectEquipmentItem> Equipments);

// عنصر معدة مشروع - معرف المعدة والكمية
public record ProjectEquipmentItem(int EquipmentId, int Quantity);

// طلب كشف المعدات مع فلاتر
public class EquipmentStatementRequest : PagedRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? IsDamaged { get; set; }
    public int? ProjectId { get; set; }
}

// عنصر كشف المعدة مع التفاصيل الكمية والمالية
public record EquipmentStatementItemDto(
    int Id, string Name, string PurchasedFrom, decimal Amount,
    int Quantity, int AvailableQuantity, int AssignedQuantity,
    bool IsDamaged, DateTime CreatedAt);

// كشف المعدات الشامل - ملخص كمي ومالي مع قائمة مُقسّمة لصفحات
public record EquipmentStatementDto(
    int TotalEquipmentCount,
    int TotalQuantity,
    int AvailableQuantity,
    int AssignedQuantity,
    int DamagedCount,
    decimal TotalValue,
    decimal TotalDamagedValue,
    PagedResult<EquipmentStatementItemDto> Entries);
