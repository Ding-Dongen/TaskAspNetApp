
using TaskAspNet.Business.Dtos;
using TaskAspNet.Business.Interfaces;

namespace TaskAspNet.Business.Services;


public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> SaveImageAsync(UploadSelectImgDto imageData, string folderName)
    {
        if (imageData == null) return null;

        if (imageData.UploadedImage != null)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageData.UploadedImage.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageData.UploadedImage.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }
        else if (!string.IsNullOrEmpty(imageData.SelectedImage))
        {
            return imageData.SelectedImage;
        }


        return null;
    }
}
