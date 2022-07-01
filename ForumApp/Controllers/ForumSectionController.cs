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

namespace ForumApp.Controllers
{
    public class ForumSectionController : Controller
    {
        
        private IForumSectionService _forumSection { get; }
        private ILogger<ForumSectionController> Logger { get; }

        private UserManager<IdentityUser> _userManager { get; }

        public ForumSectionController(IForumSectionService forumSection, ILogger<ForumSectionController> logger, UserManager<IdentityUser> userManager)
        {
            _forumSection = forumSection;
            Logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var forumSections = await _forumSection.ForumSectionIndexAsync();
            ViewBag.UserName = User.Identity.Name;
            ViewBag.IsAdmin = User.IsInRole(Roles.Admin);
            return View(forumSections);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult Create()
        {
            
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task <IActionResult> Create(ForumSectionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!(await _forumSection.GetForumSectionsNameAsync(model.Name) is null))
            {
                ModelState.AddModelError("", $"Section {model.Name} already exists");
                return View(model);
            }

            try
            {
                await _forumSection.CreateForumSectionAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Unexpect exception");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }

            
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]

        public async Task<IActionResult> Edit(int Id)
        {
            
            var editModel = new ForumSectionEditViewModel { };
            try
            {
                editModel = await _forumSection.GetEditForumSectionAsync(Id);
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(ForumSectionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           try
            {
                await _forumSection.EditForumSectionAsync(model);
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int Id)
        {
            var deleteModel = new ForumSectionViewModel { };
            try
            {
                deleteModel = await _forumSection.GetDeleteForumSectionAsync(Id);
                return View(deleteModel);
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = "Error occured";
                return View(deleteModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(ForumSectionViewModel model)
        {
            try
            {
                await _forumSection.DeleteForumSectionAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = "Error occured";
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
