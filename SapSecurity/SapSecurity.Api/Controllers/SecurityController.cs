using System.Drawing;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SapSecurity.Model.Types;
using SapSecurity.Services;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Connection;
using SapSecurity.Services.Connection.Music;
using SapSecurity.Services.Db;
using SapSecurity.Services.Security;
using SapSecurity.ViewModel;

namespace SapSecurity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController : ControllerBase
    {

        #region Fields

        private readonly ISensorGroupService _sensorGroupService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ISensorDetailService _sensorDetailService;
        private readonly IUserWebSocketManager _userWebSocketManager;
        private readonly IZoneService _zoneService;
        private readonly ISecurityManager _securityManager;
        private readonly ICameraImageService _cameraImageService;
        private readonly IConnectionHub _connectionHub;
        private readonly IMusicUserSocketManager _musicUserSocketManager;



        #endregion
        #region Methods
        [HttpGet("/start")]
        public IActionResult Start()
        {

            _connectionHub.Setup();
            _connectionHub.RunRegisterUserSocketAsync();
            _connectionHub.RunRegisterSensorLogSocketUdpAsync();
            _connectionHub.RunMusicSocketAsync();
            return Ok();
        }

        /// <summary>
        /// login user
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost("/login")]
        [ProducesResponseType(typeof(LoginResponseViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            var token = await _applicationUserService.LoginAsync(loginModel);
            if (token != null)
                return Ok(token);
            return Unauthorized();
        }
        /// <summary>
        /// active or de active each sensor
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="sensorState"></param>
        /// <returns></returns>
        [HttpPost("/sensor")]
        public async Task<IActionResult> Sensor(int sensorId, int sensorState)
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }

            var check = await _sensorDetailService.SetSensorState(sensorId, sensorState);
            if (check)
                return Ok();
            else
                return NotFound();
        }

        /// <summary>
        /// get active or de active status for security
        /// </summary>
        /// <returns></returns>
        [HttpGet("/activeStatus")]
        [ProducesResponseType(typeof(ActiveStatusViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ActiveStatus()
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }

            var check = await _applicationUserService.GetSecurityStatus(user);
            var model = new ActiveStatusViewModel { SecurityIsActive = check };
            return Ok(model);
        }

        /// <summary>
        /// active or de active entire security
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/activate")]
        public async Task<IActionResult> Activate(ActivateViewModel model)
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }
            if (model.SecurityState) Thread.Sleep(10000);
            await _applicationUserService.SetSecurityStatus(user, model.SecurityState);
            //send active status to user
            await _userWebSocketManager.SendMessage($"{model.SecurityState}", SocketMessageType.ANo, user);
            CacheManager.SetSecurityStatus(user, model.SecurityState ? SensorStatus.Active : SensorStatus.DeActive,
                true);
            CacheManager.ClearAllLogs(user);
            if (model.SecurityState)
            {
                await _musicUserSocketManager.SendMessage(1);
                _securityManager.RunSecurityTask(user);
            }
            else
            {
                await _musicUserSocketManager.SendMessage(2);
                _securityManager.StopSecurityTask(user);
            }

            CacheManager.ResetSpecialMessages(user);
            CacheManager.ResetZoneSensorValue(user);
            return Ok();
        }

        [HttpGet("/music")]
        public async Task<IActionResult> PlayMusic(int id)
        {
            await _musicUserSocketManager.SendMessage(id);
            return Ok();
        }

        /// <summary>
        /// active or de active door lock
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/lockActivate")]
        public Task<IActionResult> LockActivate(ActivateViewModel model)
        {

            if (model.SecurityState)
            {
                //door lock
                CacheManager.SetSpecialMessage("202", 0, false);
            }
            else
            {
                //dor lock
                CacheManager.SetSpecialMessage("202", 1, false);
            }

            return Task.FromResult<IActionResult>(Ok());
        }


        /// <summary>
        /// status of door lock
        /// </summary>
        /// <returns></returns>
        [HttpGet("/LockStatus")]
        public Task<IActionResult> LockStatus()
        {
            var lastMessage = CacheManager.GetSensorLastMessage("202");
            if (lastMessage == null || lastMessage == 1) return Task.FromResult<IActionResult>(Ok(false));
            return Task.FromResult<IActionResult>(Ok(true));
        }

        /// <summary>
        /// return sensor status and sensor groups
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("/groups")]
        [ProducesResponseType(typeof(PaginatedViewModel<SensorGroupViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Groups(int pageSize, int pageIndex)
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _sensorGroupService.GetAllAsync(pageSize, pageIndex, user);
            return Ok(result);
        }


        /// <summary>
        /// return zones
        /// </summary>
        /// <returns></returns>
        [HttpGet("/zones")]
        [ProducesResponseType(typeof(List<ZoneViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Zones()
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _zoneService.GetUserZonesAsync(user);
            return Ok(result);
        }

        /// <summary>
        /// return zones
        /// </summary>
        /// <returns></returns>
        [HttpGet("/sensors/{zoneId}")]
        [ProducesResponseType(typeof(List<SensorViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Sensors(int zoneId)
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _sensorDetailService.GetAllSensors(zoneId);
            return Ok(result);
        }


        [HttpGet("/webSocket")]
        public async Task GetMessage(CancellationToken token)
        {

            await _userWebSocketManager.SetupConnectionAsync(ControllerContext, token);
            // ReSharper disable once FunctionNeverReturns
        }


        [HttpPost("/image")]
        public async Task<IActionResult> ImageUpload()
        {
            var images = Request.Form.Files;
            var basePath = "C:/publish/Content/";
            var folderPath = "Images";
            var path = basePath + folderPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            foreach (var image in images)
            {
                var name = $"{DateTime.Now:HH_mm_ss_ffff}_{image.Name}.jpg";
                var imagePath = Path.Combine(path, name);
                await using FileStream fs = System.IO.File.Create(imagePath);
                var stream = image.OpenReadStream();
                await stream.CopyToAsync(fs);


                await _cameraImageService.SaveNew("1", Path.Combine(folderPath, name));
            }
            //play alert sound
            CacheManager.SetSpecialMessage("200", 200, true);
            if (await _applicationUserService.GetSecurityStatus("1"))
                _securityManager.SoundAlertAsync("1", AlertLevel.Medium);
            return Ok();
        }

        [HttpGet("/video")]
        [ProducesResponseType(typeof(List<CameraImageViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LastVideo()
        {
            var result = new List<CameraImageViewModel>();
            try
            {
                var path = "C:/publish/Content/video";
                var files = Directory.GetFiles(path);
                if (files == null)
                {
                    Console.WriteLine("No file");
                    return Ok(result);
                }
                var lastAlarm = CacheManager.AlertDate;
                var count = 0;
                files = files.OrderByDescending(x => x).ToArray();
                Console.WriteLine($"files {files.Length}");
                foreach (var file in files)
                {
                    try
                    {
                        var name = file.Split('/').Last();
                        name = name.Split('\\').Last();
                        name = name.Replace("_file.mp4", String.Empty);
                        var date = DateTime.ParseExact(name, "HH_mm_ss_ffff", CultureInfo.InvariantCulture);
                        Console.WriteLine($"date: {date}");
                        var pathVid = file.Replace("C:/publish/Content", "http://109.122.199.199:8090");
                        pathVid = pathVid.Replace("\\\\", "/");
                        var lastDate = DateTime.Now.AddSeconds(-60);
                        if (lastAlarm == null)
                        {
                            if (date > lastDate)
                            {
                                result.Add(new CameraImageViewModel()
                                {
                                    DateTime = date,
                                    Path = pathVid,
                                    Id = count++
                                });
                            }
                        }
                        else
                        {
                            if (date > lastAlarm.Value.AddSeconds(-30) && date < lastAlarm.Value.AddSeconds(30))
                            {
                                result.Add(new CameraImageViewModel()
                                {
                                    DateTime = date,
                                    Path = pathVid,
                                    Id = count++
                                });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Ok(result);
        }
        [HttpGet("/image")]
        [ProducesResponseType(typeof(List<CameraImageViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LastImages()
        {
            var user = _applicationUserService.GetUserId(GetToken());
            if (user == null)
            {
                return Unauthorized();
            }
            var images = await _cameraImageService.LastGetImages(user);
            return Ok(images);
        }

        [HttpGet("/soundAlert")]
        public Task<IActionResult> SoundAlert(string userId, int alertLevel)
        {
            _securityManager.SoundAlertAsync(userId, (AlertLevel)alertLevel);
            return Task.FromResult<IActionResult>(Ok());
        }




        [HttpGet("/Alarm")]
        public async Task<IActionResult> Alarm(int message)
        {
            CacheManager.SetSpecialMessage(3, message, false);
            return Ok();
        }
        [HttpGet("/Spray")]
        public async Task<IActionResult> Spray()
        {
            CacheManager.SetSpecialMessage(5, 0, true);
            return Ok();
        }

        [HttpGet("/HouseAlarm")]
        public async Task<IActionResult> HouseAlarm(AlertLevel alertLevel)
        {
            _securityManager.SoundAlertAsync("1", alertLevel);
            return Ok();
        }

        [HttpGet("ResetCache")]
        public IActionResult ResetCache()
        {
            CacheManager.ClearCaches();
            return Ok();
        }
        #endregion
        #region Utilities

        private string GetToken()
        {
            var check = HttpContext.Request.Headers.TryGetValue("Auth", out var token);
            if (check) return token.ToString();
            return String.Empty;
        }

        #endregion
        #region Ctor

        public SecurityController(ISensorGroupService sensorGroupService, IApplicationUserService applicationUserService, ISensorDetailService sensorDetailService, IUserWebSocketManager userWebSocketManager, IZoneService zoneService, ISecurityManager securityManager, ICameraImageService cameraImageService, IConnectionHub connectionHub, IMusicUserSocketManager musicUserSocketManager)
        {
            _sensorGroupService = sensorGroupService;
            _applicationUserService = applicationUserService;
            _sensorDetailService = sensorDetailService;
            _userWebSocketManager = userWebSocketManager;
            _zoneService = zoneService;
            _securityManager = securityManager;
            _cameraImageService = cameraImageService;
            _connectionHub = connectionHub;
            _musicUserSocketManager = musicUserSocketManager;
        }



        #endregion
    }
}