// واجهة خدمة أنواع المصروفات - تصنيفات المصاريف الخاصة بكل مستخدم
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IExpenseTypeService
{
    Task<ServiceResult<List<ExpenseTypeDto>>> GetAllAsync(int userId);
    Task<ServiceResult<ExpenseTypeDto>> CreateAsync(int userId, CreateExpenseTypeRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
