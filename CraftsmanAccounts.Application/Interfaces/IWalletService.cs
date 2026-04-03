// واجهة خدمة المحافظ المالية - إنشاء وعرض المحافظ
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IWalletService
{
    Task<ServiceResult<List<WalletDto>>> GetAllAsync(int userId);
    Task<ServiceResult<WalletDto>> CreateAsync(int userId, CreateWalletRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
