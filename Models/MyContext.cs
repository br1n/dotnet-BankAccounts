using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAccounts
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options){}
        // Your DB Sets go HERE!
        public DbSet<MainUser> Users {get; set;}
        public DbSet<Transaction> Transactions {get; set;}
    }
}