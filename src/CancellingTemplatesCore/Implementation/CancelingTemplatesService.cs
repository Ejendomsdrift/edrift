using System;
using System.Collections.Generic;
using System.Linq;
using CancellingTemplatesCore.Contract.Interfaces;
using CancellingTemplatesCore.Models;
using Infrastructure.Extensions;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Enums;

namespace CancellingTemplatesCore.Implementation
{
    public class CancelingTemplatesService : ICancelingTemplatesService
    {
        private readonly IRepository<CancellingTemplate> repository;

        public CancelingTemplatesService(IRepository<CancellingTemplate> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<ICancelingTemplateModel> GetByFilter(bool isCoordinatorReasons)
        {
            var result = repository.Find(t => !t.IsDeleted && t.IsCoordinatorReason == isCoordinatorReasons);
            var mappedResult = result.Map<IEnumerable<ICancelingTemplateModel>>();
            return mappedResult;
        }

        public IEnumerable<ICancelingTemplateModel> GetAllByTaskType(JobTypeEnum taskType, bool isCoordinatorReason)
        {
            var result = repository.Find(t => !t.IsDeleted && t.IsCoordinatorReason == isCoordinatorReason && t.JobTypeList.Contains(taskType));
            var mappedResult = result.Map<IEnumerable<ICancelingTemplateModel>>();
            return mappedResult;
        }

        public ICancelingTemplateModel Save(ICancelingTemplateModel model)
        {
            var template = model.Map<CancellingTemplate>(); 
            template.Id = Guid.NewGuid();

            repository.Save(template);
            return GetById(template.Id);
        }

        public ICancelingTemplateModel GetById(Guid cancelingTemplateId)
        {
            var result = repository.FindOne(t => t.Id == cancelingTemplateId);
            var mapperResult = result.Map<ICancelingTemplateModel>();
            return mapperResult;
        }

        public IEnumerable<ICancelingTemplateModel> GetByIds(IEnumerable<Guid> cancelingTemplateIds)
        {
            if (!cancelingTemplateIds.HasValue())
            {
                return Enumerable.Empty<ICancelingTemplateModel>();
            }

            var result = repository.Query.Where(i => cancelingTemplateIds.Contains(i.Id)).ToList();
            var mapperResult = result.Map<IEnumerable<ICancelingTemplateModel>>();
            return mapperResult;
        }

        public void Delete(Guid cancelingTemplateId)
        {
            repository.UpdateManySingleProperty(t => t.Id == cancelingTemplateId, m => m.IsDeleted, true);
        }
    }
}