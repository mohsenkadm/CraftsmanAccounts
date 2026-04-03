// كائنات نقل بيانات أنواع المصروفات
namespace CraftsmanAccounts.Application.DTOs;

// عرض نوع المصروف
public record ExpenseTypeDto(int Id, string Name);

// طلب إنشاء نوع مصروف جديد
public record CreateExpenseTypeRequest(string Name);
