using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var before = this.accountRepository.GetAccountById(fromAccountId);

            var beforeBalance = before.Balance - amount;
            if (beforeBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }
            if (beforeBalance < 500m)
            {
                this.notificationService.NotifyFundsLow(before.User.Email);
            }

            before.Balance = before.Balance - amount;
            before.Withdrawn = before.Withdrawn - amount;

            this.accountRepository.Update(before);
        }
    }
}
