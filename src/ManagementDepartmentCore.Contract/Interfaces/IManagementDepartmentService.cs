using System;
using System.Collections.Generic;

namespace ManagementDepartmentCore.Contract.Interfaces
{
    public interface IManagementDepartmentService
    {
        void SyncManagementDepartments(object syncManagements);

        Guid GetParentManagementId(Guid housingDepartmentId);

        string GetParentSyncDepartmentId(Guid housingDepartmentId);

        IEnumerable<IManagementDepartmentModel> GetAllManagements();

        List<IManagementDepartmentModel> GetByHousingDepartmentIds(IEnumerable<Guid> housingDepartmentIds);

        IEnumerable<IHousingDepartmentModel> GetAllHousingDepartments();

        IHousingDepartmentModel GetHousingDepartment(Guid departmentId);

        IEnumerable<IHousingDepartmentModel> GetHousingDepartments(Guid managementId);

        IEnumerable<Guid> GetHousingDepartmentIds(Guid managementId);

        IEnumerable<IHousingDepartmentModel> GetHousingDepartments(IEnumerable<Guid> housingDepartmentIds);

        IEnumerable<IHousingDepartmentModel> GetHousingDepartments(Guid managementDepartmentId, IEnumerable<Guid> housingDepartmentIdList);

        IEnumerable<Guid> GetHousingDepartmentIds(Guid managementDepartmentId, IEnumerable<Guid> housingDepartmentIdList);

        IEnumerable<IHousingDepartmentModel> GetHousingDepartmentsByManagementIds(IEnumerable<Guid> managementDepartmentIds);

        IEnumerable<IManagementDepartmentModel> GetManagementDepartmentsBySyncIds(IEnumerable<string> syncIds);

        IManagementDepartmentModel GetManagementDepartmentById(Guid managementDepartmentId);

        IDictionary<string, Guid> ManagementDepartmentIdsByRoles(IEnumerable<string> syncIds);

        Guid GetManagementDepartmentId(string syncId);
        
        IEnumerable<IManagementDepartmentModel> GetManagementDepartmentsByIds(IEnumerable<Guid> ids);

        IDictionary<Guid, IEnumerable<Guid>> GetManagementsToHousingDepartmentsRelation(IEnumerable<Guid> managementDepartmentsIds);

        IEnumerable<string> GetManagementSyncIds(IEnumerable<Guid> managementIds);

        string GetManagementDepartmentSyncId(Guid managementDepartmentId);
    }
}
