
using Microsoft.AspNetCore.Http;

namespace TaskAspNet.Business.Dtos
{
    public class UploadSelectImgDto
    {
        public string? CurrentImage { get; set; } 
        public string? SelectedImage { get; set; } 
        public IFormFile? UploadedImage { get; set; }
        public List<string> PredefinedImages { get; set; } = new List<string>();
    }
}
