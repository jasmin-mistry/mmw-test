using System;
using Moneybox.App.Application.Transactions.Interfaces;

namespace Moneybox.App.Application.Transactions
{
    public class WithdrawMoney
    {
        private readonly IDebitAccount debitAccount;

        public WithdrawMoney(IDebitAccount debitAccount)
        {
            this.debitAccount = debitAccount;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            debitAccount.SetAccount(fromAccountId);

            var fundsAvailable = debitAccount.IsSufficientBalanceAfterDebit(amount);

            if (!fundsAvailable)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            debitAccount.Debit(amount);
        }
    }
}