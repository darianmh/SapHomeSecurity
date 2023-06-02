using Microsoft.AspNetCore.Mvc;
using SapSecurity.Admin.Models;
using System.Diagnostics;
using SapSecurity.Services.Db;

namespace SapSecurity.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IApplicationUserService _userService;
        private readonly ISensorDetailService _sensorDetailService;
        public HomeController(ILogger<HomeController> logger, IApplicationUserService userService, ISensorDetailService sensorDetailService)
        {
            _logger = logger;
            _userService = userService;
            _sensorDetailService = sensorDetailService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsers();
            return View(users);
        }



        public async Task<IActionResult> Sensors(string userId)
        {
            var sensors = await _sensorDetailService.GetAllSensors(userId);
            return View(sensors);
        }
    }
}