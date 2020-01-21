using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<BankAccount> BankAccounts { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
}
