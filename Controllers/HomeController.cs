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
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(MainUser newUser)
        {
            if(ModelState.IsValid)
            {   
                // checks db for existing user Email
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "This Email is already in use!");
                    return View("Register");
                }

                //hash password!
                //passwordhasher of MainUser type
                PasswordHasher<MainUser> hasher = new PasswordHasher<MainUser>();
                string hashedPw = hasher.HashPassword(newUser, newUser.Password);
                // update user model with hashedpassword
                newUser.Password = hashedPw;

                //add new user to DB
                var newUserAdded = dbContext.Users.Add(newUser).Entity;
                dbContext.SaveChanges();

                //log user into session
                HttpContext.Session.SetInt32("user_Id", newUserAdded.UserId);

                return Redirect("login");
            }
            return View("Register");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                // checks db for submitted Email
                MainUser toLogin = dbContext.Users.FirstOrDefault(u => u.Email == user.EmailAttempt);

                if(toLogin == null)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Login");
                }

                //verify hashed pw
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, toLogin.Password, user.PasswordAttempt);
                if(result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("EmailAttempt", "Invalid Email/Password");
                    return View("Login");
                }
                //log user into session
                HttpContext.Session.SetInt32("user_Id", toLogin.UserId);
                //passes user_Id 
                int? user_Id = toLogin.UserId;
                return Redirect($"account/{user_Id}");
            }
            return View("Login");
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }
    }
}
