using ForumApp.Services;
using ForumApp.Store;
using ForumApp.Store.Roles;
using ForumApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApp.Controllers
{
    public class TopicController : Controller
    {
        private ITopicService _topic { get; }
        private ILogger<TopicController> Logger { get; }

        private UserManager<IdentityUser> _userManager { get; }

        public TopicController( ITopicService topic, ILogger<TopicController> logger, UserManager<IdentityUser> userManager)
        {

            _topic = topic;
            Logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int Id)
        {
            var model = await _topic.TopicIndexAsync(Id);
            ViewBag.IsAdmin = User.IsInRole(Roles.Admin);
            ViewBag.UserName = User.Identity.Name;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int Id)
        {
            ViewBag.ForumSectionId = Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(TopicCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                await _topic.CreateTopicAsync(model, user);
                return RedirectToAction("Index", new { Id = model.ForumSectionId });
            }
            catch (ArgumentNullException ane)
            {
                Logger.LogError(ane, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = "Error occured";
                return View(model);
            }
            catch (ArgumentException ae)
            {
                Logger.LogError(ae, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = $"{ae.Message}";
                return View(model);
            }
            catch (KeyNotFoundException kne)
            {
                Logger.LogError(kne, "Message for developer ArgumentNullException");
                ViewBag.ErrorMessage = $"{kne.Message}";
                return View(model);
            }
            
        }

        [Authorize]
        public async Task<IActionResult> Edit(int Id)
        {
            var editViewModel = new TopicEditViewModel { };
            try
            {
                editViewModel = await _topic.GetEditTopicAsync(Id);
                ViewBag.ForumSectionId = editViewModel.ForumSectionId;
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
        public async Task<IActionResult> Edit(TopicEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                await _topic.EditTopicAsync(model, user);
                return RedirectToAction("Index", new {Id = model.ForumSectionId});
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

            var deleteModel = new TopicViewModel { };
            try
            {
                deleteModel = await _topic.GetDeleteTopicAsync(Id);
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
        public async Task<IActionResult> Delete(TopicViewModel model)
        {
            try
            {
                await _topic.DeleteTopicAsync(model);
                return RedirectToAction("Index", new {Id = model.ForumSectionId});
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
