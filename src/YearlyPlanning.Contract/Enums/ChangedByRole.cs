using System;

namespace YearlyPlanning.Contract.Enums
{
    [Flags]
    public enum ChangedByRole
    {
        None = 0,
        Administrator = 1,
        Coordinator = 2,
        AdministratorAndCoordinator = Administrator | Coordinator
    }
}