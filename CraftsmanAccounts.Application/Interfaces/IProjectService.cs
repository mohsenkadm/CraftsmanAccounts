// واجهة خدمة المشاريع - إدارة المشاريع وتعيين العمال وإسناد المعدات
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IProjectService
{
    Task<ServiceResult<List<ProjectDto>>> GetAllAsync(int userId);
    Task<ServiceResult<ProjectDetailDto>> GetByIdAsync(int userId, int id);
    Task<ServiceResult<ProjectDto>> CreateAsync(int userId, CreateProjectRequest request);
    Task<ServiceResult<ProjectDto>> UpdateAsync(int userId, int id, UpdateProjectRequest request);
    Task<ServiceResult> DeleteAsync(int userId, int id);
    Task<ServiceResult<List<ProjectWorkerDto>>> GetProjectWorkersAsync(int userId, int projectId);
    Task<ServiceResult> AssignWorkersAsync(int userId, int projectId, AssignWorkersRequest request);
    Task<ServiceResult<List<ProjectEquipmentDto>>> GetProjectEquipmentAsync(int userId, int projectId);
    Task<ServiceResult> AssignEquipmentAsync(int userId, int projectId, AssignEquipmentRequest request);
    Task<ServiceResult> ReleaseEquipmentAsync(int userId, int projectId);
}
