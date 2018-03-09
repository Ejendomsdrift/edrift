using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace Web.Models
{
    public class JanitorUploadsModel
    {
        public IEnumerable<UploadFileModel> Images { get; set; }
        public IEnumerable<UploadFileModel> Videos { get; set; }
    }
}