// كائنات نقل بيانات سجل النشاطات
namespace CraftsmanAccounts.Application.DTOs;

public record ActivityLogDto(int Id, string Action, string EntityName, int? EntityId, string? Details, string? IpAddress, DateTime CreatedAt);

public class ActivityLogRequest : Common.PagedRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? EntityName { get; set; }
}
