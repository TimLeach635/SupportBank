using System;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    internal class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private static void Main(string[] args)
        {
            // set up logger
            var config = new LoggingConfiguration();
            var target = new FileTarget
            {
                FileName = @"C:\Training\CSBootcamp\2-support-bank\SupportBank\SupportBank\SupportBank.log",
                Layout = @"${longdate} ${level} - ${logger}: ${message}"
            };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
            
            if (args[0] != "list") throw new ArgumentOutOfRangeException(nameof(args));
            
            Bank bank = Bank.FromFile(@"C:\Training\CSBootcamp\2-support-bank\SupportBank\SupportBank\Transactions2014.csv");

            if (args[1] == "all")
            {
                Logger.Debug($"args[1]: {args[1]}");
                Logger.Debug("Executing 'all' script");
                var orderedAccounts = bank.People.OrderByDescending(p => bank.GetBalance(p));
                
                foreach (var person in orderedAccounts)
                {
                    decimal balance = bank.GetBalance(person);
                    switch (balance)
                    {
                        case > 0:
                            Console.Out.WriteLine($"{person.Name} is owed £{balance:N2}");
                            break;
                        case < 0:
                            Console.Out.WriteLine($"{person.Name} owes £{-balance:N2}");
                            break;
                        default:
                            Console.Out.WriteLine($"{person.Name} is all even!");
                            break;
                    }
                }
            }
            else
            {
                foreach (Transaction transaction in bank.AccountTransactions(args[1]))
                {
                    Console.Out.WriteLine(transaction);
                }
            }
        }
    }
}