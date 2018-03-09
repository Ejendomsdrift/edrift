using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string src)
        {
            return string.IsNullOrEmpty(src);
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static T ParseToEnum<T>(this string str)
        {
            return (T) Enum.Parse(typeof(T), str, true);
        }

        public static IEnumerable<string> SplitByComma(this string str)
        {
            return str.SplitBySeparator(",");
        }

        public static IEnumerable<string> SplitBySeparator(this string str, string separator)
        {
            return str.IsNullOrEmpty()
                ? Enumerable.Empty<string>()
                : str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string JoinWithComma(this IEnumerable<string> list)
        {
            return list.JoinWithSeparator(",");
        }

        public static string JoinNonEmptyStrings(this IEnumerable<string> enumerable, string separator)
        {
            return enumerable.Where(x => !x.IsNullOrEmpty()).JoinWithSeparator(separator);
        }

        public static string JoinWithSeparator(this IEnumerable<string> list, string separator)
        {
            return list == null ? string.Empty : string.Join(separator, list);
        }

        public static bool IsNotNullOrEmpty(this string src)
        {
            return !string.IsNullOrEmpty(src);
        }

        public static bool IsNotNullOrEmptyOrWhitespace(this string src)
        {
            return !string.IsNullOrEmpty(src) && !string.IsNullOrWhiteSpace(src);
        }

        public static string EncodeQuotes(this string src)
        {
            return string.IsNullOrWhiteSpace(src) ? string.Empty : src.Replace("'", "&#8217;").Replace("\"", "&#8221;");
        }

        public static string GetFileExtention(this string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public static string GetFileNameFromPath(this string path)
        {
            if (!path.IsNotNullOrEmptyOrWhitespace())
            {
                return path;
            }

            var pathParts = path.SplitBySeparator("/");
            return pathParts.LastOrDefault();
        }

        public static string HtmlEncodeSpecialCharacters(this string text)
        {
            if (text != null)
            {
                var sb = new StringBuilder(text.Length);
                foreach (var t in text)
                {
                    switch (t)
                    {
                        case '<':
                            sb.Append("&lt;");
                            break;
                        case '>':
                            sb.Append("&gt;");
                            break;
                        case '"':
                            sb.Append("&quot;");
                            break;
                        case '&':
                            sb.Append("&amp;");
                            break;
                        default:
                            sb.Append(t);
                            break;
                    }
                }
                return sb.ToString();
            }
            return string.Empty;
        }

        public static string Format(this string src, params object[] args)
        {
            return string.Format(src, args);
        }

        public static bool Compare(this string src, string src2)
        {
            return !src.HasValue() && !src2.HasValue() ||
                   (src.HasValue() && src.Trim().Equals(src2?.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        public static IEnumerable<T> GetCollection<T>(this string str, string separator)
        {
            if (!str.HasValue())
            {
                return Enumerable.Empty<T>();
            }

            return str.SplitBySeparator(separator).Select(i => typeof(T).IsEnum
                ? ParseToEnum<T>(i)
                : (T) Convert.ChangeType(i, typeof(T)));
        }

        public static bool ToBool(this string str, bool defaultValue = false)
        {
            bool result;
            return bool.TryParse(str, out result) ? result : defaultValue;
        }

        public static Guid ToGuid(this string str, Guid defaultValue = default(Guid))
        {
            Guid result;
            return Guid.TryParse(str, out result) ? result : defaultValue;
        }

        public static Guid? ToNullableGuid(this string str)
        {
            Guid result;
            if (Guid.TryParse(str, out result))
                return result;
            return null;
        }
    }
}