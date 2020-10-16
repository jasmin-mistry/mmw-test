using Moneybox.App.Application.Notification.Interfaces;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Infrastructure.Services.Notifications
{
    public class LowFundNotification : ILowFundNotification
    {
        private readonly INotificationService notificationService;

        public LowFundNotification(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public void Notify(Account account)
        {
            // This will now allow us to send notification to email or text or voice mail based on the user preferences
            notificationService.NotifyFundsLow(account.User.Email);
        }
    }
}