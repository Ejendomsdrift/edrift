using System;
using System.Collections.Generic;
using System.Web.Http;
using AbsenceTemplatesCore.Contract.Interfaces;

namespace Web.Controllers
{
    [RoutePrefix("api/absenceTemplates")]
    public class AbsenceTemplatesController : ApiController
    {
        private readonly IAbsenceTemplatesService absenceTemplatesService;

        public AbsenceTemplatesController(IAbsenceTemplatesService absenceTemplatesService)
        {
            this.absenceTemplatesService = absenceTemplatesService;
        }

        [HttpGet, Route("GetAll")]
        public IEnumerable<IAbsenceTemplateModel> GetAbsenceTemplates()
        {
            var result = absenceTemplatesService.GetAll();
            return result;
        }

        [HttpPost, Route("Delete")]
        public void DeleteTemplate(Guid templateId)
        {
            absenceTemplatesService.Delete(templateId);
        }

        [HttpPost, Route("Create")]
        public IAbsenceTemplateModel CreateTemplate(string templateText)
        {
            return absenceTemplatesService.Save(templateText);
        }
    }
}