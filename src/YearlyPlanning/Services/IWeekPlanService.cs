using System;
using System.Threading.Tasks;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Services
{
    public interface IWeekPlanService
    {
        IWeekJobsResultModel GetJobsForWeek(IWeekPlanFilterModel filter);

        IWeekPlanGridModel GetWeekPlanGridModel(IWeekPlanFilterModel filter);

        Task MoveExpiredJobs(bool checkWeek);

        string GetUploadedFileLink(UploadFileModel model, Guid dayAssignId);
    }
}
