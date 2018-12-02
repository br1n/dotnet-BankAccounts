using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankAccounts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BankAccounts.Controllers
{
    public class AccountController : Controller
    {
        public MainUser ActiveUser
        //produces ActiveUser object that matches that of SessionUser
        {
            get { return dbContext.Users.Include(u => u.Transactions)
            .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("user_Id")); }
        }

        private MyContext dbContext;
        public AccountController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("account/{user_Id}")]
        public IActionResult Index(int user_Id)
        {
            //validates if user logged in
            if(ActiveUser == null)
            {
                return RedirectToAction("Register", "Home");
            }

            var myUser = ActiveUser;
            ViewBag.User = myUser;
            ViewBag.Transactions = dbContext.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == ActiveUser.UserId);

            return View();
        }

        [HttpPost("transaction")]
        public IActionResult CreateTransaction(Transaction newTrans) 
        {
            MainUser transUser = ActiveUser;

            if(ActiveUser == null)
            {
                return RedirectToAction("Register", "Home");
            }

            if(ModelState.IsValid)
            {
                if(newTrans.Amount + transUser.Balance > 0)
                {
                    // specifies UserId
                    newTrans.UserId = transUser.UserId;
                    int? user_Id = newTrans.UserId;
                    // Add Transaction
                    dbContext.Transactions.Add(newTrans);
                    dbContext.SaveChanges();

                    return Redirect($"account/{user_Id}");
                }
                else
                {
                    ViewBag.Error = "Insufficient Funds";
                }
            }
            // var myUser = ActiveUser;
            ViewBag.User = transUser;
            ViewBag.Transactions = dbContext.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == ActiveUser.UserId);
            return View("Index");

        }
    }
}