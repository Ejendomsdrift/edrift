using System;

namespace Infrastructure.Helpers
{
    public interface IPathHelper
    {
        string GetAvatarPath();
        string GetAvatarPath(Guid memberId);
        string GetAvatarPathForSync();
        string GetAvatarPathForSync(Guid memberId);
        string GetJobAssignUploadsPath(Guid jobAssignId, Guid fileId, string extension);
        string GetDayAssignUploadsPath(Guid dayAssignId, Guid fileId, string extension);
        string GetJobAssignDirectoryPath(Guid jobAssignId);
    }
}
