namespace AwesomeGIC.Domain.Entities
{
    public class AccountStatement
    {
        public int NumberOfDays { get; set; }

        public decimal EodBalance { get; set; }

        public decimal Rate { get; set; }

        public decimal AnnualizedInterest 
        {
            get
            {
                return EodBalance * (Rate / 100) * NumberOfDays;
            }
        }
    }
}