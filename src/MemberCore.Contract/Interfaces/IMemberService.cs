using System;
using System.Collections.Generic;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Enums;

namespace MemberCore.Contract.Interfaces
{
    public interface IMemberService
    {
        void SyncMembersAvatars();
        void SyncMembers(object syncMembers, IEnumerable<Guid> activeUserIds);
        IMemberModel GetCurrentUser();
        IMemberModel GetById(Guid id, bool mapCoordinatorProperties = false);
        IMemberModel GetByUserName(string userName);
        IMemberModel GetByEmail(string email);
        IEnumerable<IMemberModel> GetAll();
        int GetAllJanitorsCount(string managementDepartmentSyncId);
        IEnumerable<IMemberModel> GetByIds(IEnumerable<Guid> ids, bool mapCoordinatorProperties = false);
        Dictionary<Guid, string> GetMemberNames(IEnumerable<Guid> ids);
        IEnumerable<IMemberModel> GetEmployees();
        IEnumerable<IMemberModel> GetEmployeesByManagementDepartment(Guid managementDepartmentId);
        IEnumerable<IManagementDepartmentModel> GetUserManagementDepartments(Guid id, bool onlyActiveManagementDepartment = false);
        void UpdateMember(int daysAhead, Guid? managementDepartmentId);
        RoleType SwitchMemberToNextAvailableRole(Guid memberId);
        IEnumerable<IMemberModel> GetAllowedMembersForJob();
        IEnumerable<IMemberModel> GetAllowedMembersForJobByIds(IEnumerable<Guid> memberIds = null);
        IEnumerable<IMemberModel> GetAllowedMembersForJobByDepartmentId(Guid managementDepartmentId);
        void ClearCurrentRole(Guid memberId);
        void SaveManagementDepartment(Guid managementDepartmentId);
        ICurrentUserContextModel GetCurrentUserContext();
        bool IsMemberHasJanitorRoleInManagementDepartment(Guid memberId, Guid managementDepartmentId);
        RoleType GetCurrentRoleByMemberId(Guid memberId);
    }
}