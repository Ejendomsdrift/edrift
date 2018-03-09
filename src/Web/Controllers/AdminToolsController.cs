using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using CategoryCore.Contract.Commands;
using Infrastructure.Constants;
using Infrastructure.Messaging;
using MongoRepository.Contract.Interfaces;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;

namespace Web.Controllers
{
    [RoutePrefix("api/admintools")]
    public class AdminToolsController : ApiController
    {
        private readonly IMessageBus messageBus;
        private readonly IMongoDatabaseRepository dataBaseRepository;
        private readonly IDayAssignProvider _dayAssignProvider;

        public AdminToolsController(
            IMessageBus messageBus,
            IMongoDatabaseRepository dataBaseRepository,
            IDayAssignProvider dayAssignProvider)
        {
            this.messageBus = messageBus;
            this.dataBaseRepository = dataBaseRepository;
            _dayAssignProvider = dayAssignProvider;
        }

        [HttpGet, Route("status")]
        public object Status()
        {
            return new { CurrentTime = DateTime.UtcNow.ToShortTimeString() };
        }

        [HttpPost, Route("createDefaultCategories")]
        public async Task CreateDefaultCategories()
        {
            await FirstCategory();
            await SecondCategory();
            await ThirdCategory();
            await ForthCategory();
        }

        [HttpGet, Route("getDataBase")]
        public List<string> GetDataBase()
        {
            var result = dataBaseRepository.GetAllCollections().Where(c => c != "system.indexes").ToList();
            return result;
        }

        [HttpPost, Route("clearDataBase")]
        public async Task ClearDataBase(string collections)
        {
            var collectionsToDrop = collections.Split(',').ToList();
            await dataBaseRepository.DropDataBase(collectionsToDrop);
        }

        #region utils

        private async Task FirstCategory()
        {
            var root = Guid.NewGuid();
            var firstMainColor = "#fce7ae";
            await messageBus.Publish(new CreateCategory(root, null, "Bygning", firstMainColor, visible: true));
            var klimaskrm = Guid.NewGuid();
            await messageBus.Publish(new CreateCategory(klimaskrm, root, "Klimaskærm", firstMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Fundament", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Facade", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Tag", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Altan/altangange", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Trapper/ramper", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), klimaskrm, "Døre/vinduer/porte/lemme", firstMainColor, visible: true));

            var faelles = Guid.NewGuid();
            await messageBus.Publish(new CreateCategory(faelles, root, "Fælles", firstMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), faelles, "Indvendig", firstMainColor, visible: true));
            var tekniske = Guid.NewGuid();
            await messageBus.Publish(new CreateCategory(tekniske, root, "Tekniske installationer", firstMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Afløb", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "El og belysning", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Gas", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Vand", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Varme", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Vaskeri fælles", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Ventilation", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), tekniske, "Øvrige", firstMainColor, visible: true));

            var bolig = Guid.NewGuid();
            await messageBus.Publish(new CreateCategory(bolig, root, "Bolig", firstMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), bolig, "Afløb", firstMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), bolig, "El og belysning", firstMainColor, visible: true));
        }

        private async Task SecondCategory()
        {
            var root = Guid.NewGuid();
            var secondMainColor = "#c0dba6";
            await messageBus.Publish(new CreateCategory(root, null, "Terræn", secondMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Inventar", secondMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Teknisk anlæg", secondMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Konstruktion", secondMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Beplantning", secondMainColor, visible: true));
        }

        private async Task ThirdCategory()
        {
            var root = Guid.NewGuid();
            var thirdMainColor = "#c2d2dd";
            await messageBus.Publish(new CreateCategory(root, null, "Materiel", thirdMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Kørende", thirdMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Andet", thirdMainColor, visible: true));
        }

        private async Task ForthCategory()
        {
            var root = Guid.NewGuid();
            var forthMainColor = "#A9A9A9";
            await messageBus.Publish(new CreateCategory(root, null, "Andet", forthMainColor, visible: true));

            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Affald", forthMainColor, visible: true));
            await messageBus.Publish(new CreateCategory(Guid.NewGuid(), root, "Uden For kategori", forthMainColor, visible: true));
        }

        #endregion
    }
}