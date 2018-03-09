namespace Infrastructure.Constants
{
    public class Constants
    {
        public static class Common
        {
            public const int DefaultMemberDayAhead = 1;
            public const int MinutesInOneHour = 60;
            public const int DecimalCount = 2;
            public const int LunchTimeInMinutes = 60;
            public const string CategoryPathDelimeter = "/";
        }

        public static class DateTime
        {
            public const int SundayNumber = 7;
            public const int BoundaryDayNumber = 4; //first week in year/month should contain thursday
            public const int DaysInWeek = 7;
            public const int WeeksInYear = 52;
            public const int MonthsInYear = 12;
            public const string CsvDateFormat = "dd-MM-yyyy";
        }

        public static class AppSetting
        {
            public const string MemberAvatarPath = "memberAvatarPath";
            public const string MemberAvatarPathForSync = "memberAvatarPathForSync";
            public const string MemberAvatarFolderPath = "memberAvatarFolderPath";
            public const string MemberAvatarFolderPathForSync = "memberAvatarFolderPathForSync";
            public const string JobAssignUploadsPath = "jobAssignUploadsPath";
            public const string DayAssignUploadsPath = "dayAssignUploadsPath";
            public const string IsADFSLogin = "IsADFSLogin";
            public const string TaskPrefixZeroesCount = "taskPrefixZeroesCount";
            public const string DefaultTaskStartUtcTime = "defaultTaskStartUtcTime";
            public const string DaysWorkingMinutes = "DaysWorkingMinutes";
            public const string FileExtensions = "fileExtensions";
            public const string AllowedRolesForAssigningOnJob = "allowedRolesForAssigningOnJob";
            public const string SwitchPlatformButton = "switchPlatformButton";
            public const string CsvSeparator = "csvSeparator";
            public const string AllowedStatusesForMovingInExpiredStatus = "allowedStatusesForMovingInExpiredStatus";
            public const string DefaultUserName = "defaultUserName";
            public const string MiscCategoryName = "MiscCategoryName";
            public const string TaskTypesDisplayColors = "TaskTypesDisplayColors";
            public const string HangfireDBName = "HangfireDbName";
            public const string DefaultPassword = "DefaultPassword";
            public const string SyncUrl = "SyncUrl";
        }

        public static class ChartConfigurationKeys
        {
            public const string Groupings_FacilityTasksVsTenantTasks = "Groupings_FacilityTasksVsTenantTasks";
            public const string Groupings_UnprocessedVsProcessedTasks = "Groupings_UnprocessedVsProcessedTasks";
            public const string Groupings_CompletedVsOverdueTasks = "Groupings_CompletedVsOverdueTasks";
            public const string TenantSpentTimeChartTypes = "TenantSpentTimeChartTypes"; 
            public const string FacilitySpentTimeChartTypes = "FacilitySpentTimeChartTypes";
            public const string TenantTasksVsVisitsAmountChartTypes = "TenantTasksVsVisitsAmountChartTypes";

            public const string AllowedStatuses_TenantTasksVsVisitsAmount = "AllowedStatuses_TenantTasksVsVisitsAmount";
            public const string AllowedStatuses_FacilityTasksVsTenantTasks = "AllowedStatuses_FacilityTasksVsTenantTasks";
            public const string AllowedStatuses_UnprocessedVsProcessedTasks = "AllowedStatuses_UnprocessedVsProcessedTasks";
            public const string AllowedStatuses_SpentTimeVsTenantTasks = "AllowedStatuses_SpentTimeVsTenantTasks";
            public const string AllowedStatuses_SpentTimeVsFacilityTasks = "AllowedStatuses_SpentTimeVsFacilityTasks";
            public const string AllowedStatuses_CompletedVsOverdueTasks = "AllowedStatuses_CompletedVsOverdueTasks";
            public const string AllowedStatuses_TenantTaskVsRejectedReason = "AllowedStatuses_TenantTaskVsRejectedReason";
        }

        public static class ChartName
        {
            public const string SpentTimeVsTenantTasks = "spentTimeVsTenantTasks";
            public const string SpentTimeVsFacilityTasks = "spentTimeVsFacilityTasks";
            public const string CompletedVsOverdueTasks = "completedVsOverdueTasks";
            public const string FacilityTasksVsTenantTasks = "facilityTasksVsTenantTasks";
            public const string TenantTasksVsVisitsAmount = "tenantTasksVsVisitsAmount";
            public const string AbsencesData = "absencesData";
            public const string CancelingReason = "cancelingReason";
            public const string UnprocessedVsProcessed = "unprocessedVsProcessed";
        }

        public static class Upload
        {
            public const string UploadPath = "~/Files/JobsAssignAttachments";
            public const string DayAssignUploadPath = "~/Files/DayAssignChangeStatusAttachments";
        }

        public static class TaskId
        {
            public const string Facility = "dp";
            public const string AdHoc = "d";
            public const string Tenant = "bs";
            public const string Other = "a";
        }

        public static class TaskTypeKey
        {
            public const string FacilityTaskTypeKey = "FacilityTask_Type";
            public const string AdHocTaskTypeKey = "AdHocTask_Type";
            public const string TenantTaskType = "TenantTask_Type";
            public const string OtherTaskType = "OtherTask_Type";
        }

        public static class TenantyTypeKey
        {
            public const string Carpentry = "tenantType-Carpentry";
            public const string Electricity = "tenantType-Electricity";
            public const string Other = "tenantType-Others";
            public const string Plumbing = "tenantType-Plumbing";
        }

        public static class StatusKey
        {
            public const string PendingKey = "CoordinatorPlatform_Status_Pending";
            public const string OpenedKey = "CoordinatorPlatform_Status_Opened";
            public const string InProgressKey = "CoordinatorPlatform_Status_InProgress";
            public const string PausedKey = "CoordinatorPlatform_Status_Paused";
            public const string CompletedKey = "CoordinatorPlatform_Status_Completed";
            public const string CanceledKey = "CoordinatorPlatform_Status_Canceled";
            public const string AssignedKey = "CoordinatorPlatform_Status_Assigned";
            public const string RejectedKey = "CoordinatorPlatform_Status_Rejected";
            public const string ExpiredKey = "CoordinatorPlatform_Status_Expired";
        }
    }
}