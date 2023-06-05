using Microsoft.AspNetCore.Mvc.Rendering;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface IZoneService
{
    Task<List<ZoneViewModel>> GetUserZonesAsync(string userId);
    Task<List<SelectListItem>> GetAllSelectList();
}