using System;
using System.Collections.Generic;
using Statistics.Core.Models;
using YearlyPlanning.Contract.Interfaces;

namespace Statistics.Core.Implementation
{
    public interface ITasksInfoBuilder
    {
        List<TaskInfo> GetTaskInfoList(IEnumerable<IDayAssign> dayAssigns, bool showLastCompletedOrCanceledStatus, bool onlyWithTime = false);
        IEnumerable<TaskInfo> BuildByCategoryGrouping(IDictionary<Guid, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus);
        IEnumerable<TaskInfo> BuildByIsOverdueGrouping(IDictionary<string, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus);
        IEnumerable<TaskInfo> BuildByTenantTypeGrouping(IDictionary<string, IEnumerable<IDayAssign>> groupedTasks, bool showLastCompletedOrCanceledStatus);
    }
}