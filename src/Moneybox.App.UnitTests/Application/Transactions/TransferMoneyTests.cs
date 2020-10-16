using System;
using Moneybox.App.Application.Transactions;
using Moneybox.App.Application.Transactions.Interfaces;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Moneybox.App.UnitTests.Application.Transactions
{
    [TestFixture]
    public class TransferMoneyTests
    {
        private TransferMoney sut;
        private Mock<IDebitAccount> debitMock;
        private Mock<ICreditAccount> creditMock;

        [SetUp]
        public void BeforeEach()
        {
            debitMock = new Mock<IDebitAccount>();
            creditMock = new Mock<ICreditAccount>();

            sut = new TransferMoney(debitMock.Object, creditMock.Object);

            debitMock.Setup(m => m.SetAccount(It.IsAny<Guid>()));
            debitMock.Setup(m => m.Debit(It.IsAny<decimal>()));
            debitMock.Setup(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>())).Returns(true);

            creditMock.Setup(m => m.SetAccount(It.IsAny<Guid>()));
            creditMock.Setup(m => m.Credit(It.IsAny<decimal>()));
            creditMock.Setup(m => m.IsPayInLimitReached(It.IsAny<decimal>())).Returns(false);
        }

        [Test]
        public void Execute_ShouldCreditTheAccount_WhenBalanceIsSufficientAndPayInLimitIsNotReached()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            creditMock.Verify(m => m.Credit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldDebitTheAccount_WhenBalanceIsSufficient()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            debitMock.Verify(m => m.Debit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldNotThrowInvalidOperationException_WhenBalanceIsSufficient()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            debitMock.Verify(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldNotThrowInvalidOperationException_WhenPayInLimitIsNotReached()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            creditMock.Verify(m => m.IsPayInLimitReached(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldSetCreditAccount_BeforePerformingAnyTransaction()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            creditMock.Verify(m => m.SetAccount(It.IsAny<Guid>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldSetDebitAccount_BeforePerformingAnyTransaction()
        {
            sut.Execute(new Guid(), new Guid(), 100m);

            debitMock.Verify(m => m.SetAccount(It.IsAny<Guid>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldThrowArgumentOutOfRangeException_WhenDebitAmountIs0()
        {
            Should.Throw<ArgumentOutOfRangeException>(() => sut.Execute(new Guid(), new Guid(), 0m));
        }

        [Test]
        public void Execute_ShouldThrowInvalidOperationException_WhenBalanceIsInsufficient()
        {
            debitMock.Setup(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>())).Returns(false);

            Should.Throw<InvalidOperationException>(() => sut.Execute(new Guid(), new Guid(), 100m));

            debitMock.Verify(m => m.IsSufficientBalanceAfterDebit(It.IsAny<decimal>()), Times.AtLeastOnce);
        }

        [Test]
        public void Execute_ShouldThrowInvalidOperationException_WhenPayInLimitIsReached()
        {
            creditMock.Setup(m => m.IsPayInLimitReached(It.IsAny<decimal>())).Returns(true);

            Should.Throw<InvalidOperationException>(() => sut.Execute(new Guid(), new Guid(), 100m));

            creditMock.Verify(m => m.IsPayInLimitReached(It.IsAny<decimal>()), Times.AtLeastOnce);
        }
    }
}