using AutoMapper;
using ForumApp.DTOModels;
using ForumApp.Store.Models;
using ForumApp.ViewModels;

namespace ForumApp
{
    public class WebAppMapperProfile : Profile
    {

        public WebAppMapperProfile()
        {
            CreateMap<ForumSections, ForumSectionViewModel>();
            CreateMap<ForumSections, ForumSectionEditViewModel>();
            CreateMap<ForumSectionCreateViewModel, ForumSections>();
            CreateMap<ForumSections, ForumSectionDto>();
            CreateMap<ForumSections, ForumSectionEditDto>();
            CreateMap<ForumSectionCreateDto, ForumSections>();
            CreateMap<Topics, TopicViewModel>();
            CreateMap<Topics, TopicEditViewModel>();
            CreateMap<TopicCreateViewModel, Topics>();
            CreateMap<Messages, MessageViewModel>();
            CreateMap<Messages, MessageEditViewModel>();
            CreateMap<MessageCreateViewModel, Messages>();
            CreateMap<Messages, MessagesDto>();
            CreateMap<Messages, MessageEditDto>();
            CreateMap<MessageEditDto, Messages>();
            CreateMap<Attachments, AttachmentViewModel>();
            CreateMap<AttachmentsCreateViewModel, Attachments>();

        }
    }
}
