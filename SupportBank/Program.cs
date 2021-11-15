using System;
using System.Linq;

namespace SupportBank
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args[0] != "list") throw new ArgumentOutOfRangeException(nameof(args));
            
            Bank bank = Bank.FromFile(@"C:\Training\CSBootcamp\2-support-bank\SupportBank\SupportBank\Transactions2014.csv");

            if (args[1] == "all")
            {
                var orderedAccounts = bank.Accounts.OrderByDescending(p => bank.GetBalance(p));
                
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