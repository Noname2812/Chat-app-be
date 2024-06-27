
using CloudinaryDotNet;

namespace ChatApp.Installers
{
    public class CloudinaryInstaller : IInstaller
    {
        public void InstallerService(IServiceCollection services, IConfiguration configuration)
        {
            var account = new Account();
            configuration.GetSection("Cloudinary").Bind(account);
            var cloudinary = new Cloudinary(account);
            services.AddSingleton(cloudinary);
        }
    }
}
