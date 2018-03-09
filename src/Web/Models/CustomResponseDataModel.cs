using YearlyPlanning.Contract.Models;

namespace Web.Models
{
    public class CustomResponseDataModel
    {
        public UploadFileModel UploadedFile { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}