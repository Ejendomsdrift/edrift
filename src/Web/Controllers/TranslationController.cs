using Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Translations;
using Translations.Interfaces;
using Translations.Models;
using Web.Core;

namespace Web.Controllers
{
    [RoutePrefix("api/translation")]
    public class TranslationController : ApiController
    {
        private readonly ITranslationService translationService;

        public TranslationController(ITranslationService translationService)
        {
            this.translationService = translationService;
        }

        [AllowAnonymous]
        [HttpGet, Route("translations"), StopCamelCase]
        public Task<Dictionary<string, string>> Translations(string language = LanguageKey.Default)
        {
            return translationService.Translations(language);
        }

        [HttpGet, Route("all")]
        public Task<IEnumerable<ResourceModel>> All()
        {
            return translationService.All();
        }

        [HttpGet, Route("languages")]
        public Task<IEnumerable<string>> Languages()
        {
            return translationService.Languages();
        }

        [HttpPost, Route("save")]
        public Task<ResourceModel> Save(ResourceModel model)
        {
            translationService.Save(model);
            return translationService.Get(model.Alias, model.Language);         
        }

        [HttpPost, Route("createAndReload")]
        public Task<IEnumerable<ResourceModel>> CreateAndReload(ResourceModel model)
        {
            translationService.Save(model);
            return translationService.All();
        }

        [HttpPost, Route("delete")]
        public Task Delete(string alias)
        {
            return translationService.Delete(alias);
        }

        [HttpPost, Route("import")]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var content = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.FileName.IsNotNullOrEmpty());

            if (content == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            var json = await content.ReadAsStringAsync();
            translationService.Import(json);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet, Route("export")]
        public async Task<HttpResponseMessage> Export()
        {
            var stream = await translationService.Export();
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(WebConstants.MimeType.ApplicationJson);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue(WebConstants.MimeType.Attachment)
            {
                FileName = $"Export_Resources_{DateTime.UtcNow:yyyy-MM-ddTHH-mm}.json"
            };

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        }
    }
}