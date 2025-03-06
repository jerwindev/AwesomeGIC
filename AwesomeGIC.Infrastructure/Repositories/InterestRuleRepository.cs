using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace BankAccount.Infrastructure.Repositories
{
    public class InterestRuleRepository : IInterestRuleRepository
    {
        private readonly List<InterestRule> _interestRules = new List<InterestRule>();

        public InterestRuleRepository()
        {
            Initialization();
        }

        public void Add(InterestRule entity)
        {
            _interestRules.Add(entity);
        }

        public void Delete(InterestRule entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InterestRule> Find(Expression<Func<InterestRule, bool>> predicate)
        {
            return _interestRules.AsQueryable().Where(predicate).ToList();
        }

        public IEnumerable<InterestRule> GetAll()
        {
            return _interestRules.OrderBy(r => r.Date);
        }

        public InterestRule GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(InterestRule entity)
        {
            var interestRule = _interestRules.FirstOrDefault(x => x.Date == entity.Date);

            if (interestRule != null)
            {
                interestRule.RuleId = entity.RuleId;
                interestRule.Rate = entity.Rate;
            }
        }

        // This method is used to initialize the interest rules
        private void Initialization()
        {
            _interestRules.Add(new InterestRule()
            {
                Date = new DateTime(2023, 1, 1),
                RuleId = "RULE01",
                Rate = 1.95m
            });

            _interestRules.Add(new InterestRule()
            {
                Date = new DateTime(2023, 5, 20),
                RuleId = "RULE02",
                Rate = 1.90m
            });
        }
    }
}