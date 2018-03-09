using System;

namespace Infrastructure.CustomAttributes
{
    public class EnumSortingAttribute : Attribute
    {
        public int SortIndex { get; set; }
    }
}
