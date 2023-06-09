﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using SapSecurity.Model;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Db;
using SapSecurity.ViewModel;

namespace SapSecurity.Admin.Controllers
{
    public class SensorDetailController : Controller
    {
        private readonly ISensorDetailService _sensorDetailService;
        private readonly ISensorGroupService _sensorGroupService;
        private readonly IZoneService _zoneService;

        public SensorDetailController(ISensorDetailService sensorDetailService, ISensorGroupService sensorGroupService, IZoneService zoneService)
        {
            _sensorDetailService = sensorDetailService;
            _sensorGroupService = sensorGroupService;
            _zoneService = zoneService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(SensorDetail sensorDetail)
        {
            await _sensorDetailService.Update(sensorDetail);

            await ClearCache();

            return RedirectToAction("Index", new { sensorDetailId = sensorDetail.Id });
        }
        public async Task<IActionResult> Index(int sensorDetailId)
        {
            ViewBag.Zones = await _zoneService.GetAllSelectList();
            ViewBag.Groups = await _sensorGroupService.GetAllSelectList();
            var sensor = await _sensorDetailService.GetByIdAsync(sensorDetailId);
            return View(sensor);
        }



        private async Task ClearCache()
        {
            try
            {
                var client = new HttpClient();
                await client.GetAsync("http://109.122.199.199:7080/ResetCache");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
