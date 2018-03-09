using System;
using System.Data;

namespace SyncDataService.Models
{
    public class ReplikationStatsBO
    {
        public const string Query = "SELECT TOP 1 * FROM ReplikationStats WHERE [Tabel] ='{0}' ORDER BY [DatoTid] DESC";

        public DateTime DatoTid { get; set; }
        public string Tabel { get; set; }
        public int AntalRecords { get; set; }

        public static ReplikationStatsBO FromDataRow(DataRow row)
        {
            return new ReplikationStatsBO
            {
                DatoTid = (DateTime)row["DatoTid"],
                Tabel = (string)row["Tabel"],
                AntalRecords = (int)row["AntalRecords"]
            };
        }
    }
}
