using System;
using System.Collections.Generic;
using System.Linq;
using MemberCore.Contract.Interfaces;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Models;
using YearlyPlanning.ReadModel;

namespace YearlyPlanning.Services
{
    public class GuideCommentService: IGuideCommentService
    {
        private readonly IRepository<GuideComment> guideCommentRepository;
        private readonly IMemberService memberService;

        public GuideCommentService(IRepository<GuideComment> guideCommentRepository, IMemberService memberService)
        {
            this.guideCommentRepository = guideCommentRepository;
            this.memberService = memberService;
        }

        public void SaveOrUpdate(Guid memberId, string jobId, Guid dayAssignId, string comment, Guid? commentId = null)
        {
            GuideComment model = new GuideComment
            {
                Id = commentId ?? Guid.Empty,
                MemberId = memberId,
                DayAssignId = dayAssignId,
                JobId = jobId,
                Date = DateTime.UtcNow,
                Comment = comment
            };

            guideCommentRepository.Save(model);
        }

        public void SaveOrUpdateGuideComment(IGuideCommentModel commentModel)
        {
            GuideComment model = new GuideComment
            {
                Id = commentModel.Id ?? Guid.NewGuid(),
                MemberId = commentModel.MemberId,
                DayAssignId = commentModel.DayAssignId,
                JobId = commentModel.JobId,
                Date = DateTime.UtcNow,
                Comment = commentModel.Comment
            };

            guideCommentRepository.Save(model);
        }

        public void RemoveGuideComment(Guid commentId)
        {
            guideCommentRepository.Delete(comment => comment.Id == commentId);
        }

        public IEnumerable<IGuideCommentModel> GetGuideJobCommentsByJobId(string jobId)
        {
            IEnumerable<GuideComment> comments = guideCommentRepository.Query.Where(c => c.JobId == jobId);
            return comments.Select(MapCommentGuideModel);
        }

        private IGuideCommentModel MapCommentGuideModel(GuideComment commentModel)
        {
            IMemberModel member = memberService.GetById(commentModel.MemberId);

            return new GuideCommentModel
            {
                Comment = commentModel.Comment,
                DayAssignId = commentModel.DayAssignId,
                Id = commentModel.Id,
                JobId = commentModel.JobId,
                MemberId = commentModel.MemberId,
                MemberAvatar = member.Avatar,
                MemberName = member.UserName,
                Date = commentModel.Date
            };
        }

    }
}
