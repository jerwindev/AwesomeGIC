namespace AwesomeGIC.Domain.Entities
{
    public class InterestRule
    {
        public string RuleId { get; set; }

        public DateTime Date { get; set; }
        
        public decimal Rate { get; set; }
    }
}