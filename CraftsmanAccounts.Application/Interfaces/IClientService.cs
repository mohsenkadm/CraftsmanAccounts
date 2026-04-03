// واجهة خدمة العملاء - إدارة العملاء مع فلترة حسب النوع
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IClientService
{
    Task<ServiceResult<List<ClientDto>>> GetAllAsync(int userId, ClientType? typeFilter = null);
    Task<ServiceResult<ClientDto>> GetByIdAsync(int userId, int id);
    Task<ServiceResult<ClientDto>> CreateAsync(int userId, CreateClientRequest request);
    Task<ServiceResult<ClientDto>> UpdateAsync(int userId, int id, UpdateClientRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
}
