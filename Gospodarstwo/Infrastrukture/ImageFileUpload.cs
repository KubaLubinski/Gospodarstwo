using System.Drawing;
using LazZiya.ImageResize;


namespace Gospodarstwo.Infrastrukture
{
    public class ImageFileUpload
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public ImageFileUpload(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }
        private static bool FileTypeCheck(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".png" or ".gif" => true,
                _ => false,
            };
        }
        public FileSendResult SendFile(IFormFile obrazek, string destination, int width)
        {
            FileSendResult SendingFile = new();

            string extension = Path.GetExtension(obrazek.FileName);
            if (!FileTypeCheck(extension))
            {
                SendingFile.Name = Path.GetFileName(obrazek.FileName);
                SendingFile.Success = false;
                SendingFile.Error = "Niepoprawny typ pliku graficznego.";
                return SendingFile;

            }

            SendingFile.Name = Guid.NewGuid().ToString() + extension;

            var upload = Path.Combine(hostingEnvironment.WebRootPath, destination);
            var filePath = Path.Combine(upload, SendingFile.Name);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                obrazek.CopyTo(fileStream);
            }

            string path = Path.Combine(filePath);

            using (var imgFile = Image.FromFile(path))
            {
                var miniFile = imgFile.ScaleByWidth(width);
                upload = Path.Combine(hostingEnvironment.WebRootPath, destination + "\\mini");
                filePath = Path.Combine(upload, SendingFile.Name);
                miniFile.SaveAs(filePath);
            
            }

            SendingFile.Success = true;

            return SendingFile;
        }
    }
}
