using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using AwesomeGIC.Domain.Services;
using Moq;
using System.Linq.Expressions;

public class InterestRuleServiceTests
{
    private readonly Mock<IInterestRuleRepository> _mockRepository;
    private readonly InterestRuleService _service;

    public InterestRuleServiceTests()
    {
        _mockRepository = new Mock<IInterestRuleRepository>();
        _service = new InterestRuleService(_mockRepository.Object);
    }

    [Fact]
    public void AddInterestRule_ShouldAddRule_WhenRuleIsValidAndNotExists()
    {
        // Arrange
        var interestRule = new InterestRule { RuleId = "1", Date = DateTime.Now, Rate = 1.5m };
        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<InterestRule, bool>>>())).Returns(new List<InterestRule>());

        // Act
        _service.AddInterestRule(interestRule);

        // Assert
        _mockRepository.Verify(repo => repo.Add(It.IsAny<InterestRule>()), Times.Once);
    }

    [Fact]
    public void AddInterestRule_ShouldUpdateRule_WhenRuleExists()
    {
        // Arrange
        var interestRule = new InterestRule { RuleId = "1", Date = DateTime.Now, Rate = 1.5m };
        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<InterestRule, bool>>>())).Returns(new List<InterestRule> { interestRule });

        // Act
        _service.AddInterestRule(interestRule);

        // Assert
        _mockRepository.Verify(repo => repo.Update(It.IsAny<InterestRule>()), Times.Once);
    }

    [Fact]
    public void GetInterestRules_ShouldReturnAllRules()
    {
        // Arrange
        var interestRules = new List<InterestRule> { new InterestRule { RuleId = "1", Date = DateTime.Now, Rate = 1.5m } };
        _mockRepository.Setup(repo => repo.GetAll()).Returns(interestRules);

        // Act
        var result = _service.GetInterestRules();

        // Assert
        Assert.Equal(interestRules, result);
    }

    [Fact]
    public void AddInterestRule_ShouldThrowException_WhenRuleIdIsEmpty()
    {
        // Arrange
        var interestRule = new InterestRule { RuleId = "", Date = DateTime.Now, Rate = 1.5m };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddInterestRule(interestRule));
        Assert.Equal("Rule ID is required.", exception.Message);
    }

    [Fact]
    public void AddInterestRule_ShouldThrowException_WhenRateIsInvalid()
    {
        // Arrange
        var interestRule = new InterestRule { RuleId = "1", Date = DateTime.Now, Rate = 0 };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddInterestRule(interestRule));
        Assert.Equal("Interest rate should be greater than 0 and less than 100.", exception.Message);
    }
}