using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service.Email
{
    public interface IEmailService
    {
        void SendEmailForAddedOrderDetails(string orderPrefix, string to);
        void SendEmailForDelayedOrderDetails(string orderPrefix, string teamName, string to);
        void SendEmailForRejectedOrderDetails(string orderPrefix, string teamName, string to);
    }
}
