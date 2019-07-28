using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using ImplicitServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ImplicitServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly TestUserStore _users;
        private readonly IClientStore _clientStore;
        public AccountController(TestUserStore users
            , IIdentityServerInteractionService interaction
            , IClientStore clientStore)
        {
            _users = users;
            _interaction = interaction;
            _clientStore = clientStore;
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
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (_users.ValidateCredentials(model.UserName, model.Password))
                {
                    var user = _users.FindByUsername(model.UserName);


                    //（保存）认证信息字典
                    AuthenticationProperties props = new AuthenticationProperties
                    {
                        IsPersistent = true,    //认证信息是否跨域有效
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))    //凭据有效时间
                    };

                    await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
                         HttpContext, user.SubjectId, user.Username, props);

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(returnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else if (string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
                ModelState.AddModelError(string.Empty, "errir");
            }
            return View(model);
        }
    }
}