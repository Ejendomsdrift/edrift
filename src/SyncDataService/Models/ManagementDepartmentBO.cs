using System.Collections.Generic;
using System.Data;

namespace SyncDataService.Models
{
    public class ManagementDepartmentBO
    {
        public const string Query = "SELECT * FROM Driftsafdelinger";

        public string SyncDepartmentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ManagementDepartmentRefId { get; set; }  //reference to other management department
        public List<HousingDepartmentBO> HousingDepartmentList { get; set; }

        public static ManagementDepartmentBO FromDataRow(DataRow row)
        {
            return new ManagementDepartmentBO
            {
                SyncDepartmentId = (string)row["afdelingsID"],
                Type = (string)row["AfdelingsType"],
                Name = (string)row["AfdelingsNavn"],
                ManagementDepartmentRefId = (string)row["ReferererTil"]
            };
        }
    }
}
