(function () {
    var yearPlanService = function (localStorageService) {
        var self = this;

        self.getSelectedDepartment = function (departments, filter) {
            if (filter.departmentid) {
                for (var i = 0; i < departments.length; i++) {
                    if (departments[i].id === filter.departmentid) {
                        return departments[i];
                    }
                }
            }
            return null;
        };

        self.storeFilter = function (filter) {
            var yearPlanFilter = localStorageService.get("yearPlan") || {};
            yearPlanFilter.showAll = filter.showAll;
            yearPlanFilter.showDisabled = filter.showDisabled;
            yearPlanFilter.year = filter.selectedYear ? filter.selectedYear : null;
            yearPlanFilter.departmentid = filter.selectedDepartment ? filter.selectedDepartment.id : null;
            yearPlanFilter.task = filter.selectedTask ? filter.selectedTask.id : null;
            yearPlanFilter.category = filter.selectedCategory ?
                                      { id: filter.selectedCategory.id, color: filter.selectedCategory.color } :
                                      null;

            localStorageService.set("yearPlan", yearPlanFilter);
        };

        self.clearCategoryFilter = function () {
            var yearPlanFilter = localStorageService.get("yearPlan") || {};
            yearPlanFilter.category = null;
            localStorageService.set("yearPlan", yearPlanFilter);
        };

        self.clearDepartmentFilter = function () {
            var yearPlanFilter = localStorageService.get("yearPlan") || {};
            yearPlanFilter.departmentid = null;
            localStorageService.set("yearPlan", yearPlanFilter);
        };

        self.getFilter = function () {
            return localStorageService.get("yearPlan") || {};
        }
    };

    angular.module('boligdrift').service('yearPlanService', ['localStorageService', 'calendarProvider', yearPlanService]);

})();