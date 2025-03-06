using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using AwesomeGIC.Domain.Services;
using BankAccount.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

// Set up the DI container

var serviceProvider = new ServiceCollection()
    .AddSingleton<IInterestRuleRepository, InterestRuleRepository>()
    .AddSingleton<IBankTransactionRepository, BankTransactionRepository>()
    .AddSingleton<InterestRuleService, InterestRuleService>()
    .AddSingleton<BankTransactionService, BankTransactionService>()
    .BuildServiceProvider();


// Resolve the service and use it
var interestRuleService = serviceProvider.GetService<InterestRuleService>();
var bankTransactionService = serviceProvider.GetService<BankTransactionService>();
var mainMenuFlag = true;

while (true)
{
    if (mainMenuFlag)
    {
        Console.Clear();
        Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
        mainMenuFlag = false;
    }
    else
    {
        Console.WriteLine("");
        Console.WriteLine("Is there anything else you'd like to do?");
    }

    Console.WriteLine("[T] Input transactions");
    Console.WriteLine("[I] Define interest rules");
    Console.WriteLine("[P] Print statement");
    Console.WriteLine("[Q] Quit");
    Console.Write("> ");

    var choice = Console.ReadLine()?.ToUpper();

    switch (choice)
    {
        case "T":
            InputTransactions(bankTransactionService);
            mainMenuFlag = false;
            break;
        case "I":
            DefineInterestRules(interestRuleService);
            break;
        case "P":
            PrintStatement(bankTransactionService);
            break;
        case "Q":
            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}

static void InputTransactions(BankTransactionService bankTransactionService)
{
    Console.Clear();
    Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format (or enter blank to go back to main menu):");
    Console.Write("> ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return;

    var parts = input.Split(' ');

    //if (parts.Length != 4)
    //{
    //    Console.WriteLine("Invalid input format. Please try again.");
    //    continue;
    //}

    var bankTransaction = new BankTransaction
    {
        Date = DateTime.ParseExact(parts[0], "yyyyMMdd", null),
        AccountNumber = parts[1],
        Type = char.ToUpper(parts[2][0]),
        Amount = decimal.Parse(parts[3])
    };

    bankTransactionService.AddBankTransaction(bankTransaction);
    PrintAccountStatement(bankTransactionService, bankTransaction.AccountNumber);
}

static void PrintAccountStatement(BankTransactionService bankTransactionService, string accountNumber)
{
    var transactions = bankTransactionService.GetBankTransactions(accountNumber);

    Console.WriteLine($"Account: {accountNumber}");
    Console.WriteLine("|    Date    |   Txn Id      |      Type    |   Amount  |");

    foreach (var transaction in transactions)
    {
        Console.WriteLine($"|   {transaction.Date:yyyyMMdd} |   {transaction.TxnId} |       {transaction.Type}      |   {transaction.Amount:F2} |");
    }
}

static void DefineInterestRules(InterestRuleService interestRuleService)
{
    Console.Clear();
    Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format (or enter blank to go back to main menu):");
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return;

    var parts = input.Split(' ');
    //if (parts.Length != 3)
    //{
    //    Console.WriteLine("Invalid input format. Please try again.");
    //    continue;
    //}

    var rule = new InterestRule
    {
        Date = DateTime.ParseExact(parts[0], "yyyyMMdd", null),
        RuleId = parts[1],
        Rate = decimal.Parse(parts[2])
    };

    interestRuleService.AddInterestRule(rule);
    PrintInterestRules(interestRuleService);
}

static void PrintInterestRules(InterestRuleService interestRuleService)
{
    var rules = interestRuleService.GetInterestRules();

    Console.WriteLine("Interest rules:");
    Console.WriteLine("| Date     | RuleId | Rate (%) |");

    foreach (var rule in rules)
    {
        Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId} | {rule.Rate:F2} |");
    }
}

static void PrintStatement(BankTransactionService bankTransactionService)
{
    Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month> (or enter blank to go back to main menu):");
    Console.Write("> ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return;

    var parts = input.Split(' ');
    if (parts.Length != 2)
    {
        Console.WriteLine("Invalid input format. Please try again.");
        return;
    }

    var account = parts[0];
    var year = Convert.ToInt32(parts[1].Substring(0, 4));
    var month = Convert.ToInt32(parts[1].Substring(4));

    var bankStatement = bankTransactionService.GetBankStatement(account, year, month);

    Console.WriteLine($"Account: {account}");
    Console.WriteLine("| Date     | Txn Id      | Type | Amount |   Balance");

    foreach (var transaction in bankStatement)
    {
        Console.WriteLine($"| {transaction.Date:yyyyMMdd} | {transaction.TxnId} | {transaction.Type} | {transaction.Amount:F2} | {transaction.Balance}");
    }
}