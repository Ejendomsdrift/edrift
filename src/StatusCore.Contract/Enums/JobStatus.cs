using Infrastructure.CustomAttributes;

namespace StatusCore.Contract.Enums
{
    public enum JobStatus
    {
        [EnumSorting(SortIndex = 7)]
        Pending,

        [EnumSorting(SortIndex = 6)]
        Opened,

        [EnumSorting(SortIndex = 3)]
        InProgress,

        [EnumSorting(SortIndex = 4)]
        Paused,

        [EnumSorting(SortIndex = 1)]
        Completed,

        [EnumSorting(SortIndex = 0)]
        Canceled,

        [EnumSorting(SortIndex = 5)]
        Assigned,

        [EnumSorting(SortIndex = 2)]
        Rejected,

        [EnumSorting(SortIndex = 8)]
        Expired
    }
}
