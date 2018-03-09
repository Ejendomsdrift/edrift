using System;
using System.Configuration;

namespace MemberCore.Authentication.Configurations
{
    public static class AppSettings
    {
        #region ADFS settings
        public static class ADFS
        {
            public static string Realm
            {
                get { return GetStringValue("ADFS.Realm", true); }
            }

            public static string Metadata
            {
                get { return GetStringValue("ADFS.Metadata", true); }
            }
        }

        #endregion

        #region Custom settings

        public static class CustomSettings
        {
            public static bool IsADFSLogin
            {
                get { return GetBooleanValue("IsADFSLogin", true); }
            }

            public static string CustomLoginUrl
            {
                get { return GetStringValue("CustomLoginUrl", true); }
            }

            public static string AnonymousAccessToken
            {
                get { return GetStringValue("anonymousAccessToken", true); }
            }

            public static string DefaultUserName
            {
                get { return GetStringValue("defaultUserName", true); }
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
