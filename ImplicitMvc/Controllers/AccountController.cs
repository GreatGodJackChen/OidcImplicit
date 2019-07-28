using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using ImplicitMvc.Controllers;
using ImplicitMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ImplicitApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _users;
        public AccountController(TestUserStore users)
        {
            _users = users;
        }

        public IActionResult Index()
        {
            return View("Login");
        }
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
            }
            var user = _users.FindByUsername(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.UserName), "Username not exists");
            }
            else
            {
                if (user.Password.Equals(model.Password))
                {
                    //（保存）认证信息字典
                    AuthenticationProperties props = new AuthenticationProperties
                    {
                        IsPersistent = true,    //认证信息是否跨域有效
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))    //凭据有效时间
                    };

                    await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
                        HttpContext, user.SubjectId, user.Username, props);

                    return RedirectToLoacl(returnUrl);
                }

                ModelState.AddModelError(nameof(model.Password), "Password Error");
            }
            return View(model);
        }
        private IActionResult RedirectToLoacl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}