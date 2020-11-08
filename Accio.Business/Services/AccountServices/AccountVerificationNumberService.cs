using Accio.Business.Models.AccountModels;
using Accio.Business.Models.EmailModels;
using Accio.Business.Services.EmailServices;
using Accio.Data;
using System;
using System.Linq;

namespace Accio.Business.Services.AccountServices
{
    public class AccountVerificationNumberService
    {
        private AccioContext _context { get; set; }

        public AccountVerificationNumberService(AccioContext context)
        {
            _context = context;
        }

        public AccountVerificationNumberModel GetAccountVerificationNumber(Guid accountId)
        {
            var randomNumber = GetRandomNumber();

            var now = DateTime.UtcNow;
            var number = new AccountVerificationNumber()
            {
                AccountVerificationNumberId = Guid.NewGuid(),
                AccountId = accountId,
                Expires = now.AddHours(1),
                Number = randomNumber,
                CreatedById = accountId,
                CreatedDate = now,
                UpdatedById = accountId,
                UpdatedDate = now,
                Deleted = false,
            };

            return GetAccountVerificationNumberModel(number);
        }
        public void SaveAccountVerificationNumber(AccountVerificationNumberModel numberModel)
        {
            var number = GetAccountVerificationNumber(numberModel);
            _context.AccountVerificationNumber.Add(number);
            _context.SaveChanges();
        }

        /// <summary>
        /// Returns a random number from the account verification number table, avoiding duplicates where possible.
        /// Each verification record *should* be permanently deleted after the account has been verified. This, along with
        /// using choosing numbers between 100,000 and 999,999
        /// </summary>
        /// <returns></returns>
        private int GetRandomNumber()
        {
            /*
             * This is pretty straightforward. We pull all the numbers from the database to compare. Each number is hard deleted
             * after it's used up, so this list won't ever be long. Next we get a random number and then loop. Once a random
             * number is not in the pulled list, we know it's unique. So we stop the loop and return the value.
             */

            var randomNumbers = _context.AccountVerificationNumber.Where(x => !x.Deleted).Select(x => x.Number).ToList();
            var randomNumber = new Random().Next(100000, 999999);

            var isRandomNumberValid = false;
            while (!isRandomNumberValid)
            {
                if (!randomNumbers.Any(x => x == randomNumber))
                {
                    isRandomNumberValid = true;
                }
                else
                {
                    randomNumber = new Random().Next(100000, 999999);
                }
            }

            return randomNumber;
        }

        private AccountVerificationNumberModel GetAccountVerificationNumberModel(AccountVerificationNumber accountVerificationNumber)
        {
            return new AccountVerificationNumberModel()
            {
                AccountVerificationNumberId = accountVerificationNumber.AccountVerificationNumberId,
                AccountId = accountVerificationNumber.AccountId,
                Number = accountVerificationNumber.Number,
                Expires = accountVerificationNumber.Expires,
                CreatedById = accountVerificationNumber.CreatedById,
                CreatedDate = accountVerificationNumber.CreatedDate,
                UpdatedById = accountVerificationNumber.UpdatedById,
                UpdatedDate = accountVerificationNumber.UpdatedDate,
                Deleted = accountVerificationNumber.Deleted,
            };
        }
        private AccountVerificationNumber GetAccountVerificationNumber(AccountVerificationNumberModel accountVerificationNumberModel)
        {
            return new AccountVerificationNumber()
            {
                AccountVerificationNumberId = accountVerificationNumberModel.AccountVerificationNumberId,
                AccountId = accountVerificationNumberModel.AccountId,
                Number = accountVerificationNumberModel.Number,
                Expires = accountVerificationNumberModel.Expires,
                CreatedById = accountVerificationNumberModel.CreatedById,
                CreatedDate = accountVerificationNumberModel.CreatedDate,
                UpdatedById = accountVerificationNumberModel.UpdatedById,
                UpdatedDate = accountVerificationNumberModel.UpdatedDate,
                Deleted = accountVerificationNumberModel.Deleted,
            };
        }
    }
}
