using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.Models;
using Translations.Interfaces;
using Translations.Models;

namespace Statistics.Core.Profiles
{
    internal static class CsvColumnTranslationKeys
    {
        public const string Id = "CsvColumn_Id";
        public const string Title = "CsvColumn_Title";
        public const string CompletitionDate = "CsvColumn_CompletitionDate";
        public const string CancelingDate = "CsvColumn_CancelingDate";
        public const string RejectionDate = "CsvColumn_RejectionDate";
        public const string Status = "CsvColumn_Status";
        public const string CreatorName = "CsvColumn_CreatorName";
        public const string TenantType = "CsvColumn_TenantType";
        public const string TaskType = "CsvColumn_TaskType";
        public const string SpentTime = "CsvColumn_SpentTime";
        public const string HousingDepartmentName = "CsnColumn_HousingDepartmentName";
        public const string AbsenceReason = "CsvColumn_AbsenceReason";
        public const string Hours = "CsvColumn_Hours";
        public const string Address = "CsvColumn_Address";
        public const string AmountOfVisits = "CsvColumn_AmountOfVisits";
        public const string CancelingReason = "CsvColumn_CancelingReason";
        public const string RejectionReason = "CsvColumn_RejectionReason";
        public const string CategoryName = "CsvColumn_CategoryName";
        public const string OriginalTaskId = "CsvColumn_OriginalTaskId";
    }

    public class SeparationBetweenFacilityTasksVsTenantTasksChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public SeparationBetweenFacilityTasksVsTenantTasksChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.CategoryName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.TaskType,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.CancelingDate,
                    CsvColumnTranslationKeys.CancelingReason,
                    CsvColumnTranslationKeys.OriginalTaskId
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.HousingDepartmentName).Index(2).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.CategoryName).Index(3).Name(translations[CsvColumnTranslationKeys.CategoryName]);
            Map(m => m.Address).Index(4).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.CompletitionDate).Index(5).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.SpentTime).Index(6).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>();
            Map(m => m.StatusName).Index(7).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(8).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TaskType).Index(9).Name(translations[CsvColumnTranslationKeys.TaskType]);
            Map(m => m.TenantType).Index(10).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.CancelingDate).Index(11).Name(translations[CsvColumnTranslationKeys.CancelingDate]);
            Map(m => m.CancelingReason).Index(12).Name(translations[CsvColumnTranslationKeys.CancelingReason]);
            Map(m => m.OriginalTaskId).Index(13).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
        }
    }

    public class SeparationBetweenSpentTimeVsFacilityTasksChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public SeparationBetweenSpentTimeVsFacilityTasksChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.CategoryName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.TaskType,
                    CsvColumnTranslationKeys.CancelingDate,
                    CsvColumnTranslationKeys.CancelingReason,
                    CsvColumnTranslationKeys.OriginalTaskId
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.HousingDepartmentName).Index(2).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.CategoryName).Index(3).Name(translations[CsvColumnTranslationKeys.CategoryName]);
            Map(m => m.Address).Index(4).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.CompletitionDate).Index(5).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.SpentTime).Index(6).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>(); ;
            Map(m => m.StatusName).Index(7).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(8).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TaskType).Index(9).Name(translations[CsvColumnTranslationKeys.TaskType]);
            Map(m => m.CancelingDate).Index(10).Name(translations[CsvColumnTranslationKeys.CancelingDate]);
            Map(m => m.CancelingReason).Index(11).Name(translations[CsvColumnTranslationKeys.CancelingReason]);
            Map(m => m.OriginalTaskId).Index(12).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
            Map(m => m.OriginalTaskId).Index(13).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
        }
    }

    public class FacilityTaskSpentTimeChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public FacilityTaskSpentTimeChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.OriginalTaskId
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.CompletitionDate).Index(2).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.StatusName).Index(3).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(4).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TenantType).Index(5).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.OriginalTaskId).Index(13).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
        }
    }

    public class CompletedToOverdueTaskChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public CompletedToOverdueTaskChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.CategoryName,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.TaskType,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.OriginalTaskId
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.CategoryName).Index(2).Name(translations[CsvColumnTranslationKeys.CategoryName]);
            Map(m => m.TenantType).Index(3).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.HousingDepartmentName).Index(4).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.Address).Index(5).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.CompletitionDate).Index(6).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.SpentTime).Index(7).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>();
            Map(m => m.StatusName).Index(8).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(9).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TaskType).Index(10).Name(translations[CsvColumnTranslationKeys.TaskType]);
            Map(m => m.OriginalTaskId).Index(13).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
        }
    }

    public class TenantTaskSpentTimeChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public TenantTaskSpentTimeChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.CancelingDate,
                    CsvColumnTranslationKeys.CancelingReason
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.HousingDepartmentName).Index(2).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.Address).Index(3).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.CompletitionDate).Index(4).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.SpentTime).Index(5).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>();
            Map(m => m.StatusName).Index(6).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(7).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TenantType).Index(8).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.CancelingDate).Index(9).Name(translations[CsvColumnTranslationKeys.CancelingDate]);
            Map(m => m.CancelingReason).Index(10).Name(translations[CsvColumnTranslationKeys.CancelingReason]);
        }
    }

    public class UnprocessedVsProcessedChartCsvProfile : CsvClassMap<TaskInfo>
    {
        public UnprocessedVsProcessedChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.CategoryName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CompletitionDate,
                    CsvColumnTranslationKeys.CancelingDate,
                    CsvColumnTranslationKeys.CancelingReason,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.CreatorName,          
                    CsvColumnTranslationKeys.OriginalTaskId,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.TaskType
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.HousingDepartmentName).Index(2).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.CategoryName).Index(3).Name(translations[CsvColumnTranslationKeys.CategoryName]);
            Map(m => m.Address).Index(4).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.CompletitionDate).Index(5).Name(translations[CsvColumnTranslationKeys.CompletitionDate]);
            Map(m => m.SpentTime).Index(6).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>();
            Map(m => m.StatusName).Index(7).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(8).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TaskType).Index(9).Name(translations[CsvColumnTranslationKeys.TaskType]);
            Map(m => m.TenantType).Index(10).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.CancelingDate).Index(11).Name(translations[CsvColumnTranslationKeys.CancelingDate]);
            Map(m => m.CancelingReason).Index(12).Name(translations[CsvColumnTranslationKeys.CancelingReason]);
            Map(m => m.OriginalTaskId).Index(13).Name(translations[CsvColumnTranslationKeys.OriginalTaskId]);
        }
    }

    public class AbsencesDataChartCsvProfile : CsvClassMap<AbsenceInfo>
    {
        public AbsencesDataChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.AbsenceReason,
                    CsvColumnTranslationKeys.Hours
                },
                LanguageKey.Default);

            Map(m => m.AbsenceReason).Index(0).Name(translations[CsvColumnTranslationKeys.AbsenceReason]);
            Map(m => m.Hours).Index(1).Name(translations[CsvColumnTranslationKeys.Hours]).TypeConverter<DecimalDelimiterConverter>();
        }
    }

    public class AddressStatisticInfoCsvProfile : CsvClassMap<AddressStatisticInfo>
    {
        public AddressStatisticInfoCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.AmountOfVisits,
                    CsvColumnTranslationKeys.SpentTime
                },
                LanguageKey.Default);

            Map(m => m.HousingDepartmentName).Index(0).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.Address).Index(1).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.Amount).Index(2).Name(translations[CsvColumnTranslationKeys.AmountOfVisits]);
            Map(m => m.SpentTime).Index(3).Name(translations[CsvColumnTranslationKeys.SpentTime]).TypeConverter<DecimalDelimiterConverter>();
            Map(m => m.HousingDepartmentId).Ignore();
        }
    }

    public class RejectionReasonChartCsvProfile : CsvClassMap<RejectionReasonInfo>
    {
        public RejectionReasonChartCsvProfile(ITranslationService translationService)
        {
            var translations = translationService.Get(
                new[]
                {
                    CsvColumnTranslationKeys.Id,
                    CsvColumnTranslationKeys.Title,
                    CsvColumnTranslationKeys.HousingDepartmentName,
                    CsvColumnTranslationKeys.Address,
                    CsvColumnTranslationKeys.CreatorName,
                    CsvColumnTranslationKeys.TenantType,
                    CsvColumnTranslationKeys.RejectionDate,
                    CsvColumnTranslationKeys.RejectionReason,
                    CsvColumnTranslationKeys.SpentTime,
                    CsvColumnTranslationKeys.Status,
                    CsvColumnTranslationKeys.OriginalTaskId
                },
                LanguageKey.Default);

            Map(m => m.Id).Index(0).Name(translations[CsvColumnTranslationKeys.Id]);
            Map(m => m.Title).Index(1).Name(translations[CsvColumnTranslationKeys.Title]);
            Map(m => m.HousingDepartmentName).Index(2).Name(translations[CsvColumnTranslationKeys.HousingDepartmentName]);
            Map(m => m.Address).Index(3).Name(translations[CsvColumnTranslationKeys.Address]);
            Map(m => m.StatusName).Index(4).Name(translations[CsvColumnTranslationKeys.Status]);
            Map(m => m.CreatorName).Index(5).Name(translations[CsvColumnTranslationKeys.CreatorName]);
            Map(m => m.TenantType).Index(6).Name(translations[CsvColumnTranslationKeys.TenantType]);
            Map(m => m.RejectionDate).Index(7).Name(translations[CsvColumnTranslationKeys.RejectionDate]);
            Map(m => m.RejectionReason).Index(8).Name(translations[CsvColumnTranslationKeys.RejectionReason]);
        }
    }

    public class DecimalDelimiterConverter : DefaultTypeConverter
    {
        public override string ConvertToString(TypeConverterOptions options, object value)
        {
            string result = base.ConvertToString(options, value).Replace('.', ',');
            return result;
        }
    }
}