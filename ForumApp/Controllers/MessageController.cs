using Microsoft.AspNetCore.Mvc;
using ForumApp.Services;
using ForumApp.Store;
using ForumApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ForumApp.Store.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ForumApp.Store.Roles;

namespace ForumApp.Controllers
{
    public class MessageController : Controller
    {
        private ApplicationDbContext _context;
        private IMessageService _message;
        private ILogger<ForumSectionController> Logger { get; }

        private UserManager<IdentityUser> _userManager;

        public MessageController(ApplicationDbContext context, IMessageService message, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _message = message;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = await _message.MessageIndexAsync(Id);
            ViewBag.IsAdmin = User.IsInRole(Roles.Admin);
            ViewBag.UserName = User.Identity.Name;

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create(int Id)
        {
            ViewBag.TopicId = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(MessageCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            

            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                await _message.CreateMessageAsync(model, user);
                return RedirectToAction("Index", new { Id = model.TopicId });
            }
            catch (KeyNotFoundException kne)
            {
                Logger.LogError(kne, "Message for developer KeyNotFounException");
                ViewBag.ErrorMessage = $"{kne.Message}";
                return View(model);
            }
            catch (InvalidOperationException ioe)
            {
                Logger.LogError(ioe, "Message for developer InvalidOperationException");
                ViewBag.ErrorMessage = $"{ioe.Message}";
                return View(model);
            }
            
        }

        [Authorize]
        public async Task<IActionResult> Edit(int Id)
        {
            var editViewModel = new MessageEditViewModel { };
            try
            {
                editViewModel = await _message.GetEditMessageAsync(Id);
                ViewBag.TopicId = editViewModel.TopicId;
                return View(editViewModel);
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = $"{ane.Message}";
                return View(editViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(MessageEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                await _message.EditMessageAsync(model, user);
                return RedirectToAction("Index", new TopicViewModel { Id = model.TopicId });
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

        [Authorize]
        public async Task<IActionResult> Delete(int Id)
        {
            var deleteModel = new MessageViewModel { };
            try
            {
                deleteModel = await _message.GetDeleteMessageAsync(Id);
                return View(deleteModel);
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = $"{ane.Message}";
                return View(deleteModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(MessageViewModel model)
        {
            try
            {
                await _message.DeleteMessageAsync(model);
                return RedirectToAction("Index", new { Id = model.TopicId });
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
