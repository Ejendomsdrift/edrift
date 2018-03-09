//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Elmah;
//using Infrastructure.SqlStore;
//using SqlStore.Configurations;
//using SqlStore.Models;

//namespace SqlStore.Implementation
//{
//    public class RoleRepository: IRoleRepository
//    {
//        private readonly ISqlDbConfiguration sqlConfiguration;

//        public RoleRepository(ISqlDbConfiguration sqlConfiguration)
//        {
//            this.sqlConfiguration = sqlConfiguration;               
//        }

//        public void SaveOrUpdate(IRole syncedRole)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                using (var transaction = context.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        IRole role =
//                            context.Roles.FirstOrDefault(d =>
//                                d.AfdelingsID == syncedRole.AfdelingsID &&
//                                d.Samaccountname == syncedRole.Samaccountname &&
//                                d.RolleID == syncedRole.RolleID);

//                        if (role == null)
//                        {
//                            context.Roles.Add((Role) syncedRole);
//                        }
//                        else
//                        {
//                            CopyRole(role, syncedRole);
//                        }

//                        context.SaveChanges();
//                        transaction.Commit();
//                    }
//                    catch (Exception ex)
//                    {
//                        transaction.Rollback();
//                        ErrorSignal.FromCurrentContext().Raise(ex);
//                        //send email here - not implemented yet
//                    }

//                }
//            }
//        }

//        private void CopyRole(IRole role, IRole syncedRole)
//        {
//            role.Activated = syncedRole.Activated;
//            role.RolleID = syncedRole.RolleID;
//            role.Samaccountname = syncedRole.Samaccountname;
//            role.AfdelingsID = syncedRole.AfdelingsID;
//        }

//        public IEnumerable<IRole> GetAll()
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Roles.ToList();
//            }
//        }

//        public IEnumerable<IRole> GetListByCondition(Expression<Func<IRole, bool>> predicate)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Roles.Where(predicate).ToList();
//            }
//        }
//    }
//}
