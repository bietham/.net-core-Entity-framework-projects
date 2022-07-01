using AutoMapper;
using ForumApp.DTOModels;
using ForumApp.Store;
using ForumApp.Store.Models;
using ForumApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApp.Services
{
    public interface ITopicService
    {

        Task<IdentityUser> GetModerator(int Id);
        Task<ForumSectionViewModel> TopicIndexAsync(int Id);

        Task CreateTopicAsync(TopicCreateViewModel model, IdentityUser user);

        Task<TopicEditViewModel> GetEditTopicAsync(int Id);

        Task EditTopicAsync(TopicEditViewModel model, IdentityUser user);

        Task<TopicViewModel> GetDeleteTopicAsync(int Id);

        Task DeleteTopicAsync(TopicViewModel model);
        Task<List<TopicDto>> GetAllTopics(int Id);
        Task PostTopic(TopicCreateDto model, int Id);
        Task EditTopic(TopicEditDto model, int Id);
        Task DeleteTopic(int Id);
        Task<List<MessagesDto>> GetTopicMessages(int Id);
    }

    public class TopicService : ITopicService
    {
        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };

        private ApplicationDbContext _dbContext { get; }

        private IMapper _mapper { get; }


        public TopicService(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IdentityUser> GetModerator(int Id)
        {
            var queryResult = await _dbContext.ModeratedSections
                .Include(y => y.Moderator)
                .FirstOrDefaultAsync(x => x.ForumSectionId == Id);
            if (queryResult != null)
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == queryResult.ModeratorName);
            }
            return null;
        }
        public async Task<ForumSectionViewModel> TopicIndexAsync(int Id)
        {

            var queryResult = await _dbContext.ForumSections
                .Include(x => x.Topics)
                .ThenInclude(y => y.Creator)
                .Include(x => x.Topics)
                .ThenInclude(y => y.Moderator)
                .FirstOrDefaultAsync(x => x.Id == Id);



            var model = _mapper.Map<ForumSectionViewModel>(queryResult);

            model.Moderator = await GetModerator(model.Id);

            return model;

        }

        public async Task CreateTopicAsync(TopicCreateViewModel model, IdentityUser user)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model)); ;
            }
            var topicSameName = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Name == model.Name);

            if (!(topicSameName == null))
            {
                throw new ArgumentException("Topic with this name already exists.");
            }

            var forumSection = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == model.ForumSectionId);

            if (forumSection == null)
            {
                throw new KeyNotFoundException($"ForumSection with Id {model.ForumSectionId} was not found in database");
            }

            var newTopic = _mapper.Map<Topics>(model);

            newTopic.ForumSection = forumSection;
            newTopic.Creator = user;
            newTopic.Moderator = await GetModerator(forumSection.Id);


            await _dbContext.Topics.AddAsync(newTopic);

            await _dbContext.SaveChangesAsync();


        }

        public async Task<TopicEditViewModel> GetEditTopicAsync(int Id)
        {

            var topicToEdit = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == Id);

            if (topicToEdit == null)
            {
                throw new ArgumentNullException("Topic for editing was not found.");
            }

            var editViewModel = _mapper.Map<TopicEditViewModel>(topicToEdit);

            return (editViewModel);
        }

        public async Task EditTopicAsync(TopicEditViewModel model, IdentityUser user)
        {

            if (model == null)
            {
                throw new ArgumentNullException("Received model was null");
            }

            var TopicToEdit = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == model.Id);

            var TopicSame = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Name == model.Name && x.Id != model.Id);

            if (TopicToEdit == null)
            {
                throw new KeyNotFoundException("Could not find Topic to edit.");
            }
            if (!(TopicSame == null))
            {
                throw new ArgumentException("This topic name already exists.");
            }

            var ForumSection = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == TopicToEdit.ForumSectionId);

            if (ForumSection == null)
            {
                throw new Exception($"Could not find related Forum Section. Id={TopicToEdit.ForumSectionId}");
            }



            TopicToEdit.Name = model.Name;
            TopicToEdit.Id = model.Id;
            TopicToEdit.Description = model.Description;
            TopicToEdit.ForumSectionId = ForumSection.Id;
            TopicToEdit.Creator = user;

            await _dbContext.SaveChangesAsync();
        }
        public async Task<TopicViewModel> GetDeleteTopicAsync(int Id)
        {
            var TopicToDelete = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == Id);

            if (TopicToDelete == null)
            {
                throw new ArgumentNullException($"Null exception for ID:{Id} topic.");
            }
            var deleteViewModel = _mapper.Map<TopicViewModel>(TopicToDelete);

            return deleteViewModel;
        }

        public async Task DeleteTopicAsync(TopicViewModel model)
        {
            var TopicToBeDeleted = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (TopicToBeDeleted == null)
            {
                throw new KeyNotFoundException("Could not find Forum Section to delete.");
            }

            _dbContext.Remove(TopicToBeDeleted);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TopicDto>> GetAllTopics(int Id)
        {
            var section = await _dbContext.ForumSections.Include(x => x.Topics).FirstOrDefaultAsync(x => x.Id == Id);

            if (section == null)
            {
                throw new KeyNotFoundException("Section not found");
            }

            return _mapper.Map<List<TopicDto>>(section.Topics);
        }

        public async Task PostTopic(TopicCreateDto model, int Id)
        {
            var section = await _dbContext.ForumSections.Include(x => x.Topics).FirstOrDefaultAsync(x => x.Id == Id);

            if (section == null)
            {
                throw new KeyNotFoundException("Section not found");
            }

            var newTopic = _mapper.Map<Topics>(model);

            newTopic.CreatedAt = DateTime.Now;
            newTopic.ForumSectionId = section.Id;

            await _dbContext.AddAsync(newTopic);
        }

        public async Task EditTopic(TopicEditDto model, int Id)
        {
            var topic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == Id);

            if (topic == null)
            {
                throw new KeyNotFoundException("Topic for editing not found");
            }

            var sameName = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Name == model.Name);

            if (sameName != null)
            {
                throw new ArgumentException("Topic with this name already exists");
            }

            topic.Name = model.Name;
            topic.Description = model.Description;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTopic(int Id)
        {
            var topic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == Id);

            if (topic == null)
            {
                throw new KeyNotFoundException("Topic for editing not found");

            }

            _dbContext.Remove(topic);
        }

        public async Task<List<MessagesDto>> GetTopicMessages(int Id)
        {
            var topic = await _dbContext.Topics.Include(y => y.Messages).FirstOrDefaultAsync(x => x.Id == Id);

            if (topic == null)
            {
                throw new KeyNotFoundException("Topic not found");
            }

            var messages = _mapper.Map<List<MessagesDto>>(topic.Messages);

            return messages;
        }
    }
}
