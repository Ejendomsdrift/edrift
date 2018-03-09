using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Models
{

    class TextFileResultModel : ITextFileResultModel
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}
