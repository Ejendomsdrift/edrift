//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Elmah;
//using Infrastructure.SqlStore;
//using SqlStore.Models;

//namespace SqlStore.Implementation
//{
//    public class RoleService: IRoleService
//    {
//        private readonly IRoleRepository roleRepository;

//        public RoleService(IRoleRepository roleRepository)
//        {
//            this.roleRepository = roleRepository;
//        }

//        public void SyncRoles(List<IRole> roles)
//        {
//            foreach (var role in roles)
//            {
//                role.Activated = true;
//                roleRepository.SaveOrUpdate(role);
//            }
//            DeactivateRoles(roles);
//        }

//        private void DeactivateRoles(List<IRole> roles)
//        {
//            IEnumerable<IRole> existingRoles = roleRepository.GetAll();
//            IEnumerable<IRole> deactivatedRoles = existingRoles.Where(ed => roles.All(d => IsNotTheSameRole(d, ed)));

//            foreach (var role in deactivatedRoles)
//            {
//                role.Activated = false;
//                roleRepository.SaveOrUpdate(role);
//            }
//        }

//        private bool IsNotTheSameRole(IRole leftRole, IRole rightRole)
//        {
//            return leftRole.AfdelingsID != rightRole.AfdelingsID ||
//                   leftRole.Samaccountname != rightRole.Samaccountname ||
//                   leftRole.RolleID != rightRole.RolleID;
//        }

//    }
//}
