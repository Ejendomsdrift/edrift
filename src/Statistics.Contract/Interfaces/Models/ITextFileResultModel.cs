namespace Statistics.Contract.Interfaces.Models
{
    public interface ITextFileResultModel
    {
        string FileName { get; set; }
        string Content { get; set; }
    }
}