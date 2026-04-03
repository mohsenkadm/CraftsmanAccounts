// واجهة خدمة سندات الصرف - إنشاء مدفوعات متنوعة (عامة، مشروع، عميل، عامل)
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IPaymentService
{
    Task<ServiceResult<PagedResult<PaymentDto>>> GetAllAsync(int userId, AccountStatementRequest request);
    Task<ServiceResult<PaymentDto>> CreateGeneralAsync(int userId, CreatePaymentGeneralRequest request);
    Task<ServiceResult<PaymentDto>> CreateProjectAsync(int userId, CreatePaymentProjectRequest request);
    Task<ServiceResult<PaymentDto>> CreateClientAsync(int userId, CreatePaymentClientRequest request);
    Task<ServiceResult<PaymentDto>> CreateWorkerAsync(int userId, CreatePaymentWorkerRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
