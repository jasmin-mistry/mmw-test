using System;
using Moneybox.App.Application.Transactions.Interfaces;

namespace Moneybox.App.Application.Transactions
{
    public class TransferMoney
    {
        private readonly ICreditAccount creditAccount;
        private readonly IDebitAccount debitAccount;

        public TransferMoney(IDebitAccount debitAccount, ICreditAccount creditAccount)
        {
            this.debitAccount = debitAccount;
            this.creditAccount = creditAccount;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            debitAccount.SetAccount(fromAccountId);
            creditAccount.SetAccount(toAccountId);

            if (!debitAccount.IsSufficientBalanceAfterDebit(amount))
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (creditAccount.IsPayInLimitReached(amount))
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            debitAccount.Debit(amount);
            creditAccount.Credit(amount);
        }
    }
}