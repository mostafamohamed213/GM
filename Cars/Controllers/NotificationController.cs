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

        public async Task Index()
        {
            try
            {
                await _service.AddAndSendNotificationAsnc(new List<string>() { "a5acd440-f240-49b4-8954-f9bab39921c2" }, "Add New Order", "Add Order");
            }
            catch (Exception)
            {
                return;
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
