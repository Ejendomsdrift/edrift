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
//    public class MemberRepository: IMemberRepository
//    {        
//        private readonly ISqlDbConfiguration sqlConfiguration;

//        public MemberRepository(ISqlDbConfiguration sqlConfiguration)
//        {
//            this.sqlConfiguration = sqlConfiguration;            
//        }

//        public void SaveOrUpdate(IMember syncedMember)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                using (var transaction = context.Database.BeginTransaction())
//                {
//                    try
//                    {
//                        IMember member =
//                            context.Members.FirstOrDefault(d =>
//                                d.Samaccountname == syncedMember.Samaccountname &&
//                                d.EMail == syncedMember.EMail);


//                        if (member == null)
//                        {
//                            context.Members.Add((Member) syncedMember);
//                        }
//                        else
//                        {
//                            CopyMember(member, syncedMember);
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

//        private void CopyMember(IMember member, IMember syncedMember)
//        {
//            member.Samaccountname = syncedMember.Samaccountname;
//            member.Objectguid = syncedMember.Objectguid;
//            member.Navn = syncedMember.Navn;
//            member.Titel = syncedMember.Titel;
//            member.EMail = syncedMember.EMail;
//            member.TelefonMobil = syncedMember.TelefonMobil;
//            member.TelefonArbejde = syncedMember.TelefonArbejde;
//            member.Interessentnr = syncedMember.Interessentnr;
//            member.Billede = syncedMember.Billede;
//            member.MimeType = syncedMember.MimeType;
//            member.Activated = syncedMember.Activated;
//        }

//        public IEnumerable<IMember> GetAll()
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Members.ToList();
//            }
//        }

//        public IMember GetByAccountName(string samaccountname)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Members.FirstOrDefault(h => h.Samaccountname == samaccountname);
//            }
//        }

//        public IEnumerable<IMember> GetListByCondition(Expression<Func<IMember, bool>> predicate)
//        {
//            using (var context = new EntityContext(sqlConfiguration.ConnectionString))
//            {
//                return context.Members.Where(predicate).ToList();
//            }
//        }
//    }
//}
