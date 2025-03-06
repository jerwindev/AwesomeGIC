using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using AwesomeGIC.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

public class BankTransactionServiceTests
{
    private readonly Mock<IBankTransactionRepository> _mockRepository;
    private readonly BankTransactionService _service;
    private readonly Mock<IInterestRuleRepository> _mockInterestRuleRepository;
    private readonly InterestRuleService _interestRuleService;

    public BankTransactionServiceTests()
    {
        _mockInterestRuleRepository = new Mock<IInterestRuleRepository>();
        _interestRuleService = new InterestRuleService(_mockInterestRuleRepository.Object);

        _mockRepository = new Mock<IBankTransactionRepository>();
        _service = new BankTransactionService(_mockRepository.Object, _interestRuleService);
    }

    [Fact]
    public void GetTransactions_ShouldReturnAllTransactions()
    {
        // Arrange
        var transactions = new List<BankTransaction> 
        {
            new BankTransaction
            {
                TxnId = "20230601-01",
                AccountNumber = "AC001",
                Amount = 100.0m,
                Date = DateTime.Now,
                Type = 'D',
                Balance = 250
            }
        };

        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<BankTransaction, bool>>>())).Returns(transactions);

        // Act
        var result = _service.GetBankTransactions("AC001");

        // Assert
        Assert.Equal(transactions, result);
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenAccountNumberIsInvalid()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction
            {
                TxnId = "20230601-01",
                AccountNumber = "AC001",
                Amount = 100.0m,
                Date = DateTime.Now,
                Type = 'D',
                Balance = 250
            }
        };

        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<BankTransaction, bool>>>())).Returns(transactions);

        var transaction = new BankTransaction
        {
            TxnId = "20230601-01",
            AccountNumber = "",
            Amount = 100.0m,
            Date = DateTime.Now,
            Type = 'D',
            Balance = 250
        };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddBankTransaction(transaction));

        Assert.Equal("Account number is required.", exception.Message);
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenTypeIsInvalid()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction
            {
                TxnId = "20230601-01",
                AccountNumber = "AC001",
                Amount = 100.0m,
                Date = DateTime.Now,
                Type = 'D',
                Balance = 250
            }
        };

        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<BankTransaction, bool>>>())).Returns(transactions);

        var transaction = new BankTransaction
        {
            TxnId = "20230601-01",
            AccountNumber = "AC001",
            Amount = 100.0m,
            Date = DateTime.Now,
            Type = 'X',
            Balance = 250
        };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddBankTransaction(transaction));

        Assert.Equal("Transaction type must be either 'D' or 'W'.", exception.Message);
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenAmountIsInvalid()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction
            {
                TxnId = "20230601-01",
                AccountNumber = "AC001",
                Amount = 100.0m,
                Date = DateTime.Now,
                Type = 'D',
                Balance = 250
            }
        };

        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<BankTransaction, bool>>>())).Returns(transactions);

        var transaction = new BankTransaction 
        {
            TxnId = "20230601-01",
            AccountNumber = "AC001",
            Amount = -100.0m,
            Date = DateTime.Now,
            Type = 'D',
            Balance = 250
        };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddBankTransaction(transaction));

        Assert.Equal("Amount must be greater than zero, decimals are allowed up to 2 decimal places.", exception.Message);
    }

    [Fact]
    public void AddTransaction_ShouldThrowException_WhenBalanceIsInvalid()
    {
        // Arrange
        var transactions = new List<BankTransaction>
        {
            new BankTransaction
            {
                TxnId = "20230601-01",
                AccountNumber = "AC001",
                Amount = 100.0m,
                Date = DateTime.Now,
                Type = 'D'
            }
        };

        _mockRepository.Setup(repo => repo.Find(It.IsAny<Expression<Func<BankTransaction, bool>>>())).Returns(transactions);

        var transaction = new BankTransaction
        {
            TxnId = "20230601-02",
            AccountNumber = "AC001",
            Amount = 100.0m,
            Date = DateTime.Now,
            Type = 'W'
        };

        // Act & Assert
        var exception = Assert.Throws<ApplicationException>(() => _service.AddBankTransaction(transaction));

        Assert.Equal("Account balance should not be less than zero.", exception.Message);
    }
}