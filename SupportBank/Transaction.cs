using System;

namespace SupportBank
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public Person From { get; set; }
        public Person To { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }

        public override string ToString() => $"{Date:d}: £{Amount:N2} from {From.Name} to {To.Name} for {Narrative}";
    }
}