using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using System.Linq.Expressions;

namespace BankAccount.Infrastructure.Repositories
{
    public class BankTransactionRepository : IBankTransactionRepository
    {
        private readonly List<BankTransaction> _bankTransactions = new List<BankTransaction>();

        public BankTransactionRepository()
        {
            Initialization();
        }

        public void Add(BankTransaction entity)
        {
            entity.TxnId = NewId(entity);

            _bankTransactions.Add(entity);
        }

        public BankTransaction GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BankTransaction> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BankTransaction> Find(Expression<Func<BankTransaction, bool>> predicate)
        {
            return _bankTransactions.AsQueryable().Where(predicate).OrderBy(o => o.Date);
        }

        public void Update(BankTransaction entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(BankTransaction entity)
        {
            throw new NotImplementedException();
        }

        private string NewId(BankTransaction bankTransaction)
        {
            var count = _bankTransactions.Where(x => x.Date == bankTransaction.Date).Count();
            var newId = bankTransaction.Date.ToString("yyyyMMdd") + "-" + (count + 1).ToString("00");

            return newId;
        }

        // This method is used to initialize the bank transactions
        private void Initialization()
        {
            _bankTransactions.Add(new BankTransaction()
            {
                Date=new DateTime(2023, 5,5),
                AccountNumber = "AC001",
                TxnId = "20230505-01",
                Type = 'D',
                Amount = 100,
                Balance = 100
            });

            _bankTransactions.Add(new BankTransaction()
            {
                Date = new DateTime(2023, 6, 1),
                AccountNumber = "AC001",
                TxnId = "20230601-01",
                Type = 'D',
                Amount = 150,
                Balance = 250
            });

            _bankTransactions.Add(new BankTransaction()
            {
                Date = new DateTime(2023, 6, 26),
                AccountNumber = "AC001",
                TxnId = "20230626-01",
                Type = 'W',
                Amount = 20,
                Balance = 230
            });
        }
    }
}