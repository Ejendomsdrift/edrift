using System;
using System.Collections.Generic;
using System.Linq;
using SqlStore.Configurations;
using SqlStore.Models;
//using Infrastructure.SqlStore;
using System.Linq.Expressions;
using Elmah;

namespace SqlStore.Implementation
{
    //public class DepartmentRepository : _IDepartmentRepository
    //{        
    //    private readonly ISqlDbConfiguration sqlConfiguration;

    //    public DepartmentRepository(ISqlDbConfiguration sqlConfiguration)
    //    {
    //        this.sqlConfiguration = sqlConfiguration;
    //    }

    //    public void SaveOrUpdate(_IDepartment syncedDepartment)
    //    {
    //        using (var context = new EntityContext(sqlConfiguration.ConnectionString))
    //        {
    //            using (var transaction = context.Database.BeginTransaction())
    //            {
    //                try
    //                {
    //                    _IDepartment department =
    //                        context.Departments.FirstOrDefault(d => d.AfdelingsID == syncedDepartment.AfdelingsID);
    //                    if (department == null)
    //                    {
    //                        context.Departments.Add((Department) syncedDepartment);
    //                    }
    //                    else
    //                    {
    //                        CopyDepartment(department, syncedDepartment);
    //                    }

    //                    context.SaveChanges();
    //                    transaction.Commit();

    //                }
    //                catch (Exception ex)
    //                {
    //                    transaction.Rollback();
    //                    ErrorSignal.FromCurrentContext().Raise(ex);
    //                    //send email here - not implemented yet
    //                }
    //            }
    //        }
    //    }

    //    private void CopyDepartment(_IDepartment department, _IDepartment syncedDepartment)
    //    {
    //        department.Activated = syncedDepartment.Activated;
    //        department.AfdelingsNavn = syncedDepartment.AfdelingsNavn;
    //        department.AfdelingsType = syncedDepartment.AfdelingsType;
    //        department.DriftAfdelingsId = syncedDepartment.DriftAfdelingsId;
    //    }

        public IDepartment GetById(string departmentId)
    //    {
    //        using (var context = new EntityContext(sqlConfiguration.ConnectionString))
    //        {
                return context.Departments.FirstOrDefault(h => h.AfdelingsID == departmentId);
    //        }
    //    }

    //    public IEnumerable<_IDepartment> GetListByCondition(Expression<Func<_IDepartment, bool>> predicate)
    //    {
    //        using (var context = new EntityContext(sqlConfiguration.ConnectionString))
    //        {
    //            return context.Departments.Where(predicate).ToList();
    //        }
    //    }

    //    public IEnumerable<_IDepartment> GetAll()
    //    {
    //        using (var context = new EntityContext(sqlConfiguration.ConnectionString))
    //        {
    //            return context.Departments.ToList();
    //        }
    //    }

    //}
}
