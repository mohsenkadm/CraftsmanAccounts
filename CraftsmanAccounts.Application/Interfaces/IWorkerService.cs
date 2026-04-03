// واجهة خدمة العمال - إدارة العمال (إضافة، تعديل، حذف، عرض)
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IWorkerService
{
    Task<ServiceResult<List<WorkerDto>>> GetAllAsync(int userId);
    Task<ServiceResult<WorkerDto>> GetByIdAsync(int userId, int id);
    Task<ServiceResult<WorkerDto>> CreateAsync(int userId, CreateWorkerRequest request);
    Task<ServiceResult<WorkerDto>> UpdateAsync(int userId, int id, UpdateWorkerRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
