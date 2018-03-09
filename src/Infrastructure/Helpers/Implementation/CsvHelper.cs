using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Helpers.Implementation
{
    public class CsvHelper : ICsvHelper
    {
        private readonly IAppSettingHelper appSettingHelper;

        public CsvHelper(IAppSettingHelper appSettingHelper)
        {
            this.appSettingHelper = appSettingHelper;
        }

        public string ToCsv<T>(IEnumerable<T> records, CsvClassMap<T> csvClassMap)
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.Delimiter = appSettingHelper.GetAppSetting<string>(Constants.Constants.AppSetting.CsvSeparator);
                csv.Configuration.Encoding = Encoding.Unicode;
                csv.Configuration.RegisterClassMap(csvClassMap);
                csv.Configuration.Quote = '"';
                csv.WriteRecords(records);
                var result = writer.ToString();
                return result;
            }
        }
    }
}