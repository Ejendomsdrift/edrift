using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetFirstInheritedClass(this Type type)
        {
            var typeClass = type.GetInheritedClasses().FirstOrDefault();
            return typeClass ?? type;
        }

        public static IEnumerable<Type> GetInheritedClasses(this Type type)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && type.IsAssignableFrom(t));
            return types;
        }
    }
}