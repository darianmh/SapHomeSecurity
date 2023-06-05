using Microsoft.AspNetCore.Mvc.Rendering;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface ISensorGroupService
{
    Task<PaginatedViewModel<SensorGroupViewModel>> GetAllAsync(int pageSize, int pageIndex, string userId);
    Task<List<SensorGroupViewModel>> GetAllAsync(string userId);
    Task<List<SelectListItem>> GetAllSelectList();
}