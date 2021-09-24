using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Research_Repository_Utility
{
    public class FileHelper
    {
        public static string UploadFiles(IFormFileCollection files, string webRootPath, string fileLocation)
        {
            string directoryPath = webRootPath + fileLocation;
            string filesListString = "";

            foreach (IFormFile file in files)
            {
                //Replace spaces in file name with dashes
                string fileName = file.FileName.Replace(" ", "-");

                string filePath = Path.Combine(directoryPath, fileName);

                //Add file name to list of file names if the file path does not already exist
                if(!File.Exists(filePath))
                {
                    filesListString += fileName + ",";
                }

                //Upload file or replace if the path already exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

            }

            //Return list of file names
            return filesListString;
        }

        public static void DeleteFiles(string filePath, string webRootPath,  string fileLocation)
        {
            string path = webRootPath + fileLocation;
            if (filePath != null)
            {
                string file = Path.Combine(path, filePath);
                // Delete a file by using File class static method...
                if (File.Exists(file))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }
                }
            } else
            {
                // Delete a directory and all subdirectories with Directory static method...
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }

                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

           
        }

        public static void CopyFiles(string fileName, string webRootPath, string sourceFileLocation, string targetFileLocation, bool deleteTargetFiles)
        {
            //Delete existing files in the target folder
            if (deleteTargetFiles)
            {
                DeleteFiles(null, webRootPath, targetFileLocation);
            }

            string sourcePath = webRootPath + sourceFileLocation;
            string targetPath = webRootPath + targetFileLocation;

            // Use Path class to manipulate file and directory paths.
            if(fileName != null)
            {
                string sourceFile = Path.Combine(sourcePath, fileName);
                string destFile = Path.Combine(targetPath, fileName);

                // To copy a file to another location and
                // overwrite the destination file if it already exists.
                File.Copy(sourceFile, destFile, true);
                return;
            }


            // To copy a folder's contents to a new location:
            // Create a new target folder.
            // If the directory already exists, this method does not create a new directory.
            Directory.CreateDirectory(targetPath);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (Directory.Exists(sourcePath))
            {

                string[] files = Directory.GetFiles(sourcePath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string file = Path.GetFileName(s);
                    string destFile = Path.Combine(targetPath, file);
                   File.Copy(s, destFile, true);
                }
            }
        }

        public static void MoveFiles(string fileName, string webRootPath, string sourceFileLocation, string targetFileLocation)
        {
            string sourcePath = webRootPath + sourceFileLocation;
            string targetPath = webRootPath + targetFileLocation;

            if(fileName != null) {
                string sourceFile = Path.Combine(sourcePath, fileName);
                string destFile = Path.Combine(targetPath, fileName);

                // To move a file or folder to a new location:
                File.Move(sourceFile, destFile);
                return;
            }

            // To move an entire directory. To programmatically modify or combine
            // path strings, use the System.IO.Path class.
            Directory.Move(sourcePath, targetPath);
        }
    

        public static IActionResult DownloadFile(string filePath)
        {
            //Get directory name dynamically to keep function generic
            string path = Directory.GetCurrentDirectory() + "\\wwwroot" + filePath;

            byte[] bytes = File.ReadAllBytes(path);
            string contentType = "APPLICATION/octet-stream";
            string fileName = Path.GetFileName(path);

            return new FileContentResult(bytes, contentType) { FileDownloadName = fileName };
        }
    }
}
