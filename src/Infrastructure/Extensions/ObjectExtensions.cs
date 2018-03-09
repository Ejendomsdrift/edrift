using System.Linq;

namespace Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static bool In<T>(this T value, params T[] items)
        {
            if (items == null || value == null)
            {
                return false;
            }
            return items.Contains(value);
        }
    }
}
