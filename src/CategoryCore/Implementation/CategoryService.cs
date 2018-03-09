using System;
using System.Collections.Generic;
using System.Linq;
using CategoryCore.Contract.Interfaces;
using CategoryCore.Models;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using MongoRepository.Contract.Interfaces;

namespace CategoryCore.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IAppSettingHelper appSettingHelper;

        public CategoryService(IRepository<Category> categoryRepository, IAppSettingHelper appSettingHelper)
        {
            this.categoryRepository = categoryRepository;
            this.appSettingHelper = appSettingHelper;
        }

        public IEnumerable<ICategoryModel> GetTree()
        {
            var roots = categoryRepository.GetAll()
                .OrderBy(c => c.Name)
                .Select(c => c.Map<CategoryModel>());

            var miscCategoryName = appSettingHelper.GetAppSetting<string>(Constants.AppSetting.MiscCategoryName);
            var tree = BuildTreeAndGetRoots(roots).OrderBy(c => c.Name == miscCategoryName);
            return tree;
        }

        public ICategoryModel Get(Guid id)
        {
            var category = categoryRepository.FindOne(c => c.Id == id);
            var result = category.Map<CategoryModel>();
            return result;
        }

        public IEnumerable<ICategoryModel> GetByIds(IEnumerable<Guid> ids)
        {
            if (!ids.HasValue())
            {
                return Enumerable.Empty<ICategoryModel>();
            }

            var categories = categoryRepository.Find(i => ids.Contains(i.Id));
            var result = categories.Select(c => c.Map<CategoryModel>());
            return result;
        }

        public void Save(ICategoryModel model)
        {
            var category = model.Map<Category>();
            categoryRepository.Save(category);
        }

        public IEnumerable<ICategoryModel> GetAll()
        {
            var result = categoryRepository.Query.ToList();
            var mappedResult = result.Map<IEnumerable<CategoryModel>>();
            return mappedResult;
        }

        public string GetFullPathString(ICategoryModel category, string delimeter, IDictionary<Guid, ICategoryModel> categoriesDictionary)
        {
            var path = GetFlattenPath(LinqExtensions.Enumerable(category), categoriesDictionary);
            var pathString = String.Join(delimeter, path.Select(c => c.Name));
            return pathString;
        }

        #region utils

        private IEnumerable<ICategoryModel> GetFlattenPath(IEnumerable<ICategoryModel> path, IDictionary<Guid, ICategoryModel> categoriesDictionary)
        {
            var pathList = path as IList<ICategoryModel> ?? path.ToList();
            var first = pathList.First();
            var parent = first.ParentId != null ? categoriesDictionary[first.ParentId.Value] : null;
            return parent != null ? GetFlattenPath(LinqExtensions.Enumerable(parent).Concat(pathList), categoriesDictionary) : pathList;
        }

        private static IEnumerable<CategoryModel> BuildTreeAndGetRoots(IEnumerable<CategoryModel> actualObjects)
        {
            var lookup = actualObjects.ToDictionary(x => x.Id, n => n);
            foreach (var item in lookup.Values.Where(v => v.ParentId.HasValue))
            {
                CategoryModel proposedParent;
                if (lookup.TryGetValue(item.ParentId.Value, out proposedParent))
                {
                    item.Parent = proposedParent;
                    proposedParent.Children.Add(item);
                }
            }
            return lookup.Values.Where(x => x.Parent == null);
        }

        #endregion
    }
}
