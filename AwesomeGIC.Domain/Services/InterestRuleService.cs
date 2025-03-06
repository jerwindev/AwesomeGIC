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

        public void AddInterestRule(InterestRule rule)
        {
            var rules = _interestRuleRepository.Find(x => x.Date == rule.Date);

            if (rules != null && rules.Count() > 0)
            {
                _interestRuleRepository.Update(rule);
            }
            else
            {
                _interestRuleRepository.Add(rule);
            }
        }

        public IEnumerable<InterestRule> GetInterestRules()
        {
            return _interestRuleRepository.GetAll();
        }
    }
}