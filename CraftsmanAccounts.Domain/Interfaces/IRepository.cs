// واجهة المستودع العام - توفر عمليات CRUD الأساسية لأي كيان
using System.Linq.Expressions;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    /// <summary>جلب كيان حسب المعرف</summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>جلب جميع الكيانات</summary>
    Task<List<T>> GetAllAsync();

    /// <summary>البحث عن كيانات بشرط معين</summary>
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>إنشاء استعلام قابل للتخصيص</summary>
    IQueryable<T> Query();

    /// <summary>إضافة كيان جديد</summary>
    Task AddAsync(T entity);

    /// <summary>تحديث كيان موجود</summary>
    void Update(T entity);

    /// <summary>حذف كيان</summary>
    void Remove(T entity);
}
