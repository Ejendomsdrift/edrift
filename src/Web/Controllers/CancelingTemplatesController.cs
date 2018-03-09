using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using CancellingTemplatesCore.Contract.Interfaces;
using Web.Models;
using YearlyPlanning.Contract.Enums;

namespace Web.Controllers
{
    [RoutePrefix("api/cancelingTemplates")]
    public class CancelingTemplatesController : ApiController
    {
        private readonly ICancelingTemplatesService cancelingTemplatesService;

        public CancelingTemplatesController(ICancelingTemplatesService cancelingTemplatesService)
        {
            this.cancelingTemplatesService = cancelingTemplatesService;
        }

        [HttpGet, Route("getByFilter")]
        public IEnumerable<ICancelingTemplateModel> GetByFilter(bool isCoordinatorReason)
        {
            var result = cancelingTemplatesService.GetByFilter(isCoordinatorReason);
            return result;
        }

        [HttpGet, Route("getAllByTaskType")]
        public IEnumerable<ICancelingTemplateModel> GetAllByTaskType(JobTypeEnum taskType)
        {
            var result = cancelingTemplatesService.GetAllByTaskType(taskType, isCoordinatorReason: false);
            return result;
        }

        [HttpGet, Route("getCoordinatorByTaskType")]
        public IEnumerable<ICancelingTemplateModel> GetCoordinatorByTaskType(JobTypeEnum taskType)
        {
            var result = cancelingTemplatesService.GetAllByTaskType(taskType, isCoordinatorReason: true);
            return result;
        }

        [HttpPost, Route("delete")]
        public void DeleteTemplate(Guid templateId)
        {
            cancelingTemplatesService.Delete(templateId);
        }

        [HttpPost, Route("create")]
        public ICancelingTemplateModel CreateTemplate(CancellingTemplateViewModel model)
        {
            var mappedModel = Mapper.Map<ICancelingTemplateModel>(model);
            return cancelingTemplatesService.Save(mappedModel);
        }
    }
}