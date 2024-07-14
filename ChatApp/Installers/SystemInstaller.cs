
using ChatApp.Configurations;
using ChatApp.Interfaces;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ChatApp.Services.ChatServices;
using ChatApp.Data.Modals;
using ChatApp.Data.UnitOfWork;
using ChatApp.Models.ResponeModels;

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
            services.AddScoped<Respone>();
            services.AddSingleton<IPhotoService, PhotoService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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
