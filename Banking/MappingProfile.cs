using AutoMapper;
using Banking.DTO;
using Banking.Entity;

namespace Banking
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define your mappings here
            CreateMap<UserRegisterDto, User>().ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
            CreateMap<User, RegistrationResponseDto>();
            
            // Account mappings
            CreateMap<Account, AccountResponseDto>();
            CreateMap<Account, AccountResponseFullDto>();
            CreateMap<Account, AccountResponseSimpleDto>();
            CreateMap<User, UserDto>();
        }
    }
}
