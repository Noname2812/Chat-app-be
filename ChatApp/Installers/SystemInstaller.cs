
using ChatApp.Configurations;
using ChatApp.Data;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.UserRoomChats;
using ChatApp.Data.Repository.Users;
using ChatApp.Data.Repository;
using ChatApp.Interfaces;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ChatApp.Hubs;
using ChatApp.Services.ChatServices;

namespace ChatApp.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallerService(IServiceCollection services, IConfiguration configuration)
        {
            // add signalR
            services.AddSignalR();
            //add autoMapper
            services.AddAutoMapper(typeof(AutoMapperConfig));
            //DI
            services.AddScoped<IHubService, HubService>();
            services.AddSingleton<IPhotoService, PhotoService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoomChatRepository, UserRoomChatRepository>();
            services.AddScoped<IRoomChatRepository, RoomChatRepository>();
            services.AddScoped<IMessageRespository, MessageRepository>();
            services.AddScoped(typeof(IChatAppRepository<>), typeof(ChatAppRepository<>));
            // connect database
            services.AddDbContext<ChatAppDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ChatAppDBConnection")));
            services.AddEndpointsApiExplorer();
            services.AddControllers().AddJsonOptions(x =>
                            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            ;
        }
    }
}
