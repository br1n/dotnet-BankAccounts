using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get; set;}
        [Required]
        public int Amount {get; set;}

        //Foreign Key to MainUser.UserId
        public int UserId {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;

        public MainUser CurrUser {get; set;}
    }
}