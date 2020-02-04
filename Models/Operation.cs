using System;
namespace BankingApp.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
        public int OwnerAccountId { get; set; }
        public BankAccount OwnerAccount { get; set; }
    }
}
