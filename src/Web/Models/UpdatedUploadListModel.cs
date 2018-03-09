using System;
using System.Collections.Generic;
using Web.Models.Task;

namespace Web.Models
{
    public class UpdatedUploadListModel
    {
        public IEnumerable<UploadFileViewModel> ChangedDescriptionFileList { get; set; }
        public IEnumerable<Guid> MarkedForDeletionFileIdList { get; set; }
    }
}