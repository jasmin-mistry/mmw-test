using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Application.Notification.Interfaces
{
    public interface INotification
    {
        void Notify(Account account);
    }
}