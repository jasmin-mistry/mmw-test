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
    public class CreditAccountTests
    {
        private CreditAccount sut;
        private Mock<IAccountRepository> accountRepositoryMock;
        private Mock<IPayInLimitNotification> notificationMock;

        private static IEnumerable<TestCaseData> PayInLimitReachedTestData
        {
            get
            {
                yield return new TestCaseData(Account.PayInLimit);
                yield return new TestCaseData(Account.PayInLimit + 1);
            }
        }

        private static IEnumerable<TestCaseData> SufficientPayInLimitTestData
        {
            get
            {
                yield return new TestCaseData(-1m);
                yield return new TestCaseData(0m);
                yield return new TestCaseData(Account.PayInLimit - 1);
            }
        }

        [SetUp]
        public void BeforeEach()
        {
            accountRepositoryMock = new Mock<IAccountRepository>();
            notificationMock = new Mock<IPayInLimitNotification>();

            sut = new CreditAccount(accountRepositoryMock.Object, notificationMock.Object)
            {
                Account = new Account()
            };

            accountRepositoryMock.Setup(m => m.Update(It.IsAny<Account>()));
            notificationMock.Setup(m => m.Notify(It.IsAny<Account>()));
        }

        [Test]
        public void Credit_ShouldNotify_WhenPayInLimitIsReached()
        {
            sut.Account.PaidIn = 3500;

            sut.Credit(1);

            sut.Account.PaidIn.ShouldBe(3501);
            notificationMock.Verify(m => m.Notify(It.IsAny<Account>()), Times.AtLeastOnce);
        }

        [Test]
        public void Credit_ShouldNotNotify_WhenPayInLimitIsNotReached()
        {
            sut.Account.PaidIn = 3499;

            sut.Credit(1);

            sut.Account.PaidIn.ShouldBe(3500);
            notificationMock.Verify(m => m.Notify(It.IsAny<Account>()), Times.Never);
        }

        [Test]
        public void Credit_ShouldSuccessfullyUpdateAccount_WithNewBalanceAndPaidInAmount()
        {
            sut.Account.Balance = 100;
            sut.Account.PaidIn = 100;

            sut.Credit(1);

            sut.Account.Balance.ShouldBe(101);
            sut.Account.PaidIn.ShouldBe(101);
            accountRepositoryMock.Verify(m => m.Update(It.IsAny<Account>()), Times.AtLeastOnce);
        }

        [Test]
        [TestCaseSource(nameof(SufficientPayInLimitTestData))]
        public void IsPayInLimitReached_ReturnsFalse_WhenPayInLimitIsNotReached(decimal testAmount)
        {
            sut.Account.PaidIn = testAmount;
            var expectedResult = sut.IsPayInLimitReached(1);
            expectedResult.ShouldBeFalse();
        }

        [Test]
        [TestCaseSource(nameof(PayInLimitReachedTestData))]
        public void IsPayInLimitReached_ReturnsTrue_WhenPayInLimitIsReached(decimal testAmount)
        {
            sut.Account.PaidIn = testAmount;
            var expectedResult = sut.IsPayInLimitReached(1);
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