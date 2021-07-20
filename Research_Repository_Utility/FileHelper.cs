using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_Utility
{
    public class FileHelper
    {
        public static string UploadFiles(IFormFileCollection files, string webRootPath, string fileLocation)
        {
            string upload = webRootPath + fileLocation;
            string filesListString = "";

            foreach (IFormFile file in files)
            {
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(file.FileName);
                string directoryPath = Path.Combine(upload, fileName + extension);
                Directory.CreateDirectory(Path.GetDirectoryName(directoryPath));
                using (var fileStream = new FileStream(directoryPath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                filesListString += fileName + extension + ",";

            }

            return filesListString;
        }

        public static void DeleteFile(string webRootPath, string filePath, string fileLocation)
        {
            string upload = webRootPath + fileLocation;
            string oldFile = Path.Combine(upload, filePath);

            if (File.Exists(oldFile))
            {
                File.Delete(oldFile);
            }
        }

        public static IActionResult DownloadFile(string filePath)
        {
            string path = Directory.GetCurrentDirectory() + "\\wwwroot" + filePath;

            byte[] bytes = File.ReadAllBytes(path);
            string contentType = "APPLICATION/octet-stream";
            string fileName = Path.GetFileName(path);

            return new FileContentResult(bytes, contentType) { FileDownloadName = fileName };
        }
    }
}
