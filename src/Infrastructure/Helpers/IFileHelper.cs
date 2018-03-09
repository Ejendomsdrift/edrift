namespace Infrastructure.Helpers
{
    public interface IFileHelper
    {
        void SaveFile(string filePath, byte[] content);
        void CopyFolderContent(string sourcePath, string destinationPath);
        string GetFullPath(string path);
    }
}
