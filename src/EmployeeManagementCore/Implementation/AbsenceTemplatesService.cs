using System;
using System.Collections.Generic;
using AbsenceTemplatesCore.Contract.Interfaces;
using AbsenceTemplatesCore.Models;
using Infrastructure.Extensions;
using MongoRepository.Contract.Interfaces;

namespace AbsenceTemplatesCore.Implementation
{
    public class AbsenceTemplatesService : IAbsenceTemplatesService
    {
        private readonly IRepository<AbsenceTemplate> repository;

        public AbsenceTemplatesService(IRepository<AbsenceTemplate> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<IAbsenceTemplateModel> GetAll()
        {
            var result = repository.Find(t => !t.IsDeleted);
            var mappedResult = result.Map<IEnumerable<IAbsenceTemplateModel>>();
            return mappedResult;
        }

        public IAbsenceTemplateModel Save(string templateText)
        {
            var template = new AbsenceTemplate() {Text = templateText};
            repository.Save(template);
            return GetById(template.Id);
        }

        public IAbsenceTemplateModel GetById(Guid absenceTemplateId)
        {
            var result = repository.FindOne(t => t.Id == absenceTemplateId && !t.IsDeleted);
            var mappperResult = result.Map<IAbsenceTemplateModel>();
            return mappperResult;
        }

        public void Delete(Guid absenceTemplateId)
        {
            repository.UpdateManySingleProperty(t => t.Id == absenceTemplateId, m => m.IsDeleted, true);
        }
    }
}