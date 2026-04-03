// خدمة العمال - إضافة وتعديل وحذف وعرض العمال
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class WorkerService : IWorkerService
{
    private readonly IUnitOfWork _uow;
    public WorkerService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<WorkerDto>>> GetAllAsync(int userId)
    {
        var workers = await _uow.Repository<Worker>().Query()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkerDto(w.Id, w.Name, w.Address, w.PhoneNumber, w.IsActive))
            .ToListAsync();
        return ServiceResult<List<WorkerDto>>.Ok(workers);
    }

    public async Task<ServiceResult<WorkerDto>> GetByIdAsync(int userId, int id)
    {
        var w = await _uow.Repository<Worker>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (w == null) return ServiceResult<WorkerDto>.Fail("العامل غير موجود");
        return ServiceResult<WorkerDto>.Ok(new WorkerDto(w.Id, w.Name, w.Address, w.PhoneNumber, w.IsActive));
    }

    public async Task<ServiceResult<WorkerDto>> CreateAsync(int userId, CreateWorkerRequest request)
    {
        var worker = new Worker { UserId = userId, Name = request.Name, Address = request.Address, PhoneNumber = request.PhoneNumber };
        await _uow.Repository<Worker>().AddAsync(worker);
        await _uow.SaveChangesAsync();
        return ServiceResult<WorkerDto>.Ok(new WorkerDto(worker.Id, worker.Name, worker.Address, worker.PhoneNumber, worker.IsActive));
    }

    public async Task<ServiceResult<WorkerDto>> UpdateAsync(int userId, int id, UpdateWorkerRequest request)
    {
        var w = await _uow.Repository<Worker>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (w == null) return ServiceResult<WorkerDto>.Fail("العامل غير موجود");
        w.Name = request.Name; w.Address = request.Address; w.PhoneNumber = request.PhoneNumber; w.IsActive = request.IsActive;
        _uow.Repository<Worker>().Update(w);
        await _uow.SaveChangesAsync();
        return ServiceResult<WorkerDto>.Ok(new WorkerDto(w.Id, w.Name, w.Address, w.PhoneNumber, w.IsActive));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var w = await _uow.Repository<Worker>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (w == null) return ServiceResult.Fail("العامل غير موجود");
        _uow.Repository<Worker>().Remove(w);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
