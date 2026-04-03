// واجهة وحدة العمل - تدير المعاملات وتوفر الوصول للمستودعات
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Domain.Interfaces;

public interface IUnitOfWork
{
    /// <summary>الحصول على مستودع لكيان معين</summary>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>حفظ جميع التغييرات في قاعدة البيانات</summary>
    Task<int> SaveChangesAsync();
}
