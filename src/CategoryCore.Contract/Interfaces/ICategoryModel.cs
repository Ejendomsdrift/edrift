using System;
using System.Collections.Generic;

namespace CategoryCore.Contract.Interfaces
{
    public interface ICategoryModel
    {
        Guid Id { get; }
        Guid? ParentId { get; }
        string Name { get; }
        string Color { get; }
        bool Visible { get; }
        ICategoryModel Parent { get; }
        List<ICategoryModel> Children { get; }
    }
}
