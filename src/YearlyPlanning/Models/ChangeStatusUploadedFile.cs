using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class ChangeStatusUploadedFile : IChangeStatusUploadedFile
    {
        public string Link { get; set; }
        public string FileName { get; set; }
    }
}