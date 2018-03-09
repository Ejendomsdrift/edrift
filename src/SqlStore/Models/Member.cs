//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Infrastructure.SqlStore;

//namespace SqlStore.Models
//{    
//    [Table("Medarbejdere")]
//    public class Member: IMember
//    {
//        //[Key]
//        [Column("samaccountname")]
//        public string Samaccountname { get; set; }
        
//        [Column("objectguid")]
//        public byte[] Objectguid { get; set; }

//        [Column("Navn")]
//        public string Navn { get; set; }

//        [Column("Titel")]
//        public string Titel { get; set; }

//        [Key]
//        [Column("eMail")]
//        public string EMail { get; set; }

//        [Column("Telefon_mobil")]
//        public string TelefonMobil { get; set; }

//        [Column("Telefon_arbejde")]
//        public string TelefonArbejde { get; set; }

//        //public string Jpegphoto { get; set; }

//        [Column("Interessentnr")]
//        public int Interessentnr { get; set; }

//        [Column("Billede")]
//        public byte[] Billede { get; set; }

//        [Column("MimeType")]
//        public string MimeType { get; set; }

//        public bool Activated { get; set; }

//    }
//}
