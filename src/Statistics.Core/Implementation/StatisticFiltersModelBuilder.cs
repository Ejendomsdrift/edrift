using System;
using System.Collections.Generic;
using System.Linq;
using CancellingTemplatesCore.Contract.Interfaces;
using CategoryCore.Contract.Interfaces;
using Infrastructure.Constants;
using Infrastructure.Extensions;
using ManagementDepartmentCore.Contract.Interfaces;
using MemberCore.Contract.Interfaces;
using Statistics.Contract.Interfaces;
using Statistics.Contract.Interfaces.Models;
using Statistics.Core.Models;
using YearlyPlanning.Contract.Models;

namespace Statistics.Core.Implementation
{
    public class StatisticFiltersModelBuilder : IStatisticFiltersModelBuilder
    {
        private readonly IMemberService memberService;
        private readonly IManagementDepartmentService managementDepartmentService;
        private readonly ICategoryService categoryService;
        private readonly ICancelingTemplatesService cancelingTemplatesService;

        public StatisticFiltersModelBuilder(
            IMemberService memberService,
            IManagementDepartmentService managementDepartmentService,
            ICategoryService categoryService,
            ICancelingTemplatesService cancelingTemplatesService)
        {
            this.memberService = memberService;
            this.managementDepartmentService = managementDepartmentService;
            this.categoryService = categoryService;
            this.cancelingTemplatesService = cancelingTemplatesService;
        }

        public IStatisticFiltersModel Build()
        {

            var model = new StatisticFiltersModel
            {
                ManagementDepartments = GetManagementDepartments(),
                CategoriesIdsToNamesRelation = CategoriesIdsToNamesRelation(),
                CancelingReasons = GetCancelingReasons()
            };

            return model;
        }

        private IEnumerable<ManagementDepartmentStatisticModel> GetManagementDepartments()
        {
            var currentUser = memberService.GetCurrentUser();
            IEnumerable<IManagementDepartmentModel> managements;
            if (currentUser.IsAdmin())
            {
                managements = managementDepartmentService.GetAllManagements();
            }
            else
            {
                var currentManagementsIds = currentUser.ManagementsToActiveRolesRelation[currentUser.CurrentRole];
                managements = managementDepartmentService.GetManagementDepartmentsByIds(currentManagementsIds);
            }
            var managementViewModels = managements.Map<IEnumerable<ManagementDepartmentStatisticModel>>();
            return managementViewModels.OrderBy(m => m.Name);
        }

        private IDictionary<Guid, string> CategoriesIdsToNamesRelation()
        {
            var categoriesDictionary = categoryService.GetAll().ToDictionary(c => c.Id);
            var result = categoriesDictionary.ToDictionary(
                pair => pair.Key,
                pair => categoryService.GetFullPathString(pair.Value, Constants.Common.CategoryPathDelimeter, categoriesDictionary));
            return result;
        }

        private IEnumerable<IdValueModel<Guid, string>> GetCancelingReasons()
        {
            IEnumerable<ICancelingTemplateModel> models = cancelingTemplatesService.GetByFilter(isCoordinatorReasons: true);
            IEnumerable<IdValueModel<Guid, string>> result = models.Select(x => new IdValueModel<Guid, string>
            {
                Id = x.Id,
                Value = x.Text
            });

            return result;
        } 
    }
}