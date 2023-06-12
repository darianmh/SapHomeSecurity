using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Services.Mapper;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public class CameraImageService : ICameraImageService
{

    #region Fields

    private readonly ICameraImageRepository _cameraImageRepository;
    private readonly IMapper _mapper;

    #endregion
    #region Methods


    public async Task SaveNew(string userId, string path)
    {
        var image = new CameraImage()
        {
            UserId = userId,
            DateTimeUtc = DateTime.UtcNow,
            Path = path,
        };
        await _cameraImageRepository.InsertAsync(image);
        await _cameraImageRepository.SaveChangeAsync();
    }

    public async Task<List<CameraImageViewModel>> GetImages(string userId)
    {
        var images = await _cameraImageRepository.GetAllAsync(30, 0);
        return images.Data.Select(x => _mapper.Map(x)).ToList();
    }

    public async Task<List<CameraImageViewModel>> LastGetImages(string userId)
    {
        var images = await _cameraImageRepository.GetAllAsync(300, 0);
        if (images.Count == 0) return new List<CameraImageViewModel>();
        var toSend = new List<CameraImage>();
        var lastDate = images.Data.First().DateTimeUtc;
        foreach (var image in images.Data)
        {
            if (lastDate <= image.DateTimeUtc.AddMinutes(1))
            {
                lastDate = image.DateTimeUtc;
                toSend.Add(image);
            }
            else
            {
                break;
            }
        }

        //toSend = toSend.OrderBy(x => x.Id).ToList();
        return toSend.Select(x => _mapper.Map(x)).ToList();
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public CameraImageService(IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _cameraImageRepository = scope.ServiceProvider.GetService<ICameraImageRepository>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
    }


    #endregion

}