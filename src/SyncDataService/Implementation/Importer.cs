using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyncDataService.Interfaces;
using SyncDataService.Models;

namespace SyncDataService.Implementation
{
    public class Importer : IImporter
    {
        private readonly DbClient dbClient;

        public Importer(DbClient dbClient)
        {
            this.dbClient = dbClient;
        }

        public async Task<List<MemberBO>> ImportMembers()
        {
            var members = await dbClient.Get(MemberBO.Query, MemberBO.FromDataRow);
            var roles = await ImportRoles();

            foreach (var member in members)
            {
                member.RoleList = roles.Where(i => i.AccountName == member.UserName).ToList();               
            }

            return members;
        }

        public async Task<List<ManagementDepartmentBO>> ImportManagements()
        {
            var managements = await dbClient.Get(ManagementDepartmentBO.Query, ManagementDepartmentBO.FromDataRow);
            var departments = await ImportDepartments();
            var departmentsData = await ImportHousingDepartmentsDepartmentsData();
            foreach (var housingDepartment in departments)
            {
                var housingDepartmentDataList = departmentsData.Where(d => d.HousingDepartmentId == housingDepartment.SyncDepartmentId).ToList();
                housingDepartment.AddressList = housingDepartmentDataList.Select(d => d.Address).ToList();
            }
            
            foreach (var management in managements)
            {
                management.HousingDepartmentList = departments.Where(i => i.ManagementDepartmentId == management.SyncDepartmentId).ToList();
            }

            return managements;
        }

        public async Task<LastReplicationInfoModel> GetLastReplicationDates()
        {
            var departmentLastReplication = await GetLastReplicationByTableName("Boligafdelinger");
            var departmentDataLastSyncDate = await GetLastReplicationByTableName("Boligdata");
            var managementLastReplication = await GetLastReplicationByTableName("Driftsafdelinger");
            var memberLastLastReplication = await GetLastReplicationByTableName("Medarbejdere");

            return new LastReplicationInfoModel
            {
                DepartmentsLastSyncDate = departmentLastReplication.DatoTid,
                DepartmentDataLastSyncDate = departmentDataLastSyncDate.DatoTid,
                ManagementLastSyncDate = managementLastReplication.DatoTid,
                MembersLastSyncDate = memberLastLastReplication.DatoTid
            };
        }

        private Task<List<RoleBO>> ImportRoles()
        {
            return dbClient.Get(RoleBO.Query, RoleBO.FromDataRow);
        }

        private Task<List<HousingDepartmentBO>> ImportDepartments()
        {
            return dbClient.Get(HousingDepartmentBO.Query, HousingDepartmentBO.FromDataRow);
        }

        private Task<List<DepartmentDataBO>> ImportHousingDepartmentsDepartmentsData()
        {
            return dbClient.Get(DepartmentDataBO.Query, DepartmentDataBO.FromDataRow);
        }

        private async Task<ReplikationStatsBO> GetLastReplicationByTableName(string tableName)
        {
            var list = await dbClient.Get(string.Format(ReplikationStatsBO.Query, tableName), ReplikationStatsBO.FromDataRow);
            return list.FirstOrDefault();
        }
    }
}