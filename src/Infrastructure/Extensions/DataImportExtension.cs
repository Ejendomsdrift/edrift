using System;
using System.Data;

namespace Infrastructure.Extensions
{
    public static class DataImportExtension
    {
        public static T GetSafe<T>(this DataRow row, string fieldName)
        {
            return row[fieldName] == DBNull.Value ? default(T) : (T)row[fieldName];
        }
    }
}
