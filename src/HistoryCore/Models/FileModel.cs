using HistoryCore.Contract.Interfaces;

namespace HistoryCore.Models
{
    public class FileModel: IFileModel
    {
        public string FileName { get; set; }

        public string FileUrl { get; set; }

    }
}
