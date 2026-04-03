// واجهة خدمة القيود المحاسبية - كشوفات العمال والعملاء والمشاريع وتقرير الأرباح والخسائر
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IAccountEntryService
{
    Task<ServiceResult<AccountStatementDto>> GetWorkerStatementAsync(int userId, int workerId, AccountStatementRequest request);
    Task<ServiceResult<AccountStatementDto>> GetClientStatementAsync(int userId, int clientId, AccountStatementRequest request);
    Task<ServiceResult<AccountStatementDto>> GetProjectStatementAsync(int userId, int projectId, AccountStatementRequest request);
    Task<ServiceResult<AccountStatementDto>> GetAllStatementAsync(int userId, AccountStatementRequest request);
    Task<ServiceResult<ProfitLossDto>> GetProfitLossAsync(int userId, DateTime? fromDate, DateTime? toDate);
    Task<ServiceResult<DailyMovementsDto>> GetDailyMovementsAsync(int userId, DateTime? date);
}
