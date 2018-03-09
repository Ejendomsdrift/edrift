using System;
using System.Configuration;

namespace CommonUtils.Config
{
    public static class AppSettings
    {
        #region ADFS settings

        public static class ADFS
        {
            public static string Type
            {
                get { return GetStringValue("ADFS.Type", true); }
            }

            public static string Caption
            {
                get { return GetStringValue("ADFS.Caption", true); }
            }

            public static string Realm
            {
                get { return GetStringValue("ADFS.Realm", true); }
            }

            public static string Metadata
            {
                get { return GetStringValue("ADFS.Metadata", true); }
            }

            public static string LoginUrl
            {
                get { return GetStringValue("ADFS.LoginUrl", true); }
            }

            public static string SigninCert
            {
                get { return GetStringValue("ADFS.SigninCert", true); }
            }
            public static string Issuer
            {
                get { return GetStringValue("ADFS.Issuer", true); }
            }
        }

        #endregion

        #region private methods
        private static string GetSetting(string key)
        {
            string result = ConfigurationManager.AppSettings[key];
            return result;
        }

        private static string GetStringValue(string key, bool isRequired)
        {
            string result = GetSetting(key);
            if (result == null && isRequired)
            {
                throw new ArgumentException(string.Format("The key {0} in appSettings is missing!", key));
            }
            return result ?? string.Empty;
        }

        private static bool GetBooleanValue(string key, bool isRequired)
        {
            string value = GetStringValue(key, isRequired);
            if (String.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0 || value.Equals("1"))
            {
                return true;
            }
            if (String.Compare(value, bool.FalseString, StringComparison.OrdinalIgnoreCase) == 0 || value.Equals("0"))
            {
                return false;
            }
            if (isRequired)
            {
                throw new ArgumentException(string.Format("The value from key {0} in appSettings must be boolean value!", key));
            }
            return default(bool);
        }
        #endregion
    }
}
