using AutoMapper;
using ChatApp.Data;
using ChatApp.Models.DTOs;

namespace ChatApp.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // config userDTO
            CreateMap<User, UserDTO>()
             .ReverseMap()
             .ForMember(dest => dest.Password, opt => opt.Ignore())
             .ForMember(dest => dest.UserName, opt => opt.Ignore())
             .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
             .ForMember(dest => dest.RefreshTokenExpired, opt => opt.Ignore())
             .ForMember(dest => dest.modifiedDate, opt => opt.Ignore())
             .ForMember(dest => dest.LastOnline, opt => opt.Ignore())
             .ForMember(dest => dest.UserType, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoomChat, opt => opt.Ignore())
             .ForMember(dest => dest.Messages, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoleMappings, opt => opt.Ignore());
            // config messages DTO
            CreateMap<Message, MessageDTO>().ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.RoomChat, opt => opt.Ignore());
            // config roomChatDTO
            CreateMap<RoomChat, RoomChatDTO>().ReverseMap()
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoomChat, opt => opt.Ignore());


            //example
            // config different property
            // CreateMap<Student, StudentDTO>().ReverseMap().ForMember(n => n.Name, opt => opt.MapFrom(x => x.studentName));
            // config ignore property
            // CreateMap<Student, StudentDTO>().ForMember(n => n.Name, opt => opt.Ignore());
            // config tranform property
            //CreateMap<Student, StudentDTO>().AddTransform<string>(n => string.IsNullOrEmpty(n) ? "No address " : n);
            // config mutiple
            // CreateMap<Student, StudentDTO>().ForMember(n => n.Name, opt => opt.MapFrom(n =>  string.IsNullOrEmpty(n.Name) ? "No name" : n.Name)).ReverseMap();
        }
    }
}
