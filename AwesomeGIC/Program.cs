using AwesomeGIC.Domain.Entities;
using AwesomeGIC.Domain.Interfaces;
using AwesomeGIC.Domain.Services;
using BankAccount.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

// Set up the DI container.

var serviceProvider = new ServiceCollection()
    .AddSingleton<IInterestRuleRepository, InterestRuleRepository>()
    .AddSingleton<IBankTransactionRepository, BankTransactionRepository>()
    .AddSingleton<InterestRuleService, InterestRuleService>()
    .AddSingleton<BankTransactionService, BankTransactionService>()
    .BuildServiceProvider();

// Resolve the service.

var interestRuleService = serviceProvider.GetService<InterestRuleService>();
var bankTransactionService = serviceProvider.GetService<BankTransactionService>();

// Main Menu

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
            // Upon selecting Input transactions option, application prompts user for transaction details.
            
            mainMenuFlag = InputTransactions(bankTransactionService);

            break;
        case "I":
            // Upon selecting Define interest rule option, application prompts user to define interest rules.
            
            mainMenuFlag = DefineInterestRules(interestRuleService);

            break;

        case "P":
            // Upon selecting Print statement option, application prompts user to select which account to print the statement for.
            
            mainMenuFlag = PrintStatement(bankTransactionService);

            break;
        case "Q":
            // Upon selecting Quit option, application displays a thank you message and exits.

            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");

            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");

            break;
    }
}

static bool InputTransactions(BankTransactionService bankTransactionService)
{
    Console.Clear();
    Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format (or enter blank to go back to main menu):");
    Console.Write("> ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return true;

    var inputValues = input.Split(' ');

    if (inputValues.Length != 4)
    {
        Console.WriteLine("Invalid input format. Please try again.");
        Console.ReadLine();
        InputTransactions(bankTransactionService);

        return false;
    }

    try
    {
        var bankTransaction = new BankTransaction
        {
            Date = DateTime.ParseExact(inputValues[0], "yyyyMMdd", null),
            AccountNumber = inputValues[1],
            Type = char.ToUpper(inputValues[2][0]),
            Amount = decimal.Parse(inputValues[3])
        };

        bankTransactionService.AddBankTransaction(bankTransaction);
        PrintAccountStatement(bankTransactionService, bankTransaction.AccountNumber);
    }
    catch (FormatException formatEx)
    {
        Console.WriteLine("Date should be in YYYYMMdd format.");
        Console.ReadLine();

        InputTransactions(bankTransactionService);
    }
    catch (ApplicationException appEx)
    {
        Console.WriteLine(appEx.Message);
        Console.ReadLine();

        InputTransactions(bankTransactionService);
    }

    return false;
}

static void PrintAccountStatement(BankTransactionService bankTransactionService, string accountNumber)
{
    var transactions = bankTransactionService.GetBankTransactions(accountNumber);

    Console.WriteLine($"Account: {accountNumber}");
    Console.WriteLine("| Date     | Txn Id      | Type | Amount |");

    foreach (var transaction in transactions)
    {
        Console.WriteLine($"| {transaction.Date.ToString("yyyyMMdd")} | {transaction.TxnId} | {transaction.Type.ToString().PadRight(4, ' ')} | {transaction.Amount.ToString("f2").PadLeft(6, ' ')} |");
    }
}

static bool DefineInterestRules(InterestRuleService interestRuleService)
{
    Console.Clear();
    Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format (or enter blank to go back to main menu):");
    Console.Write("> ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return true;

    var inputValues = input.Split(' ');

    if (inputValues.Length != 3)
    {
        Console.WriteLine("Invalid input format. Please try again.");
        Console.ReadLine();

        DefineInterestRules(interestRuleService);

        return false;
    }

    try
    {
        var rule = new InterestRule
        {
            Date = DateTime.ParseExact(inputValues[0], "yyyyMMdd", null),
            RuleId = inputValues[1],
            Rate = decimal.Parse(inputValues[2])
        };

        interestRuleService.AddInterestRule(rule);
        PrintInterestRules(interestRuleService);
    }
    catch (FormatException formatEx)
    {
        Console.WriteLine("Date should be in YYYYMMdd format.");
        Console.ReadLine();

        DefineInterestRules(interestRuleService);
    }
    catch (ApplicationException appEx)
    {
        Console.WriteLine(appEx.Message);
        Console.ReadLine();

        DefineInterestRules(interestRuleService);
    }

    return false;
}

static void PrintInterestRules(InterestRuleService interestRuleService)
{
    var rules = interestRuleService.GetInterestRules();

    Console.WriteLine("Interest rules:");
    Console.WriteLine("| Date     | RuleId | Rate (%) |");

    foreach (var rule in rules)
    {
        Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId} | {rule.Rate.ToString("f2").PadLeft(8, ' ')} |");
    }
}

static bool PrintStatement(BankTransactionService bankTransactionService)
{
    Console.Clear();
    Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month> (or enter blank to go back to main menu):");
    Console.Write("> ");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        return true;

    var inputValues = input.Split(' ');

    if (inputValues.Length != 2)
    {
        Console.WriteLine("Invalid input format. Please try again.");

        PrintStatement(bankTransactionService);

        return false;
    }

    var account = inputValues[0];
    var year = Convert.ToInt32(inputValues[1].Substring(0, 4));
    var month = Convert.ToInt32(inputValues[1].Substring(4));

    var bankStatement = bankTransactionService.GetBankStatement(account, year, month);

    Console.WriteLine($"Account: {account}");
    Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

    foreach (var transaction in bankStatement)
    {
        Console.WriteLine($"| {transaction.Date:yyyyMMdd} | {transaction.TxnId.ToString().PadRight(11, ' ')} | {transaction.Type.ToString().PadRight(4, ' ')} | {transaction.Amount.ToString("f2").PadLeft(6, ' ')} | {transaction.Balance.ToString("f2").PadLeft(7, ' ')} |");
    }

    return false;
}