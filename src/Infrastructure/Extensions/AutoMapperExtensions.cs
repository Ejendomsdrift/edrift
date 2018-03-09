using AutoMapper;

namespace Infrastructure.Extensions
{
    public static class AutoMapperExtensions
    {
        public static TDestination Map<TDestination>(this object obj)
        {
            return Mapper.Map<TDestination>(obj);
        }

        public static TDestination Map<TSource, TDestination>(this TDestination dstObject, TSource sourceObject)
        {
            return Mapper.Map(sourceObject, dstObject);
        }
    }
}