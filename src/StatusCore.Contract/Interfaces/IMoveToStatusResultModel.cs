namespace StatusCore.Contract.Interfaces
{
    public interface IMoveToStatusResultModel
    {
        bool IsSuccessful { get; set; }

        string ResultStatus { get; set; }

        string CurrentStatus { get; set; }

    }
}
