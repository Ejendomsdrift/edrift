using System.Data;
using Infrastructure.Extensions;

namespace SyncDataService.Models
{
    public class DepartmentDataBO
    {
        public const string Query = "SELECT * FROM Boligdata";

        public string HousingDepartmentId { get; set; }
        public string Address { get; set; }      

        public static DepartmentDataBO FromDataRow(DataRow row)
        {
            return new DepartmentDataBO
            {
                Address = row.GetSafe<string>("Adresse"),
                HousingDepartmentId = row.GetSafe<string>("BoligafdelingerID")
            };
        }
    }
}
