using Infrastructure.Extensions;
using System.Collections.Generic;
using System.Data;

namespace SyncDataService.Models
{
    public class MemberBO
    {
        public const string Query = "SELECT * FROM Medarbejdere m";

        public string UserName { get; set; }
        public byte[] ObjectGuid { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string WorkingPhone { get; set; }
        public int Interessentnr { get; set; }
        public byte[] AvatarFileContent { get; set; } // has [image] type in database
        public List<RoleBO> RoleList { get; set; }

        public static MemberBO FromDataRow(DataRow row)
        {
            return new MemberBO
            {
                UserName = row.GetSafe<string>("samaccountname"),
                ObjectGuid = row.GetSafe<byte[]>("objectguid"),
                Name = row.GetSafe<string>("Navn"),
                Title = row.GetSafe<string>("Titel"),
                Email = row.GetSafe<string>("eMail"),
                MobilePhone = row.GetSafe<string>("Telefon_mobil"),
                WorkingPhone = row.GetSafe<string>("Telefon_arbejde"),
                Interessentnr = row.GetSafe<int>("Interessentnr"),
                AvatarFileContent = row.GetSafe<byte[]>("Billede")
            };
        }
    }
}
