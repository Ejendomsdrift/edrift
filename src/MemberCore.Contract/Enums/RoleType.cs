using Infrastructure.CustomAttributes;

namespace MemberCore.Contract.Enums
{
    public enum RoleType
    {
        [EnumSorting(SortIndex = 4)]
        Administrator = 1,

        [EnumSorting(SortIndex = 1)]
        Janitor = 2,

        [EnumSorting(SortIndex = 2)]
        Coordinator = 3,

        [EnumSorting(SortIndex = 3)]
        SuperAdmin = 4
    }
}
