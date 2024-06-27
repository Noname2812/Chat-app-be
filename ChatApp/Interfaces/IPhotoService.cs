using CloudinaryDotNet.Actions;

namespace ChatApp.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> UploadImage(IFormFile file);
        Task<DeletionResult> DeleteImage(string id);
    }
}
