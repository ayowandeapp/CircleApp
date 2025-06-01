using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Enums;

namespace CircleApp.Services
{
    public class FilesService : IFilesService
    {
        public async Task<string> UploadImageAsync(IFormFile file, ImageFileTypeEnum imageFileTypeEnum)
        {
            string filePathUpload = imageFileTypeEnum switch
            {
                ImageFileTypeEnum.PostImage => "images/posts",
                ImageFileTypeEnum.StoryImage => "images/stories",
                ImageFileTypeEnum.ProfileImage => "images/profile",
                _ => throw new ArgumentException("not allowed")
            };
            //check and save the image
            if (file != null && file.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (file.ContentType.Contains("image"))
                {
                    string pathImages = Path.Combine(rootFolderPath, $"{filePathUpload}");
                    Directory.CreateDirectory(pathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(pathImages, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await file.CopyToAsync(stream);
                    //set the url
                    return $"/{filePathUpload}/{fileName}";
                }
            }
            return "";
        }
    }
}