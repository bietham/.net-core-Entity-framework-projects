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
    public interface IForumSectionService
    {
        Task<ForumSections> GetForumSectionsNameAsync(string Name);

        Task<IdentityUser> GetModerator(int Id);

        Task<List<ForumSectionViewModel>> ForumSectionIndexAsync();

        Task CreateForumSectionAsync(ForumSectionCreateViewModel model);

        Task<ForumSectionEditViewModel> GetEditForumSectionAsync(int Id);

        Task EditForumSectionAsync(ForumSectionEditViewModel model);

        Task <ForumSectionViewModel> GetDeleteForumSectionAsync(int Id);

        Task DeleteForumSectionAsync(ForumSectionViewModel model);

        Task<List<ForumSectionDto>> GetAllSections();

        Task PostSection(ForumSectionCreateDto model);

        Task EditSection(ForumSectionEditDto model, int Id);

    }

    public class ForumSectionService : IForumSectionService
    {
        private ApplicationDbContext _dbContext { get; }

        private IMapper _mapper { get; }

        public ForumSectionService(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ForumSections> GetForumSectionsNameAsync(string Name)
        {
            var queryResult = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Name == Name);

            return queryResult;
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

        public async Task<List<ForumSectionViewModel>> ForumSectionIndexAsync()
        {

            var queryResult = await _dbContext.ForumSections.ToListAsync();
            var model =  _mapper.Map<List<ForumSectionViewModel>>(queryResult);
            foreach (var item in model)
            {
                item.Moderator = await GetModerator(item.Id);
            }
            return model;

        }
        public async Task CreateForumSectionAsync(ForumSectionCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model)); ;
            }

            var newForumSection = _mapper.Map<ForumSections>(model);

            //var newForumSection = new ForumSections
            //{
            //    Name = model.Name,
            //    Description = model.Description
            //};

            _dbContext.ForumSections.Add(newForumSection);

            await _dbContext.SaveChangesAsync();


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

            var forumSectionSame = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Name == model.Name && x.Id != model.Id);

            if (forumSectionToEdit == null)
            {
                throw new KeyNotFoundException("Could not find Forum Section to edit.");
            }
            if (!(forumSectionSame == null))
            {
                throw new ArgumentException("This Forum Section name already exists.");
            }

            forumSectionToEdit.Name = model.Name;
            forumSectionToEdit.Description = model.Description;
            forumSectionToEdit.Id = model.Id;

            await _dbContext.SaveChangesAsync();


        }

        public async Task<ForumSectionViewModel> GetDeleteForumSectionAsync(int Id)
        {
            var sectionToDelete = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == Id);

            if (sectionToDelete == null)
            {
                throw new ArgumentNullException($"Null exception for ID:{Id} forum section.");
            }
            var deleteViewModel = _mapper.Map<ForumSectionViewModel>(sectionToDelete);

            return deleteViewModel;
        }
        public async Task DeleteForumSectionAsync(ForumSectionViewModel model)
        {


            var forumSectionToBeDeleted = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (forumSectionToBeDeleted == null)
            {
                throw new KeyNotFoundException("Could not find Forum Section to delete.");
            }

            _dbContext.Remove(forumSectionToBeDeleted);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ForumSectionDto>> GetAllSections()
        {
            var sections = await _dbContext.ForumSections.ToListAsync();

            var _sections = _mapper.Map<List<ForumSectionDto>>(sections);

            return _sections;
        }
        public async Task PostSection(ForumSectionCreateDto model)
        {
            var section = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Name == model.Name);

            if (section != null)
            {
                throw new ArgumentException("Section with this name already exists");
            }

            var newSection = _mapper.Map<ForumSections>(model);

           
            await _dbContext.ForumSections.AddAsync(newSection);
            
        }
        public async Task EditSection(ForumSectionEditDto model, int Id)
        {
            var section = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Name == model.Name);

            if (section != null)
            {
                throw new ArgumentException("Section with this name already exists");
            }

            var sectionToEdit = await _dbContext.ForumSections.FirstOrDefaultAsync(x => x.Id == Id);

            if (sectionToEdit == null)
            {
                throw new KeyNotFoundException("Could not find section to edit");
            }
            sectionToEdit.Name = model.Name;
            sectionToEdit.Description = model.Description;

            await _dbContext.SaveChangesAsync();
        }
        

    }



}
    


