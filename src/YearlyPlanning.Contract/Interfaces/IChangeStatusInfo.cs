using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IChangeStatusInfo
    {
        string ChangeStatusComment { get; set; }

        DateTime ChangeStatusDate { get; set; }

        string Name { get; set; }

        string Avatar { get; set; }

        IEnumerable<IChangeStatusUploadedFile> UploadedFileList { get; set; }
    }
}
