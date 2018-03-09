using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Authentication.Interfaces;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using MemberCore.Models;
using MongoRepository.Contract.Interfaces;
using NLog;
using SecurityCore.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MemberCore.Implementation
{
    public class MemberService : IMemberService
    {
        private readonly IRepository<Member> memberRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly IManagementDepartmentService managementDepartmentService;
        private readonly IPathHelper pathHelper;
        private readonly IFileHelper fileHelper;
        private readonly ISecurityService securityService;
        private readonly IAppSettingHelper appSettingHelper;

        private static Logger logger;

        public MemberService(
            IRepository<Member> memberRepository,
            IAuthenticationService authenticationService,
            IFileHelper fileHelper,
            IPathHelper pathHelper,
            IManagementDepartmentService managementDepartmentService,
            ISecurityService securityService,
            IAppSettingHelper appSettingHelper)
        {
            this.memberRepository = memberRepository;
            this.authenticationService = authenticationService;
            this.fileHelper = fileHelper;
            this.pathHelper = pathHelper;
            this.managementDepartmentService = managementDepartmentService;
            this.securityService = securityService;
            this.appSettingHelper = appSettingHelper;

            logger = LogManager.GetLogger("SyncMembersLog");
        }

        public void SyncMembers(object syncMembers, IEnumerable<Guid> activeUserIds)
        {
            logger.Info("Enter SyncMembers");

            if (syncMembers == null)
            {
                logger.Info("SyncMembers is null");

                return;
            }

            var members = syncMembers.Deserialize<IEnumerable<SyncMember>>().Where(i => i.RoleList.HasValue()).ToList();

            foreach (var syncMember in members)
            {
                var member = syncMember.Map<Member>();

                logger.Info($"Mapped member: {member.UserName}");

                // TODO Remove this after implementation functionality for SuperAdmin
                var superAdminRole = member.RoleList.FirstOrDefault(i => i.RoleId == (int)RoleType.SuperAdmin);
                if (superAdminRole != null && !member.RoleList.Any(i => i.RoleId == (int)RoleType.Administrator))
                {
                    member.RoleList.Insert(0, new Role
                    {
                        IsDeleted = false,
                        ManagementDepartmentId = superAdminRole.ManagementDepartmentId,
                        RoleId = (int)RoleType.Administrator
                    });
                }
                //End

                member.HasAvatar = syncMember.AvatarFileContent.HasValue();

                var oldMember = memberRepository.Query.FirstOrDefault(d => d.UserName == member.UserName);
                if (oldMember != null)
                {
                    logger.Info($"Update old member: {member.UserName}");
                    logger.Info($"New member role count: {member.RoleList.Count}");
                    member.RoleList = GetUpdatedRoleList(member.RoleList, oldMember.RoleList);
                    logger.Info($"Member to save role count: {member.RoleList.Count}");

                    member.Id = oldMember.Id;
                    member.DaysAhead = oldMember.DaysAhead;

                    if (oldMember.CurrentRole.HasValue &&
                        member.RoleList.Any(x => x.RoleId == (int)oldMember.CurrentRole.Value && !x.IsDeleted))
                    {
                        member.CurrentRole = oldMember.CurrentRole;
                    }
                    else
                    {
                        member.CurrentRole = null;
                    }
                }

                memberRepository.Save(member);

                logger.Info($"{member.UserName} saved");

                if (syncMember.AvatarFileContent.HasValue())
                {
                    logger.Info($"Member: {member.UserName} have avatar");

                    SaveAvatar(member.Id, syncMember.AvatarFileContent);

                    logger.Info("Avatar saved");
                }

                logger.Info("--------------------------------------------------");
            }

            var updatedUserNames = members.Select(i => i.UserName);
            DeactivateMembers(updatedUserNames, activeUserIds);

            logger.Info("Exit from SyncMembers\n\n");
        }

        public void SyncMembersAvatars()
        {
            string avatarFolder = pathHelper.GetAvatarPath();
            avatarFolder = fileHelper.GetFullPath(avatarFolder);
            string syncAvatarFolder = pathHelper.GetAvatarPathForSync();
            syncAvatarFolder = fileHelper.GetFullPath(syncAvatarFolder);
            fileHelper.CopyFolderContent(syncAvatarFolder, avatarFolder);
        }

        public IEnumerable<IMemberModel> GetByIds(IEnumerable<Guid> ids, bool mapCoordinatorProperties = false)
        {
            List<Guid> idList = ids.AsList();

            if (!idList.HasValue())
            {
                return Enumerable.Empty<IMemberModel>();
            }

            List<Member> memberList = memberRepository.Query.Where(i => !i.IsDeleted && idList.Contains(i.Id)).ToList();
            IEnumerable<IMemberModel> result = memberList.Select(x => MapMember(x, mapCoordinatorProperties));

            return result;
        }

        public Dictionary<Guid, string> GetMemberNames(IEnumerable<Guid> ids)
        {
            List<Guid> idList = ids.ToList();

            if (!idList.HasValue())
            {
                return new Dictionary<Guid, string>();
            }

            IEnumerable<Member> members = memberRepository.Query.Where(i => idList.Contains(i.Id));
            Dictionary<Guid, string> result = members.ToDictionary(k => k.Id, v => v.Name);

            return result;
        }

        public IEnumerable<IMemberModel> GetAll()
        {
            List<Member> memberList = memberRepository.Query.Where(i => !i.IsDeleted).ToList();
            IEnumerable<IMemberModel> result = memberList.Select(x => MapMember(x, mapCoordinatorProperties: true));

            return result;
        }

        public int GetAllJanitorsCount(string managementDepartmentSyncId)
        {
            Expression<Func<Member, bool>> filter = i => !i.IsDeleted 
                                                       && i.RoleList.Any(
                                                           x => !x.IsDeleted && 
                                                           x.ManagementDepartmentId == managementDepartmentSyncId && 
                                                           x.RoleId == (int) RoleType.Janitor);

            IQueryable<Member> members = memberRepository.Query.Where(filter);

            return members.Count();
        }

        public IMemberModel GetCurrentUser()
        {
            string userName = authenticationService.GetCurrentUserName();
            IMemberModel result = GetByUserName(userName);

            return result;
        }

        public IEnumerable<IManagementDepartmentModel> GetUserManagementDepartments(Guid id, bool onlyActiveManagementDepartment = false)
        {
            var managementDepartments = Enumerable.Empty<IManagementDepartmentModel>();

            Member member = memberRepository.Query.First(i => i.Id == id);
            RoleType currentRole = GetCurrentRole(member);

            if (currentRole == RoleType.Administrator || currentRole == RoleType.SuperAdmin)
            {
                managementDepartments = managementDepartmentService.GetAllManagements();
            }
            else
            {
                var managementDepartmentIdListForCurrentRole = Enumerable.Empty<string>();

                IEnumerable<Role> activeRoleListForCurrentRole = member.RoleList.Where(x => x.RoleId == (int)currentRole && !x.IsDeleted);
                managementDepartmentIdListForCurrentRole = GetManagementDepartmentIdListForCurrentRole(activeRoleListForCurrentRole, onlyActiveManagementDepartment);
                managementDepartments = managementDepartmentService.GetManagementDepartmentsBySyncIds(managementDepartmentIdListForCurrentRole);
            }

            return managementDepartments;
        }

        private IEnumerable<string> GetManagementDepartmentIdListForCurrentRole(IEnumerable<Role> activeRoleListForCurrentRole, bool onlyActiveManagementDepartment)
        {
            IEnumerable<string> managementDepartmentIdListForCurrentRole = onlyActiveManagementDepartment
                ? activeRoleListForCurrentRole.Where(x => x.IsActive).Select(x => x.ManagementDepartmentId)
                : activeRoleListForCurrentRole.Select(x => x.ManagementDepartmentId);

            return managementDepartmentIdListForCurrentRole;
        }

        public IMemberModel GetById(Guid id, bool mapCoordinatorProperties = false)
        {
            Member user = memberRepository.Query.FirstOrDefault(i => i.Id == id);
            IMemberModel result = MapMember(user, mapCoordinatorProperties);

            return result;
        }

        public IMemberModel GetByUserName(string userName)
        {
            Member user = memberRepository.Query.FirstOrDefault(i => i.UserName == userName);
            IMemberModel result = MapMember(user, mapCoordinatorProperties: true);

            return result;
        }

        public IMemberModel GetByEmail(string email)
        {
            Member user = memberRepository.Query.FirstOrDefault(i => i.Email == email.ToLowerInvariant());
            IMemberModel result = MapMember(user, mapCoordinatorProperties: true);

            return result;
        }

        public IEnumerable<IMemberModel> GetEmployees()
        {
            var allowedRoleIdList = new List<int> { (int)RoleType.Coordinator, (int)RoleType.Janitor };
            List<Member> employeeList = memberRepository.Query.Where(i => !i.IsDeleted && i.RoleList.Any(r => !r.IsDeleted && allowedRoleIdList.Contains(r.RoleId))).ToList();
            IEnumerable<IMemberModel> result = employeeList.Select(x => MapMember(x, mapCoordinatorProperties: true));

            return result;
        }

        public IEnumerable<IMemberModel> GetEmployeesByManagementDepartment(Guid managementDepartmentId)
        {
            IEnumerable<IMemberModel> result = GetEmployeesByManagementDepartmentIds(new List<Guid> { managementDepartmentId });

            return result;
        }

        public void UpdateMember(int daysAhead, Guid? departmentId)
        {
            IMemberModel currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return;
            }

            Member member = memberRepository.Query.First(m => m.Id == currentUser.MemberId);

            UnsetOldActiveRole(member, currentUser.CurrentRole);

            if (departmentId.HasValue)
            {
                IManagementDepartmentModel managementDepartmentModel = managementDepartmentService.GetManagementDepartmentById(departmentId.Value);
                Role role = member.RoleList.First(x => x.RoleId == (int)currentUser.CurrentRole && x.ManagementDepartmentId == managementDepartmentModel.SyncDepartmentId);
                role.IsActive = true;
            }
            else
            {
                Role role = member.RoleList.First(x => x.RoleId == (int)member.CurrentRole);
                role.IsActive = true;
            }

            member.DaysAhead = daysAhead;
            memberRepository.Save(member);
        }

        public void ClearCurrentRole(Guid memberId)
        {
            memberRepository.UpdateSingleProperty(memberId, m => m.CurrentRole, null);
        }

        public void SaveManagementDepartment(Guid managementDepartmentId)
        {
            IMemberModel currentUser = GetCurrentUser();
            IManagementDepartmentModel managementDepartment = managementDepartmentService.GetManagementDepartmentById(managementDepartmentId);
            Member member = memberRepository.Query.First(m => m.Id == currentUser.MemberId);
            UnsetOldActiveRole(member, currentUser.CurrentRole);

            Role newActiveRole = member.RoleList.First(x => x.ManagementDepartmentId == managementDepartment.SyncDepartmentId && x.RoleId == (int)currentUser.CurrentRole);
            newActiveRole.IsActive = true;

            memberRepository.UpdateSingleProperty(member.Id, m => m.RoleList, member.RoleList);
        }

        public ICurrentUserContextModel GetCurrentUserContext()
        {
            var managementDepartments = new List<IManagementDepartmentModel>();

            IMemberModel currentUser = GetCurrentUser();

            if (currentUser.CurrentRole == RoleType.Coordinator)
            {
                IEnumerable<Guid> ids = currentUser.ManagementsToActiveRolesRelation[currentUser.CurrentRole];
                managementDepartments = managementDepartmentService.GetManagementDepartmentsByIds(ids).ToList();
            }
            else
            {
                Member member = memberRepository.Query.First(x => x.Id == currentUser.MemberId);

                IEnumerable<string> managementDepartmentSyncIds = member.RoleList
                    .Where(x => !x.IsDeleted && x.RoleId == (int)currentUser.CurrentRole)
                    .Select(x => x.ManagementDepartmentId);

                managementDepartments = managementDepartmentService.GetManagementDepartmentsBySyncIds(managementDepartmentSyncIds).ToList();
            }

            return new CurrentUserContextModel
            {
                MemberModel = currentUser,
                ManagementDepartments = managementDepartments,
                SelectedManagementDepartment = GetSelectedManagementDepartment(currentUser.ActiveManagementDepartmentId, managementDepartments)
            };
        }

        public bool IsMemberHasJanitorRoleInManagementDepartment(Guid memberId, Guid managementDepartmentId)
        {
            Member member = memberRepository.Query.First(x => x.Id == memberId);
            IManagementDepartmentModel managementDepartment = managementDepartmentService.GetManagementDepartmentById(managementDepartmentId);
            IEnumerable<Role> roleList = member.RoleList.Where(x => x.ManagementDepartmentId == managementDepartment.SyncDepartmentId);
            bool isMemberHasJanitorRole = roleList.Any(x => x.RoleId == (int)RoleType.Janitor);

            return isMemberHasJanitorRole;
        }

        public IEnumerable<IMemberModel> GetAllowedMembersForJob()
        {
            string allowedRolesForAssigningOnJob = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.AllowedRolesForAssigningOnJob);
            IEnumerable<int> allowedRoleIds = securityService.GetRoles(allowedRolesForAssigningOnJob).Select(i => (int)i);
            Expression<Func<Member, bool>> filter = f => !f.IsDeleted && f.RoleList.Any(r => !r.IsDeleted && allowedRoleIds.Contains(r.RoleId));
            List<Member> memberList = memberRepository.Query.Where(filter).ToList();
            IEnumerable<IMemberModel> result = memberList.Select(x => MapMember(x, mapCoordinatorProperties: true));

            return result;
        }

        public IEnumerable<IMemberModel> GetAllowedMembersForJobByIds(IEnumerable<Guid> memberIds = null)
        {
            string allowedRolesForAssigningOnJob = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.AllowedRolesForAssigningOnJob);
            IEnumerable<int> allowedRoleIds = securityService.GetRoles(allowedRolesForAssigningOnJob).Select(i => (int)i);

            Expression<Func<Member, bool>> filter = i => !i.IsDeleted && i.RoleList.Any(role => !role.IsDeleted && allowedRoleIds.Contains(role.RoleId));

            if (memberIds.HasValue())
            {
                filter = filter.And(i => memberIds.Contains(i.Id));
            }

            List<Member> memberList = memberRepository.Query.Where(filter).ToList();
            List<IMemberModel> result = memberList.Select(x => MapMember(x, mapCoordinatorProperties: true)).ToList();

            return result;
        }

        public IEnumerable<IMemberModel> GetAllowedMembersForJobByDepartmentId(Guid managementDepartmentId)
        {
            string allowedRolesForAssigningOnJob = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.AllowedRolesForAssigningOnJob);
            IManagementDepartmentModel management = managementDepartmentService.GetManagementDepartmentById(managementDepartmentId);
            IEnumerable<int> allowedRoleIds = securityService.GetRoles(allowedRolesForAssigningOnJob).Select(i => (int)i);

            Expression<Func<Member, bool>> filter = f => !f.IsDeleted &&
                                                          f.RoleList.Any(
                                                              r=> !r.IsDeleted && 
                                                              r.ManagementDepartmentId == management.SyncDepartmentId && 
                                                              allowedRoleIds.Contains(r.RoleId));

            List<Member> memberList = memberRepository.Query.Where(filter).ToList();
            IEnumerable<IMemberModel> result = memberList.Select(x => MapMember(x, mapCoordinatorProperties: true));

            return result;
        }

        private IManagementDepartmentModel GetSelectedManagementDepartment(Guid? activeManagementDepartmentId, List<IManagementDepartmentModel> managementDepartments)
        {
            if (activeManagementDepartmentId.HasValue && managementDepartments.Any(x => x.Id == activeManagementDepartmentId.Value))
            {
                return managementDepartments.First(x => x.Id == activeManagementDepartmentId.Value);
            }

            return managementDepartments.First();
        }

        private void UnsetOldActiveRole(Member member, RoleType currentRole)
        {
            Role oldActiveRole = member.RoleList.FirstOrDefault(x => x.RoleId == (int)currentRole && x.IsActive);

            if (oldActiveRole != null)
            {
                oldActiveRole.IsActive = false;
            }
        }

        private IEnumerable<IMemberModel> GetEmployeesByManagementDepartmentIds(IEnumerable<Guid> ids)
        {
            IEnumerable<string> syncIds = managementDepartmentService.GetManagementSyncIds(ids);
            var roleIdList = new List<int> { (int)RoleType.Administrator, (int)RoleType.SuperAdmin };

            List<Member> employeeList = memberRepository.Query.Where(i => !i.IsDeleted
                && i.RoleList.Any(r => !r.IsDeleted && syncIds.Contains(r.ManagementDepartmentId) && !roleIdList.Contains(r.RoleId))).ToList();

            IEnumerable<IMemberModel> result = employeeList.Select(x => MapMember(x, mapCoordinatorProperties: true));

            return result;
        }

        private IMemberModel MapMember(Member member, bool mapCoordinatorProperties = false)
        {
            if (member == null)
            {
                return null;
            }

            IOrderedEnumerable<Role> activeRoles = member.RoleList.Where(i => !i.IsDeleted).OrderBy(x => x.RoleId);
            RoleType currentRole = GetCurrentRole(member);

            var resultMember = new MemberModel
            {
                MemberId = member.Id,
                Name = member.Name,
                UserName = member.UserName,
                IsDeleted = member.IsDeleted,
                Roles = activeRoles.Select(i => (RoleType)i.RoleId),
                CurrentRole = currentRole,
                Avatar = member.HasAvatar ? pathHelper.GetAvatarPath(member.Id) : string.Empty,
                Email = member.Email,
                WorkingPhone = member.WorkingPhone,
                MobilePhone = member.MobilePhone,
                DaysAhead = member.DaysAhead ?? Constants.Common.DefaultMemberDayAhead
            };

            resultMember.LazyManagementsToActiveRolesRelation = new Lazy<IDictionary<RoleType, IEnumerable<Guid>>>(() => {
                var managementSyncIds = activeRoles.Select(i => i.ManagementDepartmentId);
                var managementDepartmentIdsToSyncIds = managementDepartmentService.ManagementDepartmentIdsByRoles(managementSyncIds);
                var managementsToActiveRolesRelation = GetManagementsToActiveRolesRelation(activeRoles, managementDepartmentIdsToSyncIds);
                return managementsToActiveRolesRelation;
            });

            resultMember.LazyActiveManagementDepartmentId = new Lazy<Guid?>(() => {
                var role = GetActiveRole(activeRoles, member, currentRole);
                var managementDepartmentId = managementDepartmentService.GetManagementDepartmentId(role.ManagementDepartmentId);
                return managementDepartmentId;
            });              

            return resultMember;
        }

        private IDictionary<RoleType, IEnumerable<Guid>> GetManagementsToActiveRolesRelation(IEnumerable<Role> roles, IDictionary<string, Guid> managementDepartmentIdsToSyncId)
        {
            var result = roles.GroupBy(r => (RoleType)r.RoleId, r => r.ManagementDepartmentId)
                .ToDictionary(g => g.Key, g => g.Select(syncId => managementDepartmentIdsToSyncId[syncId]));

            return result;
        }

        private Role GetActiveRole(IOrderedEnumerable<Role> roles, Member member, RoleType currentRole)
        {
            IEnumerable<Role> roleListForCurrentRole = roles.Where(x => x.RoleId == (int)currentRole);
            Role activeRole = roleListForCurrentRole.FirstOrDefault(x => x.IsActive);
            Role result = activeRole ?? roleListForCurrentRole.First();

            member.RoleList.First(x => x.Id == result.Id).IsActive = true;
            memberRepository.UpdateSingleProperty(member.Id, m => m.RoleList, member.RoleList);

            return result;
        }

        private void SaveAvatar(Guid memberId, byte[] avatarFileContent)
        {
            string filePath = pathHelper.GetAvatarPathForSync(memberId);

            fileHelper.SaveFile(filePath, avatarFileContent);
        }

        private void DeactivateMembers(IEnumerable<string> updatedUserNames, IEnumerable<Guid> activeUserIds)
        {
            memberRepository.UpdateManySingleProperty(i => !updatedUserNames.Contains(i.UserName) && !activeUserIds.Contains(i.Id), m => m.IsDeleted, true);
        }

        private List<Role> GetUpdatedRoleList(List<Role> newRoles, List<Role> oldRoles)
        {
            var result = newRoles.Count > default(int) ? new HashSet<Role>(newRoles) : new HashSet<Role>();

            foreach (var oldRole in oldRoles)
            {
                Role newRole = result.FirstOrDefault(i => i.Equals(oldRole));

                if (newRole == null)
                {
                    oldRole.IsDeleted = true;
                    oldRole.IsActive = false;
                    result.Add(oldRole);
                }
                else
                {
                    newRole.IsActive = oldRole.IsActive;
                    newRole.Id = oldRole.Id;
                }
            }

            return result.ToList();
        }

        public RoleType SwitchMemberToNextAvailableRole(Guid memberId)
        {
            string switchPlatformButtonKey = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.SwitchPlatformButton);
            Member member = memberRepository.Query.First(m => m.Id == memberId);
            List<RoleType> allowedRoleList = securityService.GetRoles(switchPlatformButtonKey).OrderByDescending(x => x.GetSortIndex()).ToList();
            RoleType currentRole = GetCurrentRole(member);
            RoleType nextRole = GetNextRole(currentRole, allowedRoleList);
            memberRepository.UpdateSingleProperty(memberId, m => m.CurrentRole, nextRole);

            return nextRole;
        }

        public RoleType GetCurrentRoleByMemberId(Guid memberId)
        {
            Member member = memberRepository.Query.First(x => x.Id == memberId);

            return GetCurrentRole(member);
        }

        private RoleType GetNextRole(RoleType? currentRole, List<RoleType> roles)
        {
            if (currentRole.HasValue && roles.Any(x => x == currentRole))
            {
                roles.Remove(currentRole.Value);
            }

            return roles.First();
        }

        private RoleType GetCurrentRole(Member member)
        {
            var isMobile = HttpContext.Current != null && HttpContext.Current.Request.Browser.IsMobileDevice;
            List<RoleType> memberRoleList = member.RoleList.Where(i => !i.IsDeleted).Select(x => (RoleType)x.RoleId).OrderByDescending(x => x.GetSortIndex()).ToList();

            if (memberRoleList.Count == 1)
            {
                return memberRoleList.First();
            }

            RoleType currentRole = isMobile && memberRoleList.Any(m => m == RoleType.Janitor)
                ? RoleType.Janitor
                : member.CurrentRole ?? memberRoleList.First();

            return currentRole;
        }
    }
}