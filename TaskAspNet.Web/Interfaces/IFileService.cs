
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.Interfaces;

public interface IFileService
{
    Task<string?> SaveImageAsync(UploadSelectImgDto imageData, string folderName);
}
