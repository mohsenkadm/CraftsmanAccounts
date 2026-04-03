// خدمة العملاء - إدارة العملاء مع فلترة حسب النوع
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _uow;
    public ClientService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<ClientDto>>> GetAllAsync(int userId, ClientType? typeFilter = null)
    {
        var q = _uow.Repository<Client>().Query().Where(c => c.UserId == userId);
        if (typeFilter.HasValue) q = q.Where(c => c.ClientType == typeFilter.Value);
        var clients = await q.OrderByDescending(c => c.CreatedAt)
            .Select(c => new ClientDto(c.Id, c.Name, c.PhoneNumber, c.Address, c.ClientType, c.IsActive))
            .ToListAsync();
        return ServiceResult<List<ClientDto>>.Ok(clients);
    }

    public async Task<ServiceResult<ClientDto>> GetByIdAsync(int userId, int id)
    {
        var c = await _uow.Repository<Client>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (c == null) return ServiceResult<ClientDto>.Fail("العميل غير موجود");
        return ServiceResult<ClientDto>.Ok(new ClientDto(c.Id, c.Name, c.PhoneNumber, c.Address, c.ClientType, c.IsActive));
    }

    public async Task<ServiceResult<ClientDto>> CreateAsync(int userId, CreateClientRequest request)
    {
        var client = new Client { UserId = userId, Name = request.Name, PhoneNumber = request.PhoneNumber, Address = request.Address, ClientType = request.ClientType };
        await _uow.Repository<Client>().AddAsync(client);
        await _uow.SaveChangesAsync();
        return ServiceResult<ClientDto>.Ok(new ClientDto(client.Id, client.Name, client.PhoneNumber, client.Address, client.ClientType, client.IsActive));
    }

    public async Task<ServiceResult<ClientDto>> UpdateAsync(int userId, int id, UpdateClientRequest request)
    {
        var c = await _uow.Repository<Client>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (c == null) return ServiceResult<ClientDto>.Fail("العميل غير موجود");
        c.Name = request.Name; c.PhoneNumber = request.PhoneNumber; c.Address = request.Address; c.ClientType = request.ClientType; c.IsActive = request.IsActive;
        _uow.Repository<Client>().Update(c);
        await _uow.SaveChangesAsync();
        return ServiceResult<ClientDto>.Ok(new ClientDto(c.Id, c.Name, c.PhoneNumber, c.Address, c.ClientType, c.IsActive));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var c = await _uow.Repository<Client>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (c == null) return ServiceResult.Fail("العميل غير موجود");
        _uow.Repository<Client>().Remove(c);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
