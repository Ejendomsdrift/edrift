using StatusCore.Contract.Interfaces;

namespace StatusCore.Models
{
    public class MoveToStatusResultModel: IMoveToStatusResultModel
    {
        public bool IsSuccessful { get; set; }
        public string ResultStatus { get; set; }
        public string CurrentStatus { get; set; }
    }
}
