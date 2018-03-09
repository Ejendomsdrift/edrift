using System;

namespace Infrastructure.CustomAttributes
{
    public class LocalizationKeyAttribute : Attribute
    {
        public string Key { get; set; }
    }
}
