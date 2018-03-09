using System;
using Infrastructure.Extensions;
using MemberCore.Contract.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translations.Interfaces;
using Translations.Models;

namespace Translations.Implementation
{
    public class TranslationService : ITranslationService
    {
        private readonly ITranslationRepository translationRepository;
        private readonly ITranslationLogRepository logRepository;
        private readonly IMemberService memberService;

        public TranslationService(ITranslationRepository translationRepository, ITranslationLogRepository logRepository, IMemberService memberService)
        {
            this.translationRepository = translationRepository;
            this.logRepository = logRepository;
            this.memberService = memberService;
        }

        public IDictionary<string, string> Get(IEnumerable<string> keys, string language)
        {
            var keysList = keys as IList<string> ?? keys.ToList();
            CreateTranslationsIfNotExist(keysList, language);
            var result = translationRepository.Query
                .Where(r => keysList.Contains(r.Alias))
                .ToDictionary(resource => resource.Alias, resource => resource.Translations.FirstOrDefault(l => l.Language == language)?.Value);
            return result;
        }

        public async Task<Dictionary<string, string>> Translations(string language)
        {
            var resources = await translationRepository.GetAll();
            var result = resources.ToDictionary(x => x.Alias, x => x.Translations.FirstOrDefault(y => y.Language == language)?.Value);
            return result;
        }

        public async Task<IEnumerable<ResourceModel>> All()
        {
            var resources = await translationRepository.GetAll();
            var result = resources.OrderBy(r => r.Alias)
                .SelectMany(r => r.Translations.Select(x => new ResourceModel
                {
                    Alias = r.Alias,
                    Description = r.Description,
                    Language = x.Language,
                    Translation = x.Value
                }));
            return result;
        }

        public async Task<IEnumerable<string>> Languages()
        {
            var resources = await translationRepository.GetAll();
            var distinct = resources.SelectMany(r => r.Translations).Select(t => t.Language).Distinct();
            return distinct;
        }

        public async Task<ResourceModel> Get(string alias, string language)
        {
            if (alias.IsNullOrEmpty()) return new ResourceModel();

            var resourceResult = await translationRepository.Get(alias) ?? new Resource();

            var result = new ResourceModel
            {
                Alias = resourceResult.Alias,
                Description = resourceResult.Description,
                Language = language,
                Translation = resourceResult.Translations.FirstOrDefault(y => y.Language == language)?.Value
            };

            return result;
        }

        public void Save(ResourceModel model)
        {
            if (model.Alias.IsNullOrEmpty()) return;

            if (model.Language.IsNullOrEmpty()) model.Language = LanguageKey.Default;

            var resource = translationRepository.Query.FirstOrDefault(i => i.Alias == model.Alias) ?? new Resource { Alias = model.Alias };

            resource.Description = model.Description;

            var translation = resource.Translations.FirstOrDefault(x => x.Language == model.Language);
            if (translation == null)
            {
                resource.Translations.Add(new Translation { Language = model.Language, Value = model.Translation ?? model.Alias });
            }
            else
            {
                translation.Value = model.Translation ?? model.Alias;
            }

            var log = GetLog(resource);

            translationRepository.Save(resource);
            logRepository.Save(log);
        }

        public async Task Delete(string alias)
        {
            var log = GetLog(alias);

            await translationRepository.Delete(alias);
            logRepository.Save(log);
        }

        public void Import(string json)
        {
            var resources = json.Deserialize<IList<Resource>>();

            foreach (var resource in resources)
            {
                var log = GetLog(resource);

                translationRepository.Save(resource);
                logRepository.Save(log);
            }
        }

        public async Task<Stream> Export()
        {
            var resources = await translationRepository.GetAll();
            var json = resources.ToJson();
            var buffer = Encoding.UTF8.GetBytes(json);
            return new MemoryStream(buffer);
        }

        private IEnumerable<string> GetLanguages()
        {
            var languages = translationRepository.Query.SelectMany(r => r.Translations.Select(t => t.Language)).Distinct().ToList();
            return languages;
        }

        private void CreateTranslationsIfNotExist(IEnumerable<string> keys, string language)
        {
            var keysList = keys as IList<string> ?? keys.ToList();
            var existingResources = translationRepository.Query.Where(r => keysList.Contains(r.Alias)).ToList();
            var absentResources = keysList.Except(existingResources.Select(t => t.Alias)).ToList();

            absentResources.ForEach(t =>
            {
                translationRepository.Save(new Resource
                {
                    Alias = t,
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            Language = language,
                            Value = t
                        }
                    }
                });
            });

            var resourcesWithAbsentTranslation = translationRepository.Query
                .Where(r => keysList.Contains(r.Alias) && !r.Translations.Any(t => t.Language == language))
                .ToList();

            resourcesWithAbsentTranslation.ForEach(r =>
            {
                r.Translations.Add(new Translation { Language = language, Value = r.Alias });
                translationRepository.Save(r);
            });
        }

        private ResourceLog GetLog(string alias)
        {
            ResourceLog log = logRepository.Get(alias);

            var newLog = new ResourceLog
            {
                ActionType = ActionTypes.Delete,
                UserId = memberService.GetCurrentUser().MemberId,
                PreviousLogId = log.Id
            };

            return newLog;
        }

        private ResourceLog GetLog(Resource resource)
        {
            IMemberModel currentUser = memberService.GetCurrentUser();
            ResourceLog previousLog = logRepository.Get(resource.Alias);

            var log = new ResourceLog
            {
                UserId = currentUser.MemberId,
                Resource = resource,
                PreviousLogId = previousLog?.Id ?? Guid.Empty,
                ActionType = previousLog == null ? ActionTypes.Create : ActionTypes.Modify
            };

            return log;
        }
    }
}