namespace AwesomeGIC.Domain.Entities
{
    public class BankTransaction
    {
        public string TxnId { get; set; }
        
        public DateTime Date { get; set; }
        
        public string AccountNumber { get; set; }
        
        public char Type { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }
    }
}