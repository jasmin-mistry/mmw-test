using System;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Application.Transactions.Interfaces
{
    public interface ICreditAccount
    {
        Account Account { get; }
        void SetAccount(Guid accountId);
        void Credit(decimal amount);
        bool IsPayInLimitReached(decimal amount);
    }
}