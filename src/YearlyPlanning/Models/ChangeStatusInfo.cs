using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.Models
{
    public class ChangeStatusInfo: IChangeStatusInfo
    {
        public string ChangeStatusComment { get; set; }

        public string CancellationReason { get; set; }

        public DateTime ChangeStatusDate { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public IEnumerable<IChangeStatusUploadedFile> UploadedFileList { get; set; }
    }
}
