//using System.Data.Entity;
//using Infrastructure.SqlStore;

//namespace SqlStore.Models
//{
//    public class EntityContext: DbContext
//    {
//        public EntityContext(string connectionString) : base(connectionString){}

//        //public DbSet<Department> Departments { get; set; }

//        //public DbSet<Management> Managements { get; set; }

//        public DbSet<Member> Members { get; set; }

//        public DbSet<Role> Roles { get; set; }


//        protected override void OnModelCreating(DbModelBuilder builder)
//        {
//            builder.Entity<Role>().HasKey(table => new { table.AfdelingsID, table.RolleID, table.Samaccountname });
//        }
//    }
//}
