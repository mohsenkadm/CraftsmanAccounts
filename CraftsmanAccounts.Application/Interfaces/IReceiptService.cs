// واجهة خدمة سندات القبض - إنشاء سندات قبض عامة ولمشاريع
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IReceiptService
{
    Task<ServiceResult<PagedResult<ReceiptDto>>> GetAllAsync(int userId, AccountStatementRequest request);
    Task<ServiceResult<ReceiptDto>> CreateGeneralAsync(int userId, CreateReceiptGeneralRequest request);
    Task<ServiceResult<ReceiptDto>> CreateProjectAsync(int userId, CreateReceiptProjectRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
