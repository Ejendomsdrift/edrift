using Infrastructure.Extensions;
using System.Collections.Generic;
using System.Data;

namespace SyncDataService.Models
{
    public class HousingDepartmentBO
    {
        public const string Query = "SELECT * FROM Boligafdelinger";

        public string SyncDepartmentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ManagementDepartmentId { get; set; }
        public IEnumerable<string> AddressList { get; set; }

        public static HousingDepartmentBO FromDataRow(DataRow row)
        {
            return new HousingDepartmentBO
            {
                SyncDepartmentId = (string)row["afdelingsID"],
                Name = (string)row["AfdelingsNavn"],
                Type = (string)row["AfdelingsType"],
                ManagementDepartmentId = row.GetSafe<string>("DriftAfdelingsId")
            };
        }

    }
}
