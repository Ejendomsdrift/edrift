using Infrastructure.Extensions;
using MemberCore.Contract.Interfaces;
using MongoRepository.Contract.Interfaces;
using SecurityCore.Contract.Interfaces;
using SecurityCore.Models;
using System.Collections.Generic;
using System.Linq;
using MemberCore.Contract.Enums;
using System;
using StatusCore.Contract.Enums;

namespace SecurityCore.Implementation
{
    public class SecurityService : ISecurityService
    {
        private IRepository<SecurityPermission> repository;

        public SecurityService(IRepository<SecurityPermission> repository)
        {
            this.repository = repository;
        }

        public Dictionary<string, bool> HasAccessByKeyList(ISecurityQuery query)
        {
            var permissionList = Get(query.KeyList);
            return permissionList.ToDictionary(permission => permission.Key, permission => HasAccess(query, permission));
        }

        public Dictionary<string, bool> HasAccessByGroupName(ISecurityQuery query)
        {
            var permissionList = Get(query.GroupName).ToList();
            return permissionList.ToDictionary(permission => permission.Key, permission => HasAccess(query, permission));
        }

        public void Save(ISecurityPermissionModel permission)
        {
            var savedPermission = GetByKey(permission.Key);
            
            if (savedPermission == null)
            {
                var model = permission.Map<SecurityPermission>();
                repository.Save(model);
            }
        }

        public IEnumerable<RoleType> GetRoles(string key)
        {
            var permission = GetByKey(key);
            var rule = permission.Rules.FirstOrDefault();
            return rule != null && rule.ViewRoleList.Any() ? rule.ViewRoleList : Enumerable.Empty<RoleType>();
        }

        private bool HasAccess(ISecurityQuery query, SecurityPermission permission)
        {
            if (permission == null)
            {
                return false;
            }

            var result = IsAllowedPermission(permission, query);
            return result;
        }

        private SecurityPermission GetByKey(string key)
        {
            return repository.Query.FirstOrDefault(i => i.Key == key);
        }

        private IEnumerable<SecurityPermission> Get(string groupName)
        {
            return repository.Query.Where(i => i.GroupName == groupName);
        }

        private IEnumerable<SecurityPermission> Get(IEnumerable<string> keyList)
        {
            return repository.Query.Where(x => keyList.Contains(x.Key));
        }

        private bool IsAllowedPermission(SecurityPermission permission, ISecurityQuery query)
        {
            return IsValidMember(query.Member) &&
                   IsValidWeek(query.DayAssignDate) && 
                   IsValidDayAssignStatus(query.DayAssignStatus) &&
                   IsValidUserRoleList(permission, query) &&
                   IsValidUserPlatform(permission, query) && 
                   IsValidRole(permission, query) && 
                   IsValidTab(permission, query) && 
                   IsValidForGroupedTask(permission, query);
        }

        private bool IsValidMember(IMemberModel member)
        {
            return member != null && !member.IsDeleted && member.Roles.HasValue();
        }

        private bool IsValidWeek(DateTime? dayAssignDate)
        {
            if (!dayAssignDate.HasValue)
            {
                return true;
            }

            var taskWeek = dayAssignDate.Value.GetWeekNumber();
            var currentWeek = DateTime.UtcNow.GetWeekNumber();
            var isFutureYear = dayAssignDate.Value.Year > DateTime.UtcNow.Year;
            var isValidWeek = dayAssignDate.Value.Year == DateTime.UtcNow.Year && currentWeek <= taskWeek;

            return isFutureYear || isValidWeek;
        }

        private bool IsValidDayAssignStatus(JobStatus? status)
        {
            if (!status.HasValue)
            {
                return true;
            }

            var isValidStatus = !status.In(JobStatus.Canceled, JobStatus.Completed, JobStatus.Expired);
            return isValidStatus;
        }

        private bool IsValidUserRoleList(SecurityPermission permission, ISecurityQuery query)
        {
            if (!permission.Rules.Any(x => x.IsUserShouldHaveAllRoles))
            {
                return true;
            }

            Rule rule = permission.Rules.First(x => x.IsUserShouldHaveAllRoles);
            bool result = rule.UserRoleList.All(s => query.Member.Roles.Contains(s));

            return result;
        }

        private bool IsValidUserPlatform(SecurityPermission permission, ISecurityQuery query)
        {
            if (permission.Rules.All(x => x.AllowedPlatformList.Count == 0) || !query.CurrentPlatformType.HasValue)
            {
                return true;
            }

            bool result = permission.Rules.Any(x => x.AllowedPlatformList.Contains(query.CurrentPlatformType.Value));
            return result;
        }

        private bool IsValidRole(SecurityPermission permission, ISecurityQuery query)
        {
            if (query.CreatorRole.HasValue && query.Page.HasValue)
            {
                var rule = permission.Rules.First(x => x.Page == query.Page);
                return rule.IsEditable && (query.Member.CurrentRole == query.CreatorRole.Value || rule.EditRoleList.Contains(query.Member.CurrentRole));
            } 

            return permission.Rules.Any(x => x.ViewRoleList.Contains(query.Member.CurrentRole));
        }

        private bool IsValidTab(SecurityPermission permission, ISecurityQuery query)
        {
            if (!query.Page.HasValue)
            {
                return true;
            }

            var rule = permission.Rules.First(x => x.Page == query.Page);
            return rule.ViewRoleList.Contains(query.Member.CurrentRole);
        }

        private bool IsValidForGroupedTask(SecurityPermission permission, ISecurityQuery query)
        {
            var rule = permission.Rules.FirstOrDefault(x => x.Page == query.Page);

            if (rule == null || !rule.IsDisabledForGroupingTask)
            {
                return true;
            }

            return !query.IsGroupedTask;
        }
    }
}