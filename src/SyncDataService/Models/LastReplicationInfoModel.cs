using System;

namespace SyncDataService.Models
{
    public class LastReplicationInfoModel
    {
        public DateTime MembersLastSyncDate { get; set; }

        public DateTime DepartmentsLastSyncDate { get; set; }

        public DateTime ManagementLastSyncDate { get; set; }

        public DateTime DepartmentDataLastSyncDate { get; set; }
    }
}
