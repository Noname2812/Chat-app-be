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
             .ForMember(dest => dest.modifiedDate, opt => opt.Ignore())
             .ForMember(dest => dest.UserType, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoomChat, opt => opt.Ignore())
             .ForMember(dest => dest.Messages, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoleMappings, opt => opt.Ignore())
             .ForMember(dest => dest.IsOnline, opt => opt.Ignore())
             .ForMember(dest => dest.LastOnline, opt => opt.Ignore());


            // config messages DTO
            CreateMap<Message, MessageDTO>().ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.RoomChat, opt => opt.Ignore());
            // config roomChatDTO
            CreateMap<RoomChat, RoomChatDTO>().ReverseMap()
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoomChat, opt => opt.Ignore());

            // config friendship DTO
            CreateMap<Friendship, FriendShipDTO>().ReverseMap()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FriendId, opt => opt.MapFrom(src => src.FriendId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))

                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Friend, opt => opt.Ignore());
            // config friendDTO
            CreateMap<User, FriendDTO>().ReverseMap()
              .ForMember(dest => dest.Password, opt => opt.Ignore())
             .ForMember(dest => dest.UserName, opt => opt.Ignore())
             .ForMember(dest => dest.modifiedDate, opt => opt.Ignore())
             .ForMember(dest => dest.UserType, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoomChat, opt => opt.Ignore())
             .ForMember(dest => dest.Messages, opt => opt.Ignore())
             .ForMember(dest => dest.UserRoleMappings, opt => opt.Ignore())
             .ForMember(dest => dest.Email, opt => opt.Ignore())
             .ForMember(dest => dest.Address, opt => opt.Ignore())
             .ForMember(dest => dest.Phone, opt => opt.Ignore())
             .ForMember(dest => dest.createAt, opt => opt.Ignore())
             .ForMember(dest => dest.userTypeId, opt => opt.Ignore());



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
