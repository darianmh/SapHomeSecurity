using Microsoft.AspNetCore.Mvc;
using SapSecurity.Admin.Models;
using System.Diagnostics;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Db;
using SapSecurity.Services.Security;

namespace SapSecurity.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationUserService _userService;
        private readonly ISensorDetailService _sensorDetailService;
        private readonly ISecurityManager _securityManager;
        public HomeController(ILogger<HomeController> logger, IApplicationUserService userService, ISensorDetailService sensorDetailService, ISecurityManager securityManager)
        {
            _logger = logger;
            _userService = userService;
            _sensorDetailService = sensorDetailService;
            _securityManager = securityManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsers();
            return View(users);
        }

        public async Task<IActionResult> Alarm(int message)
        {
            CacheManager.SetSpecialMessage(3, message, false);
            return Ok();
        }
        public async Task<IActionResult> Spray()
        {
            CacheManager.SetSpecialMessage(5, 1, true);
            return Ok();
        }

        public async Task<IActionResult> HouseAlarm(AlertLevel alertLevel)
        {
            _securityManager.SoundAlertAsync("1", alertLevel);
            return Ok();
        }

        public async Task<IActionResult> Sensors(string userId)
        {
            ViewBag.UId = userId;
            var sensors = await _sensorDetailService.GetAllSensors(userId);
            return View(sensors);
        }
    }
}