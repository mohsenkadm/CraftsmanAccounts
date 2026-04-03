// خدمة المحافظ المالية - إنشاء وعرض وحذف المحافظ
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _uow;
    public WalletService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<WalletDto>>> GetAllAsync(int userId)
    {
        var wallets = await _uow.Repository<Wallet>().Query()
            .Where(w => w.UserId == userId)
            .Select(w => new WalletDto(w.Id, w.Name, w.Balance))
            .ToListAsync();
        return ServiceResult<List<WalletDto>>.Ok(wallets);
    }

    public async Task<ServiceResult<WalletDto>> CreateAsync(int userId, CreateWalletRequest request)
    {
        var wallet = new Wallet { UserId = userId, Name = request.Name, Balance = request.InitialBalance };
        await _uow.Repository<Wallet>().AddAsync(wallet);
        await _uow.SaveChangesAsync();
        return ServiceResult<WalletDto>.Ok(new WalletDto(wallet.Id, wallet.Name, wallet.Balance));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var w = await _uow.Repository<Wallet>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (w == null) return ServiceResult.Fail("المحفظة غير موجودة");
        _uow.Repository<Wallet>().Remove(w);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
