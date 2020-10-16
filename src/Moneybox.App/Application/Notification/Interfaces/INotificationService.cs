namespace Moneybox.App.Application.Notification.Interfaces
{
    public interface INotificationService
    {
        void NotifyApproachingPayInLimit(string emailAddress);

        void NotifyFundsLow(string emailAddress);
    }
}