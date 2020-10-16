using System;
using Moneybox.App.Domain.Entities;

namespace Moneybox.App.Application.DataAccess
{
    public interface IAccountRepository
    {
        Account GetAccountById(Guid accountId);

        void Update(Account account);
    }
}