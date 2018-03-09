//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Elmah;
//using Infrastructure.SqlStore;

//namespace SqlStore.Implementation
//{
//    public class _ManagementService: _IManagementService
//    {
//        private readonly _IManagementRepository managementRepository;

//        public _ManagementService(_IManagementRepository managementRepository)
//        {
//            this.managementRepository = managementRepository;
//        }

//        public void SyncManagements(List<IManagement> managements)
//        {
//            foreach (var management in managements)
//            {
//                management.Activated = true;
//                managementRepository.SaveOrUpdate(management);
//            }
//            DeactivateManagements(managements);
//        }

//        public IEnumerable<IManagement> GetAllManagements()
//        {
//            IEnumerable<IManagement>  managements = managementRepository.GetAll();
//            return managements.OrderBy(m => m.AfdelingsNavn);
//        }

//        private void DeactivateManagements(List<IManagement> managements)
//        {
//            IEnumerable<IManagement> existingManagements = managementRepository.GetAll();
//            IEnumerable<IManagement> deactivatedManagements = existingManagements.Where(ed => managements.All(d => d.AfdelingsID != ed.AfdelingsID));

//            foreach (var management in deactivatedManagements)
//            {
//                management.Activated = false;
//                managementRepository.SaveOrUpdate(management);
//            }
//        }

//    }
//}
