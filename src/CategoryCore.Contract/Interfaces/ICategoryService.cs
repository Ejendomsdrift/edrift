using System;
using System.Collections.Generic;

namespace CategoryCore.Contract.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<ICategoryModel> GetTree();
        ICategoryModel Get(Guid id);
        IEnumerable<ICategoryModel> GetByIds(IEnumerable<Guid> ids);
        void Save(ICategoryModel model);
        IEnumerable<ICategoryModel> GetAll();
        string GetFullPathString(ICategoryModel category, string delimeter, IDictionary<Guid, ICategoryModel> categoriesDictionary);
    }
}
