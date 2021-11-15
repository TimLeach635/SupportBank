using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank
{
    public class Bank
    {
        public List<Person> Accounts = new();
        public List<Transaction> Transactions = new();

        public static Bank FromFile(string path)
        {
            using var reader = new StreamReader(path);

            Bank bank = new Bank();
            
            // discard header row
            reader.ReadLine();
            
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line is null)
                    break;
                var values = line.Split(",");

                var dateString = values[0];
                var fromString = values[1];
                var toString = values[2];
                var narrativeString = values[3];
                var amountString = values[4];

                if (bank.Accounts.All(p => p.Name != fromString))
                {
                    bank.Accounts.Add(new Person { Name = fromString });
                }
                if (bank.Accounts.All(p => p.Name != toString))
                {
                    bank.Accounts.Add(new Person { Name = toString });
                }
                
                bank.Transactions.Add(new Transaction
                {
                    Date = DateTime.Parse(dateString),
                    From = bank.Accounts.Find(p => p.Name == fromString),
                    To = bank.Accounts.Find(p => p.Name == toString),
                    Narrative = narrativeString,
                    Amount = Decimal.Parse(amountString)
                });
            }

            return bank;
        }

        public List<Transaction> AccountTransactions(string name)
        {
            return Transactions.Where(t => t.From.Name == name || t.To.Name == name).ToList();
        }

        public List<Transaction> AccountTransactions(Person person)
        {
            return Transactions.Where(t => t.From.Equals(person) || t.To.Equals(person)).ToList();
        }

        public List<Transaction> TransactionsTo(string name)
        {
            return Transactions.Where(t => t.To.Name == name).ToList();
        }
        
        public List<Transaction> TransactionsTo(Person person)
        {
            return Transactions.Where(t => t.To.Equals(person)).ToList();
        }
        
        public List<Transaction> TransactionsFrom(string name)
        {
            return Transactions.Where(t => t.From.Name == name).ToList();
        }
        
        public List<Transaction> TransactionsFrom(Person person)
        {
            return Transactions.Where(t => t.From.Equals(person)).ToList();
        }

        public decimal GetBalance(string name)
        {
            decimal totalOut = TransactionsFrom(name)
                .Select(t => t.Amount)
                .Aggregate(Decimal.Add);
            decimal totalIn = TransactionsTo(name)
                .Select(t => t.Amount)
                .Aggregate(Decimal.Add);
            return totalIn - totalOut;
        }
        
        public decimal GetBalance(Person person)
        {
            decimal totalOut = TransactionsFrom(person)
                .Select(t => t.Amount)
                .Aggregate(Decimal.Add);
            decimal totalIn = TransactionsTo(person)
                .Select(t => t.Amount)
                .Aggregate(Decimal.Add);
            return totalIn - totalOut;
        }
    }
}