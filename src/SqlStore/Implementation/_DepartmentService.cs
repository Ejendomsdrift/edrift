//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Elmah;
//using Infrastructure.SqlStore;

//namespace SqlStore.Implementation
//{
//    public class _DepartmentService : _IDepartmentService
//    {
//        private readonly _IDepartmentRepository departmentRepository;

//        public _DepartmentService(_IDepartmentRepository departmentRepository)
//        {
//            this.departmentRepository = departmentRepository;
//        }

//        public void SyncDepartments(List<_IDepartment> departments)
//        {
//            foreach (var department in departments)
//            {
//                department.Activated = true;
//                departmentRepository.SaveOrUpdate(department);
//            }
//            DeactivateDepartments(departments);
//        }

//        private void DeactivateDepartments(List<_IDepartment> departments)
//        {
//            IEnumerable<_IDepartment> existingDepartments = departmentRepository.GetAll();
//            IEnumerable<_IDepartment> deactivatedDepartments = existingDepartments.Where(ed => departments.All(d => d.AfdelingsID != ed.AfdelingsID));

//            foreach (var department in deactivatedDepartments)
//            {
//                department.Activated = false;
//                departmentRepository.SaveOrUpdate(department);
//            }
//        }

//        public IEnumerable<_IDepartment> GetAllDepartments()
//        {
//            IEnumerable<_IDepartment> departments = departmentRepository.GetAll();
//            return departments.OrderBy(d=>d.AfdelingsNavn);
//        }

//        public IEnumerable<_IDepartment> GetManagementDepartments(string managementId)
//        {
//            IEnumerable<_IDepartment> departments = departmentRepository.GetListByCondition(m => m.DriftAfdelingsId == managementId);
//            return departments.OrderBy(d => d.AfdelingsNavn);
//        }

//    }
//}
