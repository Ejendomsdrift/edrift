using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Interfaces
{
    public interface IGuideCommentService
    {
        void SaveOrUpdate(Guid memberId, string jobId, Guid dayAssignId, string comment, Guid? commentId = null);

        IEnumerable<IGuideCommentModel> GetGuideJobCommentsByJobId(string jobId);

        void SaveOrUpdateGuideComment(IGuideCommentModel commentModel);

        void RemoveGuideComment(Guid commentId);
    }
}
