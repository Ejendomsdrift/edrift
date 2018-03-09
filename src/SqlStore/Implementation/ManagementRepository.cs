//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Elmah;
//using SqlStore.Configurations;
//using SqlStore.Models;
//using Infrastructure.SqlStore;

//namespace SqlStore.Implementation
//{
//    public class ManagementRepository: _IManagementRepository
//    {
//        private readonly ISqlDbConfiguration sqlConfiguration;

//        public ManagementRepository(ISqlDbConfiguration sqlConfiguration)
//        {
//            this.sqlConfiguration = sqlConfiguration;           
//        }

//        public void SaveOrUpdate(IManagement syncedManagement)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                using (var transaction = context.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        IManagement management =
//                            context.Managements.FirstOrDefault(d => d.AfdelingsID == syncedManagement.AfdelingsID);
//                        if (management == null)
//                        {
//                            context.Managements.Add((Management) syncedManagement);
//                        }
//                        else
//                        {
//                            CopyManagement(management, syncedManagement);
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

//        private void CopyManagement(IManagement management, IManagement syncedManagement)
//        {

//            management.AfdelingsType = syncedManagement.AfdelingsType;

//            management.AfdelingsNavn = syncedManagement.AfdelingsNavn;

//            management.ReferererTil = syncedManagement.ReferererTil;

//            management.Activated = syncedManagement.Activated;
//        }

//        public IEnumerable<IManagement> GetAll()
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Managements.ToList();
//            }
//        }

//        public IManagement GetById(string afdelingsID)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Managements.FirstOrDefault(h => h.AfdelingsID == afdelingsID);
//            }
//        }

//        public IEnumerable<IManagement> GetListByCondition(Expression<Func<IManagement, bool>> predicate)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Managements.Where(predicate).ToList();
//            }
//        }
//    }
//}
