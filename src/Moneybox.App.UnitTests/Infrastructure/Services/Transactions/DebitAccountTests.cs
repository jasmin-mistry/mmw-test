using System;
using System.Collections.Generic;
using Moneybox.App.Application.DataAccess;
using Moneybox.App.Application.Notification.Interfaces;
using Moneybox.App.Domain.Entities;
using Moneybox.App.Infrastructure.Services.Transactions;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Moneybox.App.UnitTests.Infrastructure.Services.Transactions
{
    [TestFixture]
    public class DebitAccountTests
    {
        private DebitAccount sut;
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<ILowFundNotification> notificationMock;

        private static IEnumerable<TestCaseData> SufficientBalanceTestData
        {
            get
            {
                yield return new TestCaseData(2m);
                yield return new TestCaseData(Account.PayInLimit);
            }
        }

        private static IEnumerable<TestCaseData> InsufficientBalanceTestData
        {
            get
            {
                yield return new TestCaseData(-1m);
                yield return new TestCaseData(0m);
            }
        }

        [SetUp]
        public void BeforeEach()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationMock = new Mock<ILowFundNotification>();

            sut = new DebitAccount(accountRepositoryMock.Object, notificationMock.Object)
            {
                Account = new Account()
            };

            accountRepositoryMock.Setup(m => m.Update(It.IsAny<Account>()));
            notificationMock.Setup(m => m.Notify(It.IsAny<Account>()));
        }

        [Test]
        public void Debit_ShouldNotify_WhenBalanceIsLow()
        {
            sut.Account.Balance = 500;

            sut.Debit(1);

            sut.Account.Balance.ShouldBe(499);
            notificationMock.Verify(m => m.Notify(It.IsAny<Account>()), Times.AtLeastOnce);
        }

        [Test]
        public void Debit_ShouldNotNotify_WhenBalanceIsSufficient()
        {
            sut.Account.Balance = 501;

            sut.Debit(1);

            sut.Account.Balance.ShouldBe(500);
            notificationMock.Verify(m => m.Notify(It.IsAny<Account>()), Times.Never);
        }

        [Test]
        public void Debit_ShouldSuccessfullyUpdateAccount_WithNewBalanceAndWithdrawnAmount()
        {
            sut.Account.Balance = 100;
            sut.Account.Withdrawn = 100;

            sut.Debit(1);

            sut.Account.Balance.ShouldBe(99);
            sut.Account.Withdrawn.ShouldBe(99);
            accountRepositoryMock.Verify(m => m.Update(It.IsAny<Account>()), Times.AtLeastOnce);
        }

        [Test]
        [TestCaseSource(nameof(SufficientBalanceTestData))]
        public void IsSufficientBalanceAfterDebit_ReturnsFalse_WhenBalanceIsMoreThen0(decimal testAmount)
        {
            sut.Account.Balance = testAmount;
            var expectedResult = sut.IsSufficientBalanceAfterDebit(1);
            expectedResult.ShouldBeFalse();
        }

        [Test]
        [TestCaseSource(nameof(InsufficientBalanceTestData))]
        public void IsSufficientBalanceAfterDebit_ReturnsTrue_WhenPayInLimitIsReached(decimal testAmount)
        {
            sut.Account.Balance = testAmount;
            var expectedResult = sut.IsSufficientBalanceAfterDebit(1);
            expectedResult.ShouldBeTrue();
        }

        [Test]
        public void SetAccount_ShouldGetAccount_WhenAccountIdIsPassed()
        {
            accountRepositoryMock.Setup(m => m.GetAccountById(It.IsAny<Guid>())).Returns(new Account
            {
                Balance = 100m,
                PaidIn = 100m,
                Withdrawn = 10m,
                User = new User
                {
                    Email = "test@test.com",
                    Name = "Test Test"
                }
            });

            sut.SetAccount(new Guid());

            sut.Account.ShouldNotBeNull();
        }
    }
}