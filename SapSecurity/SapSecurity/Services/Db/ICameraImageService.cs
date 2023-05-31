using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface ICameraImageService
{
    Task SaveNew(string userId, string path);
    Task<List<CameraImageViewModel>> GetImages(string userId);
    Task<List<CameraImageViewModel>> LastGetImages(string userId);
}