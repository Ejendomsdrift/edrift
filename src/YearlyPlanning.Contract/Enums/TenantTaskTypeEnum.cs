using Infrastructure.CustomAttributes;

namespace YearlyPlanning.Contract.Enums
{
    public enum TenantTaskTypeEnum
    {
        [LocalizationKey(Key = "TenantTaskType_Plumbing")]
        Plumbing,
        [LocalizationKey(Key = "TenantTaskType_Carpentry")]
        Carpentry,
        [LocalizationKey(Key = "TenantTaskType_Electricity")]
        Electricity,
        [LocalizationKey(Key = "TenantTaskType_Others")]
        Others
    }
}
