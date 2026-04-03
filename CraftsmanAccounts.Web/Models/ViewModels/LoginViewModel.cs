// نموذج عرض تسجيل الدخول - بيانات تسجيل دخول المدير
using System.ComponentModel.DataAnnotations;

namespace CraftsmanAccounts.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    public string Password { get; set; } = string.Empty;
}
