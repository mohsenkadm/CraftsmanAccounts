// واجهة خدمة اليوميات - سجل يومي لأجور العمال ودفعها
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IWorkerDailyService
{
    Task<ServiceResult<PagedResult<WorkerDailyDto>>> GetAllAsync(int userId, int? projectId, AccountStatementRequest request);
    Task<ServiceResult<WorkerDailyDto>> CreateAsync(int userId, CreateWorkerDailyRequest request);
    Task<ServiceResult> PayBatchAsync(int userId, List<int> workerDailyIds, int walletId);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
