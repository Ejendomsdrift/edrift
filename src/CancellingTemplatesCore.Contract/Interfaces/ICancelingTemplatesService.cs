using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Enums;

namespace CancellingTemplatesCore.Contract.Interfaces
{
    public interface ICancelingTemplatesService
    {
        IEnumerable<ICancelingTemplateModel> GetByFilter(bool isCoordinatorReasons);
        IEnumerable<ICancelingTemplateModel> GetAllByTaskType(JobTypeEnum taskType, bool isCoordinatorReason);
        ICancelingTemplateModel GetById(Guid cancelingTemplateId);
        IEnumerable<ICancelingTemplateModel> GetByIds(IEnumerable<Guid> cancelingTemplateId);
        ICancelingTemplateModel Save(ICancelingTemplateModel model);
        void Delete(Guid cancelingTemplateId);
    }
}