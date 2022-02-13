using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service.Hangfire
{
    public interface INotificationService
    {
        Task SendNotificationsAsync();
    }
}
