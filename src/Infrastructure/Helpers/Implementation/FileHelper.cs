using Infrastructure.Extensions;
using System.IO;
using System.Web;

namespace Infrastructure.Helpers.Implementation
{
    public class FileHelper : IFileHelper
    {
        public void SaveFile(string filePath, byte[] content)
        {
            if (!content.HasValue())
            {
                return;
            }

            var fullPath = GetFullPath(filePath);
            EnsureFolder(fullPath);

            File.WriteAllBytes(fullPath, content);
        }

        public void CopyFolderContent(string sourcePath, string destinationPath)
        {
            EnsureFolder(destinationPath);

            foreach (var filePath in Directory.GetFiles(sourcePath))
            {
                string newFilePath = filePath.Replace(sourcePath, destinationPath);

                if (File.Exists(newFilePath))
                {
                    File.Replace(filePath, newFilePath, destinationBackupFileName: null);
                }
                else
                {
                    File.Copy(filePath, newFilePath);
                }
            }
        }

        public static void EnsureFolder(string path)
        {
            var folderPath = new DirectoryInfo(Path.GetDirectoryName(path));
            if (!folderPath.Exists)
            {
                folderPath.Create();
            }
        }

        public string GetFullPath(string path)
        {
           return path.Contains(":\\") ? path : HttpContext.Current.Server.MapPath(path);
        }       
    }
}
