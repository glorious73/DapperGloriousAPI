using Application.DTO.Account;
using AutoMapper;
using Infrastructure.UserRepository;

namespace Application.Queries;

public class FilterUserQueryMapProfile : Profile
{
    public FilterUserQueryMapProfile()
    {
        CreateMap<FilterUserDTO, UserQueryFilter>()
            .ForMember(query => query.EmailAddress, option => option.MapFrom(src => src.Search))
            .ForMember(query => query.Limit, option => option.MapFrom(src => src.PageSize))
            .ForMember(query => query.Offset, option => option.MapFrom(src => (src.PageNumber-1)*src.PageSize));
    }
}