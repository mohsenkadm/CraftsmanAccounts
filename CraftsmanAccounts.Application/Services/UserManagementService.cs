// خدمة إدارة المستخدمين - الموافقة والرفض وتفعيل/تعطيل حسابات المستخدمين
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class UserManagementService : IUserManagementService
{
    private readonly IUnitOfWork _uow;
    public UserManagementService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<AppUserDto>>> GetAllUsersAsync(PagedRequest request, ApprovalStatus? statusFilter = null)
    {
        var q = _uow.Repository<AppUser>().Query().AsQueryable();
        if (statusFilter.HasValue) q = q.Where(u => u.ApprovalStatus == statusFilter.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            q = q.Where(u => u.FullName.Contains(request.SearchTerm) || u.PhoneNumber.Contains(request.SearchTerm));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(u => u.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(u => new AppUserDto(u.Id, u.FullName, u.Address, u.PhoneNumber, u.IsActive, u.ApprovalStatus, u.CreatedAt))
            .ToListAsync();

        return ServiceResult<PagedResult<AppUserDto>>.Ok(new PagedResult<AppUserDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }

    public async Task<ServiceResult<AppUserDto>> GetUserByIdAsync(int id)
    {
        var u = await _uow.Repository<AppUser>().GetByIdAsync(id);
        if (u == null) return ServiceResult<AppUserDto>.Fail("المستخدم غير موجود");
        return ServiceResult<AppUserDto>.Ok(new AppUserDto(u.Id, u.FullName, u.Address, u.PhoneNumber, u.IsActive, u.ApprovalStatus, u.CreatedAt));
    }

    public async Task<ServiceResult> ApproveUserAsync(int id)
    {
        var u = await _uow.Repository<AppUser>().GetByIdAsync(id);
        if (u == null) return ServiceResult.Fail("المستخدم غير موجود");
        u.ApprovalStatus = ApprovalStatus.Approved;
        u.IsActive = true;
        _uow.Repository<AppUser>().Update(u);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تمت الموافقة على المستخدم");
    }

    public async Task<ServiceResult> RejectUserAsync(int id)
    {
        var u = await _uow.Repository<AppUser>().GetByIdAsync(id);
        if (u == null) return ServiceResult.Fail("المستخدم غير موجود");
        u.ApprovalStatus = ApprovalStatus.Rejected;
        _uow.Repository<AppUser>().Update(u);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم رفض المستخدم");
    }

    public async Task<ServiceResult> ToggleUserActiveAsync(int id)
    {
        var u = await _uow.Repository<AppUser>().GetByIdAsync(id);
        if (u == null) return ServiceResult.Fail("المستخدم غير موجود");
        u.IsActive = !u.IsActive;
        _uow.Repository<AppUser>().Update(u);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok(u.IsActive ? "تم تفعيل المستخدم" : "تم تعطيل المستخدم");
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _uow.Repository<AppUser>().Query().CountAsync(u => u.ApprovalStatus == ApprovalStatus.Pending);
    }
}
