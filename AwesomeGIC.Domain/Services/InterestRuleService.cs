using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;

namespace AwesomeGIC.Domain.Services
{
    public class InterestRuleService
    {
        private readonly IInterestRuleRepository _interestRuleRepository;

        public InterestRuleService(IInterestRuleRepository interestRuleRepository)
        {
            _interestRuleRepository = interestRuleRepository;
        }

        public void AddInterestRule(InterestRule interestRule)
        {
            Validate(interestRule);

            var rules = _interestRuleRepository.Find(x => x.Date == interestRule.Date);

            if (rules != null && rules.Count() > 0)
            {
                _interestRuleRepository.Update(interestRule);
            }
            else
            {
                _interestRuleRepository.Add(interestRule);
            }
        }

        public IEnumerable<InterestRule> GetInterestRules()
        {
            return _interestRuleRepository.GetAll();
        }

        private void Validate(InterestRule interestRule)
        {
            if (string.IsNullOrEmpty(interestRule.RuleId))
            {
                throw new ApplicationException("Rule ID is required.");
            }
            else if (interestRule.Rate <= 0 || interestRule.Rate >= 100)
            {
                throw new ApplicationException("Interest rate should be greater than 0 and less than 100.");
            }
        }
    }
}