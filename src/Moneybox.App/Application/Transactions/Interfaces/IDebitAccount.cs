using System;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Application.Transactions.Interfaces
{
    public interface IDebitAccount
    {
        Account Account { get; }
        void SetAccount(Guid accountId);
        void Debit(decimal amount);
        bool IsSufficientBalanceAfterDebit(decimal amount);
    }
}