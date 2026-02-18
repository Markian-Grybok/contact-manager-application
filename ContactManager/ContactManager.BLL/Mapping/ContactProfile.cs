using AutoMapper;
using ContactManager.BLL.Models;
using ContactManager.DAL.Entities;

namespace ContactManager.BLL.Mapping;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<ContactCreateViewModel, Contacts>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Contacts, ContactCreateViewModel>().ReverseMap();

        CreateMap<ContactEditViewModel, Contacts>();
        CreateMap<Contacts, ContactEditViewModel>();

        CreateMap<Contacts, ContactResponse>();
        CreateMap<ContactResponse, Contacts>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<ContactCreateViewModel, ContactResponse>();
        CreateMap<ContactResponse, ContactCreateViewModel>();

        CreateMap<ContactEditViewModel, ContactResponse>();
        CreateMap<ContactResponse, ContactEditViewModel>();
    }
}
