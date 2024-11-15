using AutoMapper;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.Helpers
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, Account>();
            CreateMap<ArticleCreateVM, Article>();
            CreateMap<ArticleEditVM, Article>().ReverseMap();
            CreateMap<ArticleDeleteVM, Article>().ReverseMap();
            CreateMap<AccountEditVM, Account>().ReverseMap();
            CreateMap<AccountDeleteVM, Account>().ReverseMap();
            CreateMap<CategoryCreateVM, Category>().ReverseMap();
            CreateMap<CategoryEditVM, Category>().ReverseMap();
            CreateMap<TagCreateVM, Tag>().ReverseMap();
            CreateMap<TagEditVM, Tag>().ReverseMap();
            CreateMap<AccountEditInfoVM, Account>().ReverseMap();
        }
    }
}
