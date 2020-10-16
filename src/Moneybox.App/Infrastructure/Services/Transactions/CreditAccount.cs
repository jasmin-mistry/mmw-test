using System;
using Moneybox.App.Application.DataAccess;
using Moneybox.App.Application.Notification.Interfaces;
using Moneybox.App.Application.Transactions.Interfaces;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Infrastructure.Services.Transactions
{
    public class CreditAccount : ICreditAccount
    {
        private readonly IAccountRepository accountRepository;
        private readonly IPayInLimitNotification payInLimitNotification;

        public CreditAccount(IAccountRepository accountRepository,
            IPayInLimitNotification payInLimitNotification)
        {
            this.accountRepository = accountRepository;
            this.payInLimitNotification = payInLimitNotification;
        }

        public void SetAccount(Guid accountId)
        {
            Account = accountRepository.GetAccountById(accountId);
        }

        public Account Account { get; set; }

        public void Credit(decimal amount)
        {
            Account.Balance += amount;
            Account.PaidIn += amount;

            accountRepository.Update(Account);

            if (Account.PayInLimit - Account.PaidIn < 500m)
            {
                payInLimitNotification.Notify(Account);
            }
        }

        public bool IsPayInLimitReached(decimal amount)
        {
            var paidIn = Account.PaidIn + amount;
            return (paidIn > Account.PayInLimit);
        }
    }
}