using System;
using System.Collections.Generic;

namespace BankingApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public decimal CreditLimit { get; set; } = 0;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        private IList<Operation> _operations;
        public IList<Operation> Operations
        {
            get
            { return _operations ?? (_operations = new List<Operation>()); }
            set
            { _operations = value; }
        }


        public void MakeDeposit(decimal amount, string description = null)
        {
            Balance += amount;
            AddEntry(amount, EventType.Deposit, description);
        }

        // Withdrawal method, return true if succesful, false otherwise
        public bool MakeWithdrawal(decimal amount, string description = null)
        {
            if (Balance - amount > CreditLimit)
            {
                Balance -= amount;
                AddEntry(amount, EventType.Withdrawal, description);
                return true;
            }
            else
            {
                AddEntry(amount, EventType.Failed, $"{EventType.Failed.ToString()}: Unsufficient funds.");
                return false;
            }
        }

        public void TransferMoney(BankAccount otherAccount, decimal amount)
        {
            // proceed to Deposit only if Withdrawal succeeds,
            if (MakeWithdrawal(amount, $"Transfer to account #{otherAccount.Id}"))
            {
                otherAccount.MakeDeposit(amount, $"Transfer from account #{this.Id}");
            }
        }

        public void AddEntry(decimal amount, EventType type, string description = null)
        {
            Operation entry = new Operation()
            {
                Amount = amount,
                OwnerAccount = this,
                OwnerAccountId = this.Id,
                Type = type,
                Description = !string.IsNullOrEmpty(description) ? description : type.ToString()
            };
            Operations.Add(entry);
        }
    }
}
