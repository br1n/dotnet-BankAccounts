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

        [HttpGet("account/{id}")]
        public IActionResult Index(int id)
        {
            //check if User is Actively in Session
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
            if(ActiveUser == null)
            {
                return RedirectToAction("Register", "Home");
            }

            if(ModelState.IsValid)
            {
                dbContext.Transactions.Add(newTrans);
                dbContext.SaveChanges();
                return RedirectToAction("");
            }
            var myUser = ActiveUser;
            ViewBag.User = myUser;
            ViewBag.Transactions = dbContext.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.UserId == ActiveUser.UserId);
            return View("Index");

        }
    }
}