using System;
using System.Collections.Generic;
using System.Linq;
using MongoRepository.Contract.Interfaces;
using ManagementDepartmentCore.Models;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;

namespace ManagementDepartmentCore.Implementation
{
    public class ManagementDepartmentService : IManagementDepartmentService
    {
        private readonly IRepository<ManagementDepartment> managementRepository;
        public ManagementDepartmentService(IRepository<ManagementDepartment> managementRepository)
        {
            this.managementRepository = managementRepository;
        }

        public void SyncManagementDepartments(object syncManagements)
        {
            if (syncManagements == null)
            {
                return;
            }

            var managements = syncManagements.Deserialize<IEnumerable<ManagementDepartment>>();
            var savedManagements = managementRepository.Query.ToList();

            foreach (var management in managements)
            {
                var oldManagement = savedManagements.FirstOrDefault(d => d.SyncDepartmentId == management.SyncDepartmentId);
                if (oldManagement != null)
                {
                    management.Id = oldManagement.Id;
                }

                management.HousingDepartmentList = GetUpdatedDepartments(management, savedManagements);
                managementRepository.Save(management);
            }

            var updatedManagements = managements.Select(i => i.SyncDepartmentId);
            DeactivateManagements(updatedManagements, savedManagements);
        }

        public IEnumerable<IManagementDepartmentModel> GetAllManagements()
        {
            var managements = managementRepository.Query.Where(i => !i.IsDeleted).OrderBy(i => i.Name).ToList();
            managements.ForEach(i => i.HousingDepartmentList = i.HousingDepartmentList.Where(d => !d.IsDeleted).ToList());
            var mappedData = managements.Map<IEnumerable<IManagementDepartmentModel>>();
            return mappedData;
        }

        public Guid GetParentManagementId(Guid housingDepartmentId)
        {
            var managementDepartment = managementRepository.Query.First(x => !x.IsDeleted && x.HousingDepartmentList.Any(f => f.Id == housingDepartmentId));
            return managementDepartment.Id;
        }

        public List<IManagementDepartmentModel> GetByHousingDepartmentIds(IEnumerable<Guid> housingDepartmentIds)
        {
            var managementDepartments = managementRepository.Query
                .Where(x => !x.IsDeleted && x.HousingDepartmentList.Any(f => housingDepartmentIds.Contains(f.Id)))
                .ToList();

            var result = managementDepartments.Map<List<IManagementDepartmentModel>>();

            return result;
        }

        public string GetParentSyncDepartmentId(Guid housingDepartmentId)
        {
            var managementDepartment = managementRepository.Query.First(x => !x.IsDeleted && x.HousingDepartmentList.Any(f => f.Id == housingDepartmentId));
            return managementDepartment.SyncDepartmentId;
        }

        public string GetManagementDepartmentSyncId(Guid managementDepartmentId)
        {
            var syncId = managementRepository.Query.FirstOrDefault(x => x.Id == managementDepartmentId);

            return syncId == null? string.Empty : syncId.SyncDepartmentId;
        }

        public IEnumerable<string> GetManagementSyncIds(IEnumerable<Guid> managementIds)
        {
            var syncIds = managementRepository.Query
                                              .Where(x => !x.IsDeleted && managementIds.Contains(x.Id))
                                              .Select(i => i.SyncDepartmentId)
                                              .ToList();
            return syncIds;
        }

        public IEnumerable<IHousingDepartmentModel> GetAllHousingDepartments()
        {
            var data = managementRepository.Query.Where(i => !i.IsDeleted)
                                                 .SelectMany(i => i.HousingDepartmentList)
                                                 .Where(d => !d.IsDeleted);

            foreach (var housingDepartment in data)
            {
                var model = housingDepartment.Map<IHousingDepartmentModel>();
                model.ManagementDepartmentId = housingDepartment.Id;
                yield return model;
            }
        }

        public IEnumerable<IHousingDepartmentModel> GetHousingDepartments(Guid managementDepartmentId)
        {
            var managementDepartment = managementRepository.Query.First(md => md.Id == managementDepartmentId);
            var mappedData = managementDepartment.HousingDepartmentList
                                                 .Where(i => !i.IsDeleted)
                                                 .Select(i => MapHousingDepartment(i, managementDepartmentId));
            return mappedData;
        }

        public IEnumerable<Guid> GetHousingDepartmentIds(Guid managementId)
        {
            IEnumerable<IHousingDepartmentModel> departments = GetHousingDepartments(managementId);
            IEnumerable<Guid> departmentIds = departments.Select(x => x.Id);

            return departmentIds;
        }

        public IHousingDepartmentModel GetHousingDepartment(Guid departmentId)
        {
            var managementDepartment = managementRepository.Query.First(md => !md.IsDeleted && md.HousingDepartmentList.Any(x => x.Id == departmentId));
            var housingDepartment = managementDepartment.HousingDepartmentList.First(x => x.Id == departmentId);
            var mappedData = MapHousingDepartment(housingDepartment, managementDepartment.Id);
            return mappedData;
        }

        public IEnumerable<IHousingDepartmentModel> GetHousingDepartments(IEnumerable<Guid> housingDepartmentIds)
        {
            var managements = managementRepository.Query
                                                  .Where(i => i.HousingDepartmentList.Any(d => housingDepartmentIds.Contains(d.Id)))
                                                  .Select(i => new { ManagmentId = i.Id, Departments = i.HousingDepartmentList.Where(d => !d.IsDeleted) })
                                                  .ToList();

            var mappedData = managements.SelectMany(i => i.Departments.Select(d => MapHousingDepartment(d, i.ManagmentId)))
                                        .Where(i => housingDepartmentIds.Contains(i.Id));
            return mappedData;
        }

        public IEnumerable<IHousingDepartmentModel> GetHousingDepartments(Guid managementDepartmentId, IEnumerable<Guid> housingDepartmentIdList)
        {
            var managements = managementRepository.Query
                                                  .Where(i => i.HousingDepartmentList.Any(d => housingDepartmentIdList.Contains(d.Id)) && i.Id == managementDepartmentId)
                                                  .Select(i => new { ManagmentId = i.Id, Departments = i.HousingDepartmentList.Where(d => !d.IsDeleted) })
                                                  .ToList();

            var mappedData = managements.SelectMany(i => i.Departments.Select(d => MapHousingDepartment(d, i.ManagmentId)))
                                        .Where(i => housingDepartmentIdList.Contains(i.Id));
            return mappedData;
        }

        public IEnumerable<Guid> GetHousingDepartmentIds(Guid managementDepartmentId, IEnumerable<Guid> housingDepartmentIdList)
        {
            var managements = managementRepository.Query
                                                  .Where(i => i.HousingDepartmentList.Any(d => housingDepartmentIdList.Contains(d.Id)) && i.Id == managementDepartmentId)
                                                  .Select(i => new { ManagmentId = i.Id, Departments = i.HousingDepartmentList.Where(d => !d.IsDeleted) })
                                                  .ToList();

            return managements.SelectMany(x => x.Departments.Select(y => y.Id));
        }

        public IEnumerable<IHousingDepartmentModel> GetHousingDepartmentsByManagementIds(IEnumerable<Guid> managementDepartmentIds)
        {
            var managements = managementRepository.Query.Where(i => managementDepartmentIds.Contains(i.Id)).ToList();
            var mappedData = managements.SelectMany(i => i.HousingDepartmentList.Select(d => MapHousingDepartment(d, i.Id)));
            return mappedData;
        }

        public IManagementDepartmentModel GetManagementDepartmentById(Guid managementDepartmentId)
        {
            var management = managementRepository.Query.First(m => m.Id == managementDepartmentId);
            management.HousingDepartmentList = management.HousingDepartmentList.Where(i => !i.IsDeleted).ToList();
            var mappedData = management.Map<IManagementDepartmentModel>();
            return mappedData;
        }

        public Guid GetManagementDepartmentId(string syncId)
        {
            if (!syncId.HasValue())
            {
                return Guid.Empty;
            }

            Guid id = managementRepository.Query.First(i => !i.IsDeleted && i.SyncDepartmentId == syncId).Id;
            return id;
        }

        public IDictionary<string, Guid> ManagementDepartmentIdsByRoles(IEnumerable<string> syncIds)
        {
            if (!syncIds.HasValue())
            {
                return new Dictionary<string, Guid>();
            }

            var data = managementRepository.Query
                .Where(i => syncIds.Contains(i.SyncDepartmentId))
                .ToDictionary(i => i.SyncDepartmentId, v=> v.Id);

            return data;
        }

        public IEnumerable<IManagementDepartmentModel> GetManagementDepartmentsByIds(IEnumerable<Guid> ids)
        {
            if (!ids.HasValue())
            {
                return Enumerable.Empty<IManagementDepartmentModel>();
            }

            var managements = managementRepository.Query.Where(i => ids.Contains(i.Id)).ToList();
            managements.ForEach(i => i.HousingDepartmentList = i.HousingDepartmentList.Where(d => !d.IsDeleted).ToList());
            var mappedData = managements.Map<IEnumerable<IManagementDepartmentModel>>();
            return mappedData;
        }

        public IEnumerable<IManagementDepartmentModel> GetManagementDepartmentsBySyncIds(IEnumerable<string> syncIds)
        {
            var managements = managementRepository.Query.Where(md => syncIds.Contains(md.SyncDepartmentId)).ToList();
            managements.ForEach(i => i.HousingDepartmentList = i.HousingDepartmentList.Where(d => !d.IsDeleted).ToList());
            var mappedData = managements.Map<IEnumerable<IManagementDepartmentModel>>();
            return mappedData;
        }

        public IDictionary<Guid, IEnumerable<Guid>> GetManagementsToHousingDepartmentsRelation(IEnumerable<Guid> managementDepartmentsIds)
        {
            return managementRepository.Query.Where(m => managementDepartmentsIds.Contains(m.Id))
                .Select(m => new {Key = m.Id, Value = m.HousingDepartmentList.Select(h => h.Id)})
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void DeactivateManagements(IEnumerable<string> updatedManagements, IEnumerable<ManagementDepartment> savedManagements)
        {
            var deletedManagements = savedManagements.Where(i => !updatedManagements.Contains(i.SyncDepartmentId)).ToList();
            foreach (var management in deletedManagements)
            {
                management.IsDeleted = true;
                management.HousingDepartmentList.ForEach(i => i.IsDeleted = true);
                managementRepository.Save(management);
            }
        }

        private List<HousingDepartment> GetUpdatedDepartments(ManagementDepartment management, IEnumerable<ManagementDepartment> savedManagements)
        {
            var result = new List<HousingDepartment>();

            var savedDepartments = savedManagements.SelectMany(i => i.HousingDepartmentList).ToList();
            foreach (var department in management.HousingDepartmentList)
            {
                department.IsDeleted = false;
                var savedDepartment = savedDepartments.FirstOrDefault(i => i.SyncDepartmentId == department.SyncDepartmentId);
                if (savedDepartment != null)
                {
                    department.Id = savedDepartment.Id;
                }
                result.Add(department);
            }

            var savedManagement = savedManagements.FirstOrDefault(i => i.SyncDepartmentId == management.SyncDepartmentId);
            if (savedManagement == null)
            {
                return result;
            }

            foreach (var oldDepartment in savedManagement.HousingDepartmentList)
            {
                if (result.Any(i => i.SyncDepartmentId == oldDepartment.SyncDepartmentId))
                {
                    continue;
                }
                oldDepartment.IsDeleted = true;
                result.Add(oldDepartment);
            }

            return result;
        }

        private IHousingDepartmentModel MapHousingDepartment(HousingDepartment data, Guid managementId)
        {
            var model = data.Map<IHousingDepartmentModel>();
            model.ManagementDepartmentId = managementId;
            return model;
        }
    }
}
