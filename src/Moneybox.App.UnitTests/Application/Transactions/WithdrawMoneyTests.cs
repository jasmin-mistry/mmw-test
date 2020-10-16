using System;
using Moneybox.App.Application.Transactions;
using Moneybox.App.Application.Transactions.Interfaces;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Moneybox.App.UnitTests.Application.Transactions
{
    [TestFixture]
    public class WithdrawMoneyTests
    {
        private WithdrawMoney sut;
        private Mock<IDebitAccount> accountMock;

        [SetUp]
        public void BeforeEach()
        {
            accountMock = new Mock<IDebitAccount>();

            sut = new WithdrawMoney(accountMock.Object);

            accountMock.Setup(m => m.SetAccount(It.IsAny<Guid>()));
            accountMock.Setup(m => m.Debit(It.IsAny<decimal>()));

            accountMock.Setup(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>())).Returns(true);
        }

        [Test]
        public void Execute_ShouldDebitTheAccount_WhenBalanceIsSufficient()
        {
            sut.Execute(new Guid(), 100m);

            accountMock.Verify(m => m.Debit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldNotThrowInvalidOperationException_WhenBalanceIsSufficient()
        {
            sut.Execute(new Guid(), 100m);

            accountMock.Verify(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldSetAccount_BeforePerformingAnyDebitTransaction()
        {
            sut.Execute(new Guid(), 100m);

            accountMock.Verify(m => m.SetAccount(It.IsAny<Guid>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldThrowArgumentOutOfRangeException_WhenDebitAmountIs0()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => sut.Execute(new Guid(), 0m));
        }

        [Test]
        public void Execute_ShouldThrowInvalidOperationException_WhenBalanceIsInsufficient()
        {
            accountMock.Setup(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>())).Returns(false);

            Should.Throw<InvalidOperationException>(() => sut.Execute(new Guid(), 100m));

            accountMock.Verify(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }
    }
}