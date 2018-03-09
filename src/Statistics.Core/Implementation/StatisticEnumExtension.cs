using Infrastructure.Constants;
using StatusCore.Contract.Enums;
using System;
using System.Collections.Generic;
using Translations;
using Translations.Interfaces;
using Translations.Models;
using YearlyPlanning.Contract.Enums;

namespace Statistics.Core.Implementation
{
    public static class StatisticEnumExtension
    {
        public static string GetTaskTypeLabel(this JobTypeEnum jobType, ITranslationService translationService)
        {
            var taskKeys = new[]
            {
                Constants.TaskTypeKey.FacilityTaskTypeKey,
                Constants.TaskTypeKey.AdHocTaskTypeKey,
                Constants.TaskTypeKey.TenantTaskType,
                Constants.TaskTypeKey.OtherTaskType
            };

            IDictionary<string, string> translationList = translationService.Get(taskKeys, LanguageKey.Default);

            switch (jobType)
            {
                case JobTypeEnum.Facility:
                    return translationList[Constants.TaskTypeKey.FacilityTaskTypeKey];
                case JobTypeEnum.AdHock:
                    return translationList[Constants.TaskTypeKey.AdHocTaskTypeKey];
                case JobTypeEnum.Tenant:
                    return translationList[Constants.TaskTypeKey.TenantTaskType];
                case JobTypeEnum.Other:
                    return translationList[Constants.TaskTypeKey.OtherTaskType];
                default:
                    throw new NotImplementedException($"No such task type {jobType}");
            }
        }

        public static string GetTenantTypeTaskLabel(this TenantTaskTypeEnum type, ITranslationService translationService)
        {
            var typeKeys = new[]
            {
                Constants.TenantyTypeKey.Carpentry,
                Constants.TenantyTypeKey.Electricity,
                Constants.TenantyTypeKey.Other,
                Constants.TenantyTypeKey.Plumbing
            };

            IDictionary<string, string> translationList = translationService.Get(typeKeys, LanguageKey.Default);

            switch (type)
            {
                case TenantTaskTypeEnum.Carpentry:
                    return translationList[Constants.TenantyTypeKey.Carpentry];
                case TenantTaskTypeEnum.Electricity:
                    return translationList[Constants.TenantyTypeKey.Electricity];
                case TenantTaskTypeEnum.Others:
                    return translationList[Constants.TenantyTypeKey.Other];
                case TenantTaskTypeEnum.Plumbing:
                    return translationList[Constants.TenantyTypeKey.Plumbing];
                default:
                    throw new NotImplementedException($"No such task type {type}");
            }
        }

        public static string GetStatusLabel(this JobStatus jobStatus, ITranslationService translationService)
        {
            var statusKeys = new[]
            {
                Constants.StatusKey.PendingKey,
                Constants.StatusKey.OpenedKey,
                Constants.StatusKey.InProgressKey,
                Constants.StatusKey.PausedKey,
                Constants.StatusKey.CompletedKey,
                Constants.StatusKey.CanceledKey,
                Constants.StatusKey.AssignedKey,
                Constants.StatusKey.RejectedKey,
                Constants.StatusKey.ExpiredKey
            };

            IDictionary<string, string> translationList = translationService.Get(statusKeys, LanguageKey.Default);

            switch (jobStatus)
            {
                case JobStatus.Pending:
                    return translationList[Constants.StatusKey.PendingKey];
                case JobStatus.Opened:
                    return translationList[Constants.StatusKey.OpenedKey];
                case JobStatus.InProgress:
                    return translationList[Constants.StatusKey.InProgressKey];
                case JobStatus.Paused:
                    return translationList[Constants.StatusKey.PausedKey];
                case JobStatus.Completed:
                    return translationList[Constants.StatusKey.CompletedKey];
                case JobStatus.Canceled:
                    return translationList[Constants.StatusKey.CanceledKey];
                case JobStatus.Assigned:
                    return translationList[Constants.StatusKey.AssignedKey];
                case JobStatus.Rejected:
                    return translationList[Constants.StatusKey.RejectedKey];
                case JobStatus.Expired:
                    return translationList[Constants.StatusKey.ExpiredKey];
                default:
                    throw new NotImplementedException($"No such job status {jobStatus}");
            }
        }
    }
}
