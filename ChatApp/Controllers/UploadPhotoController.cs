using ChatApp.Interfaces;
using ChatApp.Models.ResponeModels;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/photo")]
    [ApiController]
    public class UploadPhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        public UploadPhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }
        [HttpPost]
        [Route("up-load-photo", Name = "UploadPhoto")]
        public async Task<ActionResult<UploadImageResultDTO>> UploadPhoto(IFormFile file)
        {
            var result = await _photoService.UploadImage(file);
            return Ok(new UploadImageResultDTO { Id = result.PublicId, Url = result.Url});
        }
        [HttpDelete]
        [Route("{publicId:alpha}", Name = "DeletePhoto")]

        public async Task<ActionResult<DeletionResult>> DeletePhoto(string publicId)
        {
            var result = await _photoService.DeleteImage(publicId);
            return Ok(result);
        }
    }
}
