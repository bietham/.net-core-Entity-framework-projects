using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ForumApp.Store;
using ForumApp.Store.Models;
using ForumApp.ViewModels;
using ForumApp.Services;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ForumApp.Store.Roles;
using Microsoft.AspNetCore.Mvc;

namespace ForumApp.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminController : Controller

    {
        private IAdminService _admin { get; }
        private ILogger<ForumSectionController> Logger { get; }
        private UserManager<IdentityUser> _userManager { get; }

        public AdminController(IAdminService admin, ILogger<ForumSectionController> logger, UserManager<IdentityUser> userManager)
        {
            _admin = admin;
            Logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _admin.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {

            var editModel = new ForumSectionEditViewModel { };
            try
            {
                editModel = await _admin.GetEditForumSectionAsync(Id);
                ViewBag.Moderators = await _admin.GetModerators();
                return View(editModel);
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = "Error occured";
                return View(editModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ForumSectionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _admin.EditForumSectionAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException.");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }
            catch (KeyNotFoundException kne)
            {
                Logger.LogError(kne, "Message for developer ArgumentNullException.");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }
            catch (ArgumentException ae)
            {
                Logger.LogError(ae, "Message for developer ArgumentNullException.");
                ViewBag.ErrorMessage = "Forum Section with this name already exists.";
                return View(model);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unexpect exception");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }
        }
    }
}
