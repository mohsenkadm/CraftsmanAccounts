// خدمة الإشعارات - جلب إشعارات المستخدم وتحديث حالة القراءة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    public NotificationService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<NotificationDto>>> GetByUserAsync(int userId, PagedRequest request)
    {
        var q = _uow.Repository<Notification>().Query()
            .Where(n => n.UserId == userId);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.Type, n.IsRead, n.CreatedAt))
            .ToListAsync();

        return ServiceResult<PagedResult<NotificationDto>>.Ok(new PagedResult<NotificationDto>
        {
            Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize
        });
    }

    public async Task<ServiceResult> MarkAsReadAsync(int userId, int notificationId)
    {
        var notification = await _uow.Repository<Notification>().Query()
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        if (notification == null) return ServiceResult.Fail("الإشعار غير موجود");
        notification.IsRead = true;
        _uow.Repository<Notification>().Update(notification);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تحديث حالة الإشعار");
    }

    public async Task<ServiceResult> MarkAllAsReadAsync(int userId)
    {
        var unread = await _uow.Repository<Notification>().FindAsync(n => n.UserId == userId && !n.IsRead);
        foreach (var n in unread)
        {
            n.IsRead = true;
            _uow.Repository<Notification>().Update(n);
        }
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تحديث جميع الإشعارات");
    }

    public async Task<ServiceResult<int>> GetUnreadCountAsync(int userId)
    {
        var count = await _uow.Repository<Notification>().Query()
            .CountAsync(n => n.UserId == userId && !n.IsRead);
        return ServiceResult<int>.Ok(count);
    }
}
