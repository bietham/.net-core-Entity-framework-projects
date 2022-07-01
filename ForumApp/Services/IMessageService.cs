using AutoMapper;
using ForumApp.DTOModels;
using ForumApp.Store;
using ForumApp.Store.Models;
using ForumApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApp.Services
{
    public interface IMessageService
    {
        Task<IdentityUser> GetModerator(int Id);
        Task<TopicViewModel> MessageIndexAsync(int Id);

        Task CreateMessageAsync(MessageCreateViewModel model ,IdentityUser user);

        Task<MessageEditViewModel> GetEditMessageAsync(int Id);

        Task EditMessageAsync(MessageEditViewModel model, IdentityUser user);

        Task<MessageViewModel> GetDeleteMessageAsync(int Id);

        Task DeleteMessageAsync(MessageViewModel model);

        Task createAttachmentsAsync(List<IFormFile> Attachments, int messageId);

        Task<List<AttachmentViewModel>> GetAttachmentsAsync(int messageId);

        Task PostMessage(MessageEditDto model, int Id);
        Task EditMessage(MessageEditDto model, int Id);
        Task DeleteMessage(int Id);



    }

    public class MessageService : IMessageService
    {
        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
        private ApplicationDbContext _dbContext { get; }
        private IMapper _mapper { get; }

        public MessageService(ApplicationDbContext dbContext,
           IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IdentityUser> GetModerator(int Id)
        {
            var queryResult = await _dbContext.Topics
                .Include(y => y.Moderator)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (queryResult != null)
            {
                return queryResult.Moderator;
            }
            return null;
        }

            public async Task<TopicViewModel> MessageIndexAsync(int Id)
        {
            var topic = _dbContext.Topics
                .Include(x => x.Messages)
                .ThenInclude(y => y.Creator)
                .Include(x => x.Moderator)
                .FirstOrDefault(x => x.Id == Id);

            var Messages = await _dbContext.Messages
                .Where(b => b.TopicId == Id)
                .ToListAsync();

            var _Messages = _mapper.Map<List<MessageViewModel>>(Messages);

            var _MessagesAttachments = new List<MessageViewModel>();

            foreach(var _message in _Messages)
            {
                _message.Attachments = await GetAttachmentsAsync(_message.Id);
                _MessagesAttachments.Add(_message);
            }

            var _topic =_mapper.Map<TopicViewModel>(topic);

            _topic.Messages = _Messages;

            return _topic;
        }

        public async Task CreateMessageAsync(MessageCreateViewModel model, IdentityUser user)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model)); ;
            }


            //To avoid conflicts with FormFile format
            var _Attachments = new List<IFormFile> { };

            if (model.Attachments != null)
            {
                _Attachments.AddRange(model.Attachments);
                model.Attachments.Clear();
            }



            var Topic = await _dbContext.Topics.Include(x=> x.Moderator).FirstOrDefaultAsync(x => x.Id == model.TopicId);

            if (Topic == null)
            {
                throw new KeyNotFoundException($"ForumSection with Id {model.TopicId} was not found in database");
            }


            var newMessage = _mapper.Map<Messages>(model);

            newMessage.Topic = Topic;
            newMessage.Moderator = Topic.Moderator;
            newMessage.Creator = user;
            newMessage.CreatedAt = DateTime.Now;

            await _dbContext.Messages.AddAsync(newMessage);

            _dbContext.SaveChanges();

            var newMessageList = await _dbContext.Messages.ToListAsync();


            var i = new MessageEditViewModel { };

            if (_Attachments != null)
            {
                try
                {
                    await createAttachmentsAsync(_Attachments, newMessageList.Last().Id);
                }
                catch (InvalidOperationException ioe)
                {
                    throw new InvalidOperationException(ioe.Message);
                }
            }


            await _dbContext.SaveChangesAsync();
            
        }

        public async Task <MessageEditViewModel> GetEditMessageAsync(int Id)
        {
            var messageToEdit = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == Id);

            if (messageToEdit == null)
            {
                throw new ArgumentNullException("Message for editing was not found.");
            }

            var editViewModel = _mapper.Map<MessageEditViewModel>(messageToEdit);

            return (editViewModel);
        }

        public async Task EditMessageAsync(MessageEditViewModel model, IdentityUser user)
        {
            if (model == null)
            {
                throw new ArgumentNullException("Received model was null");
            }

            var MessageToEdit = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == model.Id);

            

            if (MessageToEdit == null)
            {
                throw new KeyNotFoundException("Could not find Topic to edit.");
            }
            

            var Topic = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == MessageToEdit.TopicId);

            if (Topic == null)
            {
                throw new Exception($"Could not find related Forum Section. Id={MessageToEdit.TopicId}");
            }



            MessageToEdit.Text = model.Text;
            MessageToEdit.Modified = DateTime.Now;
            MessageToEdit.TopicId = Topic.Id;
            MessageToEdit.Creator = user;


            await _dbContext.SaveChangesAsync();
        }

        public async Task<MessageViewModel> GetDeleteMessageAsync (int Id)
        {
            var MessageToDelete = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == Id);

            if (MessageToDelete == null)
            {
                throw new ArgumentNullException($"Null exception for ID:{Id} topic.");
            }
            var deleteViewModel = _mapper.Map<MessageViewModel>(MessageToDelete);

            deleteViewModel.TopicId = MessageToDelete.TopicId;

            return deleteViewModel;
        }

        public async Task DeleteMessageAsync(MessageViewModel model)
        {
            var MessageToBeDeleted = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (MessageToBeDeleted == null)
            {
                throw new KeyNotFoundException("Could not find Message to delete.");
            }

            _dbContext.Remove(MessageToBeDeleted);

            _dbContext.SaveChanges();
        }

        //Attachments Management
        public async Task<List<AttachmentViewModel>> GetAttachmentsAsync(int messageId)
        {
            var queryResult = await _dbContext.Attachments
                .Where(x => x.MessageId == messageId)
                .ToListAsync();
            return _mapper.Map<List<AttachmentViewModel>>(queryResult);
        }

        public async Task createAttachmentsAsync(List<IFormFile> newAttachments, int messageId)
        {
            List<Attachments> attachments = new List<Attachments> { };

            foreach (var file in newAttachments)
            {
                var extension = Path.GetExtension(file.FileName)?.Replace(".", "");
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException("Invalid file extension(s). Please only upload " + string.Join(", ", AllowedExtensions.ToArray()) + " files");
                }

                string Folder = "Files";
                string root = "wwwroot";
                string rootFolder = $"{root}/{Folder}";
                var fileId = Guid.NewGuid();
                string path = $"{Folder}/{fileId}.{extension}";
                string fullPath = $"{root}/{path}";


                bool exists = System.IO.Directory.Exists(rootFolder);

                if (!exists)
                    System.IO.Directory.CreateDirectory(rootFolder);

                using (var fileStream = new FileStream(fullPath,
                    FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                var attachment = new Attachments
                {
                    FileName = fileId.ToString(),
                    FilePath = path,
                    Created = DateTime.Now,
                    MessageId = messageId
                };
                attachments.Add(attachment);
            }

            await _dbContext.AddRangeAsync(attachments);
        }

        public async Task PostMessage(MessageEditDto model, int Id)
        {
            var topic = await _dbContext.Topics.FirstOrDefaultAsync(x => x.Id == Id);

            if (topic == null)
            {
                throw new KeyNotFoundException("Topic not found");
            }

            var newMessage = _mapper.Map<Messages>(model);

            newMessage.CreatedAt = DateTime.Now;
            newMessage.TopicId = topic.Id;

            await _dbContext.AddAsync(newMessage);
        }
        public async Task EditMessage(MessageEditDto model, int Id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == Id);

            if (message == null)
            {
                throw new KeyNotFoundException("Message for editing not found");
            }

            message.Text = model.Text;
            message.Modified = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMessage(int Id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(x => x.Id == Id);

            if (message == null)
            {
                throw new KeyNotFoundException("Topic for editing not found");

            }

            _dbContext.Remove(message);
        }

    }
}
