using System;
using Moneybox.App.Application.DataAccess;
using Moneybox.App.Application.Notification.Interfaces;
using Moneybox.App.Application.Transactions.Interfaces;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Infrastructure.Services.Transactions
{
    public class DebitAccount : IDebitAccount
    {
        private readonly IAccountRepository accountRepository;
        private readonly ILowFundNotification lowFundNotification;

        public DebitAccount(IAccountRepository accountRepository, ILowFundNotification lowFundNotification)
        {
            this.accountRepository = accountRepository;
            this.lowFundNotification = lowFundNotification;
        }

        public void SetAccount(Guid accountId)
        {
            Account = accountRepository.GetAccountById(accountId);
        }

        public Account Account { get; set; }

        public void Debit(decimal amount)
        {
            Account.Balance -= amount;
            Account.Withdrawn -= amount;

            accountRepository.Update(Account);

            if (Account.Balance < 500m)
            {
                lowFundNotification.Notify(Account);
            }
        }

        public bool IsSufficientBalanceAfterDebit(decimal amount)
        {
            var fromBalance = Account.Balance - amount;
            return (fromBalance < 0m);
        }
    }
}