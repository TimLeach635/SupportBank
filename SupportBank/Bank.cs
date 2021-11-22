using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;

namespace SupportBank
{
    public class Bank
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        public List<Person> People = new();
        public List<Transaction> Transactions = new();

        public static Bank FromFile(string path)
        {
            Logger.Debug($"Reading transactions from file {path}");
            using var reader = new StreamReader(path);

            Bank bank = new Bank();
            
            // discard header row
            reader.ReadLine();

            var lineNumber = 2;
            
            while (!reader.EndOfStream)
            {
                Logger.Debug($"Reading line {lineNumber} of {path}...");
                lineNumber++;
                var line = reader.ReadLine();
                if (line is null)
                    break;
                var values = line.Split(",");

                string dateString = values[0];
                string fromString = values[1];
                string toString = values[2];
                string narrativeString = values[3];
                string amountString = values[4];

                if (bank.People.All(p => p.Name != fromString))
                {
                    bank.People.Add(new Person { Name = fromString });
                }
                if (bank.People.All(p => p.Name != toString))
                {
                    bank.People.Add(new Person { Name = toString });
                }

                try
                {
                    bank.Transactions.Add(new Transaction
                    {
                        Date = DateTime.Parse(dateString),
                        From = bank.People.Find(p => p.Name == fromString),
                        To = bank.People.Find(p => p.Name == toString),
                        Narrative = narrativeString,
                        Amount = Decimal.Parse(amountString)
                    });
                }
                catch (Exception e)
                {
                    Logger.Error($"Error converting {path} line {lineNumber} to a Transaction.");
                    Logger.Error($"Line: {line}");
                    throw new InvalidCastException($"Failed to create a Transaction from {path}, line {lineNumber}", e);
                }
            }

            return bank;
        }

        public static Bank FromJsonFile(string path)
        {
            using var reader = new StreamReader(path);

            Bank bank = new Bank();

            var file = reader.ReadToEnd();
            var fileDecoded = JsonConvert.DeserializeObject(file);

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