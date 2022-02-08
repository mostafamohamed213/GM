using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService _service;
        private readonly NotificationUserService _notificationUserService;
        public NotificationController(NotificationService service, NotificationUserService notificationUserService)
        {
            _service = service;
            _notificationUserService = notificationUserService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await _service.AddAndSendNotificationAsnc(new List<string>() { "42bc920c-9361-4bef-8356-0c208322a16a" }, "new Notification", "Add addaulkfgsal");
                return RedirectToAction("Index", "Finance");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> GetTopTen()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _notificationUserService.GetAllByUserIDAsync(userId);
            return PartialView();

        }
    }
}
