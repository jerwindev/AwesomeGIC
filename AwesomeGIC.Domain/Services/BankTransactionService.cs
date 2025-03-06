using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;

namespace AwesomeGIC.Domain.Services
{
    public class BankTransactionService
    {
        private readonly IBankTransactionRepository _bankTransactionRepository;
        private readonly InterestRuleService _interestRuleService;

        public BankTransactionService(IBankTransactionRepository bankTransactionRepository,
            InterestRuleService interestRuleService)
        {
            _bankTransactionRepository = bankTransactionRepository;
            _interestRuleService = interestRuleService;
        }

        public void AddBankTransaction(BankTransaction bankTransaction)
        {
            var lastTransaction = GetBankTransactions(bankTransaction.AccountNumber).LastOrDefault();
            var runningBalance = lastTransaction.Balance;

            if (bankTransaction.Type == 'D')
            {
                bankTransaction.Balance = runningBalance + bankTransaction.Amount;
            }
            else if (bankTransaction.Type == 'W')
            {
                bankTransaction.Balance = runningBalance - bankTransaction.Amount;
            }

            _bankTransactionRepository.Add(bankTransaction);
        }

        public IEnumerable<BankTransaction> GetBankTransactions(string accountNumber)
        {
            return _bankTransactionRepository.Find(x => x.AccountNumber == accountNumber);
        }

        public IEnumerable<BankTransaction> GetBankStatement(string account, int year, int month)
        {
            var interestRules = _interestRuleService.GetInterestRules().ToList();
            var bankTransactions = GetBankTransactions(account)
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

            var period1 = new DateTime(year, month, 14);
            var period2 = period1.AddDays(11);
            var period3 = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            var statement1 = new AccountStatement();

            statement1.NumberOfDays = period1.Day;
            statement1.EodBalance = bankTransactions.Where(t => t.Date <= period1).Last().Balance;
            statement1.Rate = interestRules.Where(r => r.Date <= period1).Last().Rate;

            var statement2 = new AccountStatement();

            statement2.NumberOfDays = (period2 - period1).Days;
            statement2.EodBalance = bankTransactions.Where(t => t.Date <= period2).Last().Balance;
            statement2.Rate = interestRules.Where(r => r.Date <= period2).Last().Rate;

            var statement3 = new AccountStatement();

            statement3.NumberOfDays = (period3 - period2).Days;
            statement3.EodBalance = bankTransactions.Where(t => t.Date <= period3).Last().Balance;
            statement3.Rate = interestRules.Where(r => r.Date <= period3).Last().Rate;

            var totalInterest = Math.Round( (statement1.AnnualizedInterest + 
                statement2.AnnualizedInterest + 
                statement3.AnnualizedInterest) / 365, 2);

            bankTransactions.Add(new BankTransaction()
            {
                Date = period3,
                AccountNumber = account,
                TxnId = "",
                Type = 'I',
                Amount = totalInterest,
                Balance = bankTransactions.LastOrDefault().Balance + totalInterest
            });

            return bankTransactions;
        }
    }
}