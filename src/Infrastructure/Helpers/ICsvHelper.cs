using System.Collections.Generic;
using CsvHelper.Configuration;

namespace Infrastructure.Helpers
{
    public interface ICsvHelper
    {
        string ToCsv<T>(IEnumerable<T> records, CsvClassMap<T> csvClassMap);
    }
}