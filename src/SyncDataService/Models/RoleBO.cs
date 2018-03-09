using Infrastructure.Extensions;
using System.Data;

namespace SyncDataService.Models
{
    public class RoleBO
    {
        public const string Query = "SELECT DISTINCT * FROM rel_Medarbejder_Rolle_Afdeling";

        public string AccountName { get; set; }
        public short RoleId { get; set; }
        public string ManagementDepartmentId { get; set; }

        public static RoleBO FromDataRow(DataRow row)
        {
            return new RoleBO
            {
                AccountName = row.GetSafe<string>("samaccountname"),
                RoleId = (short)row["RolleID"],
                ManagementDepartmentId = (string)row["AfdelingsID"]
            };
        }
    }
}
