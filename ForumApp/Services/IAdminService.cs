using AutoMapper;
using ForumApp.Store;
using ForumApp.Store.Models;
using ForumApp.Store.Roles;
using ForumApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ForumApp.Services
{
    public interface IAdminService
    {
        Task<IdentityUser> GetModeratorAsync(int Id);
        Task<List<ForumSectionViewModel>> GetAllAsync();
        Task<ForumSectionEditViewModel> GetEditForumSectionAsync(int Id);
        Task EditForumSectionAsync(ForumSectionEditViewModel model);

        Task UpdateTopic(IdentityUser moderator, int sectionId);
        Task UpdateMessage(IdentityUser moderator, int sectionId);

        Task<List<IdentityUser>> GetModerators();
    }
    public class AdminService : IAdminService
    {
        private ApplicationDbContext _dbContext { get; }
        private IMapper _mapper { get; }
        public AdminService(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IdentityUser> GetModeratorAsync(int Id)
        {
            var queryResult = _dbContext.ModeratedSections
                .FirstOrDefault(x => x.ForumSectionId == Id);
            if (queryResult != null)
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == queryResult.ModeratorName);
            }
            return null;

        }
        public async Task<List<IdentityUser>> GetModerators()
        {
            var moderatorId = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == Roles.Moderator);
            var moderatorIds = await _dbContext.UserRoles.Where(y => y.RoleId == moderatorId.Id).ToListAsync();
            var moderatorList = new List<IdentityUser>{ }; 

            foreach(var _moderator in moderatorIds)
            {
                var moderator = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == _moderator.UserId);
                moderatorList.Add(moderator);
            }
            
            return moderatorList;
        }
        public async Task<List<ForumSectionViewModel>> GetAllAsync()
        {
            var queryResult = await _dbContext.ForumSections.ToListAsync();
            var model = _mapper.Map<List<ForumSectionViewModel>>(queryResult);
            foreach (var item in model)
            {
                item.Moderator = await GetModeratorAsync(item.Id);
            }
            return model;
        }

        public async Task<ForumSectionEditViewModel> GetEditForumSectionAsync(int Id)
        {
            var sectionToEdit = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == Id);

            if (sectionToEdit == null)
            {
                throw new ArgumentNullException($"Null exception for ID:{Id} forum section.");
            }
            var editViewModel = _mapper.Map<ForumSectionEditViewModel>(sectionToEdit);

            return editViewModel;
        }

        public async Task EditForumSectionAsync(ForumSectionEditViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("Received model was null");
            }

            var forumSectionToEdit = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == model.Id);

            var ModeratorSectionToEdit = await _dbContext.ModeratedSections
                .Include(x => x.Moderator)
                .FirstOrDefaultAsync(y => y.ForumSectionId == model.Id);

            var moderator = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == model.ModeratorEmail);

            if (forumSectionToEdit == null)
            {
                throw new KeyNotFoundException("Could not find Forum Section to edit.");
            }

            var newModeratorSection = new ModeratedSections { };

            if (ModeratorSectionToEdit == null)
            {
                newModeratorSection.ForumSectionId = model.Id;
                newModeratorSection.ModeratorName = model.ModeratorEmail;
                newModeratorSection.Moderator = moderator;
                await _dbContext.AddAsync(newModeratorSection);
            }
            else
            {
                ModeratorSectionToEdit.Moderator = moderator;
                ModeratorSectionToEdit.ModeratorName = model.ModeratorEmail;
            }
            await UpdateTopic(moderator, model.Id);

            await _dbContext.SaveChangesAsync();

        }
        public async Task UpdateTopic(IdentityUser moderator , int sectionId)
        {
            var topicsToUpdate = await _dbContext.Topics.Where(x => x.ForumSectionId == sectionId).ToListAsync();

            if (topicsToUpdate != null)
            {
                foreach (var topic in topicsToUpdate)
                {
                    topic.Moderator = moderator;
                     await UpdateMessage(moderator, topic.Id);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateMessage(IdentityUser moderator, int topicId)
        {
            var messagesToUpdate = await _dbContext.Messages.Where(x => x.TopicId == topicId).ToListAsync();

            if (messagesToUpdate != null)
            {
                foreach (var message in messagesToUpdate)
                {
                    message.Moderator = moderator;
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
