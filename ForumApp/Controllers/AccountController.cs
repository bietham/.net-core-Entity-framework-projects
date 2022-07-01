using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using ForumApp.ViewModels.Account;

namespace ForumApp.Controllers
{

    [AllowAnonymous]
    public class AccountController : Controller
    {
        
            private UserManager<IdentityUser> UserManager { get; }
            private SignInManager<IdentityUser> SignInManager { get; }
            public AccountController(UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager)
            {
                UserManager = userManager;
                SignInManager = signInManager;
            }

            public IActionResult Register()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegisterViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var identityResult = await UserManager.CreateAsync(
                    new IdentityUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = model.Email,
                        UserName = model.Email
                    },
                    model.Password);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, identityResult.Errors.First().Description);
                return View(model);
            }

            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Login(LoginViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                //Обработать ошибки менеджера и вывести юзеру сообщени о причине отказа
                return View(model);
            }

            [Authorize]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Logout()
            {
                await SignInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
        }
    }

