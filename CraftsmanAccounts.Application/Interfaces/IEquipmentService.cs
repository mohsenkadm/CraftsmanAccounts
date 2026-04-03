// واجهة خدمة المعدات - إدارة المعدات وتسجيل التلف وتتبع الكمية المتاحة وكشف المعدات
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IEquipmentService
{
    Task<ServiceResult<List<EquipmentDto>>> GetAllAsync(int userId);
    Task<ServiceResult<List<EquipmentDto>>> GetAvailableAsync(int userId);
    Task<ServiceResult<EquipmentDto>> CreateAsync(int userId, CreateEquipmentRequest request);
    Task<ServiceResult> MarkDamagedAsync(int userId, int id);
    Task<ServiceResult> DeleteAsync(int userId, int id);

    // كشف المعدات الشامل (كمي ومالي) مع فلاتر
    Task<ServiceResult<EquipmentStatementDto>> GetStatementAsync(int userId, EquipmentStatementRequest request);
}
