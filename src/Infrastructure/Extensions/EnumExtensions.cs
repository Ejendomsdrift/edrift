using Infrastructure.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        public static int GetSortIndex(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<EnumSortingAttribute>()
                            .SortIndex;
        }

        public static string GetLocalizationKey(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<LocalizationKeyAttribute>()
                            .Key;
        }

        public static Dictionary<TEnum, string> GetAllLocalizationKeys<TEnum>() where TEnum : struct
        {
            var keys = GetValues<TEnum>().ToDictionary(k => k, v => GetLocalizationKey(v as Enum));
            return keys;
        }

        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
    }
}
