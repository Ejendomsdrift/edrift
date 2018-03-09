var yearPlanController = function ($scope,
    $state,
    $stateParams,
    $filter,
    departmentProvider,
    yearPlanProvider,
    calendarProvider,
    yearPlanService,
    treeHelper,
    securityService,
    urlService) {

    var emptyCellCount = 2;
    var weeksInYear = 52;
    var WeekChangedBy = Object.freeze({ Administrator: 1, Coordinator: 2 });
    var TaskStatus = Object.freeze({ NotDefined: 0, NotStarted: 1, Finished: 2, Failed: 3 });
    var lazyLoadAdditionalCount = 40;
    var self = this;
    self.state = State;
    self.changedByAdministratorAndCoordinator |= WeekChangedBy.Administrator | WeekChangedBy.Coordinator;
    self.showCategoryTree = $stateParams.isShowCategoryPanel;

    self.categoryTreeConfig = {
        dropdownView: true,
        showViewAllButton: function () { return $state.includes(State.YearPlanTaskOverview); },
        selectOnlyTasks: function () { return $state.includes(State.YearPlanDepartmentOverview); },
        findSelectedNode: function () { },
        onLoaded: onCategoryPickerControlLoaded
    };

    self.categoryTreeWithTasksConfig = {
        dropdownView: true,
        showViewAllButton: function() { return $state.includes(State.YearPlanTaskOverview); },
        showTasks: function() { return $state.includes(State.YearPlanDepartmentOverview); },
        selectOnlyTasks: function() { return $state.includes(State.YearPlanDepartmentOverview); },
        isIncludeHiddenTasks: false,
        isIncludeGroupedTasks: true
    };

    self.changeYear = function (year) {
        var isYearChanged = self.selectedYear != year;
        self.selectedYear = year;
        storeFilter();
        getCalendar(self.selectedYear);

        if (!isYearChanged) {
            return;
        }

        if ($state.includes(State.YearPlanDepartmentOverview)) {
            getAllDepartmentsYearPlan();
        } else if (self.selectedHousingDepartment) {
            getWeeksData(self.selectedHousingDepartment.id);
        }
    }

    self.changeShowDisabled = function () {
        var selectedCategoryId = getCategoryId(self.selectedCategory);
        storeFilter();
        updateCategories(selectedCategoryId);
    }

    self.changeShowAll = function () {
        var selectedCategoryId = getCategoryId(self.selectedCategory);
        storeFilter();
        updateCategories(selectedCategoryId);
    }

    self.getCategoryColor = function () {
        return self.selectedCategory ? self.selectedCategory.color : self.selectedCategoryColor;
    }

    self.editTask = function (taskId) {
        $state.go(State.FacilityTaskGeneral, { id: taskId });
    }

    self.categoryTreeSelected = function (task, category, isCleared) {
        var isItemSelected = false;

        if ($state.includes(State.YearPlanDepartmentOverview)) {
            self.selectedTask = task;
            getAllDepartmentsYearPlan();
            isItemSelected = true;
            self.selectedHousingDepartment = yearPlanService.getSelectedDepartment(self.departments, self.filter);
        }

        if ($state.includes(State.YearPlanTaskOverview)) {
            self.selectedCategory = category;
            self.selectedCategoryId = category != null ? category.id : '';
            isItemSelected = true;
        }

        if (isCleared) {
            self.selectedCategory = null;
            self.filter.category = null;
        }

        storeFilter();

        var selectedCategoryId = getCategoryId(self.selectedCategory);
        updateCategories(selectedCategoryId);

        self.scrollToTop();
        return isItemSelected;
    };

    self.changeDepartment = function (department) {
        self.selectedHousingDepartment = department;
        storeFilter();
        clearDepartmentFilterIfSelectedEmpty();

        if (self.selectedHousingDepartment) {
            if (self.departmentYearPlanAll && self.selectedHousingDepartment) {
                getWeeksData(self.selectedHousingDepartment.id);
            } else if (self.departmentYearPlanAll && !self.selectedHousingDepartment) {
                var selectedCategoryId = getCategoryId(self.selectedCategory);
                setWeeksData([]);
                updateCategories(selectedCategoryId);
            } else {
                getDepartmentYearPlan();
            }
        }
    }

    self.getCssClass = function (week) {
        var cssClass = '';
        var isChangedByAdmin = week.changedBy === WeekChangedBy.Administrator ||
            week.changedBy === self.changedByAdministratorAndCoordinator;

        if (week.status != TaskStatus.NotDefined && isChangedByAdmin) {
            cssClass = '_grey-box';
        }

        return cssClass;
    }

    self.getWeekCssClass = function (week) {
        if (week.isDisabled || week.status == TaskStatus.NotDefined) {
            return '';
        }

        switch (week.status) {
            case TaskStatus.NotStarted:
                return ' _active';
            case TaskStatus.Finished:
                return ' _finished';
            case TaskStatus.Failed:
                return ' _failed';
            default:
                return '';
        }
    }

    self.getHeadersClassNames = function (node) {
        var result = '';

        if (node && node.isTask) {

            if (node.isDisabled) {
                result += '_disabled';
            } else if (node.byCoordinator) {
                result += '_by-coordinator';

                if (!node.isAssigned) {
                    result += ' _not-assigned';
                }
            } else if (!node.isAssigned) {
                result += '_not-assigned';
            } else if (!self.isTaskAssignedToSelectedHousingDepartment(node)) {
                result += '_not-assigned-selected-depatment';
            }
        }

        return result;
    }

    self.filterOperationalJobs = function (node) {
        return node.jobTypeId == JobType.Facility;
    }

    self.loadMoreCategories = function () {
        if (self.filteredCategories.length === self.categories.length) {
            return;
        }

        var start = self.categories.length;
        var hiddenCount = self.filteredCategories.length - self.categories.length;
        var count = hiddenCount >= lazyLoadAdditionalCount ? lazyLoadAdditionalCount : hiddenCount;
        var end = start + count;

        for (var i = start; i < end; i++) {
            var elem = self.filteredCategories[i];
            self.categories.push(elem);
        }
    }

    self.loadMoreDepartments = function () {
        if (self.allDepartmentsYearPlan.length == self.departmentYearPlan.length) {
            return;
        }

        var start = self.departmentYearPlan.length;
        var hiddenCount = self.allDepartmentsYearPlan.length - self.departmentYearPlan.length;
        var count = hiddenCount >= lazyLoadAdditionalCount ? lazyLoadAdditionalCount : hiddenCount;
        var end = start + count;

        for (var i = start; i < end; i++) {
            var elem = self.allDepartmentsYearPlan[i];
            self.departmentYearPlan.push(elem);
        }
    }

    self.getNodeTitle = function (node) {
        if (!node.isGroupedJob || node.address == null) {
            return node.name;
        }

        var address = node.address.length ? node.address : node.addressListForParentTask[self.selectedHousingDepartment.id];
        return address ? node.name + " - " + address : node.name;
    }

    self.isTaskAssignedToSelectedHousingDepartment = function (task) {
        if (isCategory(task)) {
            return true;
        }

        var isTaskAssigned = task.assignedHousingDepartmentIdList.some(function(id) {
            return id == self.selectedHousingDepartment.id;
        });

        return isTaskAssigned;
    }

    angular.element(window).on('scroll', function (e) {
        var tableStickyHeader = $(".js-sticky-thead");
        var headerHeight = $(".js-header").outerHeight();
        var housingDepartment = $(".js-housing-picker");
        var housingHeight = housingDepartment.outerHeight();
        var functionsFilter = $(".js-functions-filter");
        var functionsFilterHeight = functionsFilter.outerHeight();
        var tableHeader = $(".js-table-header");
        var tableHeaderHeight = tableHeader.outerHeight();

        if (tableStickyHeader.length) {

            if (window.pageYOffset >= tableStickyHeader.offset().top - headerHeight) {
                tableStickyHeader.addClass("_sticky");
                if (housingDepartment.hasClass("ng-hide")) {
                    functionsFilter.css('top', headerHeight);
                    tableHeader.css('top', headerHeight + functionsFilterHeight);
                    tableStickyHeader.css('padding-top', tableHeaderHeight + functionsFilterHeight);
                } else {
                    housingDepartment.css('top', headerHeight);
                    functionsFilter.css('top', headerHeight + housingHeight);
                    tableHeader.css('top', headerHeight + housingHeight + functionsFilterHeight);
                    tableStickyHeader.css('padding-top', tableHeaderHeight + housingHeight + functionsFilterHeight);
                }
            }
            else {
                tableStickyHeader.removeClass("_sticky");
                housingDepartment.css('top', '0');
                functionsFilter.css('top', '0');
                tableHeader.css('top', '0');
                tableStickyHeader.css('padding-top', '0');
            }
        }
    });

    self.scrollToTop = function () {
        $("body, html").animate({ scrollTop: 0 }, 400);
    }

    angular.element(window).on("scroll", function (e) {
        $scope.$apply(function () {
            self.isVisibleButton = window.pageYOffset > 300;
        });
    });

    function isCategory(node) {
        return !node.isTask;
    }

    function getAllDepartmentsYearPlan() {
        self.isManagementDepartmentChanged = true;
        self.selectedHousingDepartment = null;

        var model = {
            jobId: self.selectedTask ? self.selectedTask.id : '',
            year: self.selectedYear
        };

        yearPlanProvider.getAllDepartmentsYearPlan(model).then(function (result) {
            self.allDepartmentsYearPlan = $filter('orderBy')(result.data, 'name');
            self.departmentYearPlan = self.allDepartmentsYearPlan.slice(0, lazyLoadAdditionalCount);
        });
    }

    function initDepartmentPicker() {
        self.departmentPickerConfig = new DepartmentPickerModel();
        self.departmentPickerConfig.onSelect = self.changeDepartment;
    }

    function getDepartmentsCallback(result) {
        self.departments = result.data;
        self.selectedHousingDepartment = yearPlanService.getSelectedDepartment(result.data, self.filter);
    }

    function onCategoryPickerControlLoaded(tree) {
        var selectedCategoryId = getCategoryId(self.selectedCategory);
        self.categoryTree = tree;
        if (self.departmentYearPlanAll) {
            updateCategories(selectedCategoryId);
        }
    }

    function getCategoryId(selectedCategory) {
        return selectedCategory ? selectedCategory.id : getCategoryIdFromFilter();
    }

    function getCategoryIdFromFilter() {
        return self.filter && self.filter.category ? self.filter.category.id : null;
    }

    function updateCategories(categoryId) {
        var categoryNode = self.categoryTreeConfig.findSelectedNode(self.categoryTree, categoryId);
        var selectedCategoriesIds = _.map(treeHelper.getSelectedCategoriesList(categoryNode), function (category) { return category.id; });
        var departmentYearPlanItemList = self.departmentYearPlanAll ? self.departmentYearPlanAll : [];
        self.filteredCategories = departmentYearPlanItemList.filter(function (categoryItem) { return isCategoryItemVisible(categoryItem, selectedCategoriesIds); });
        self.categories = self.filteredCategories.slice(0, lazyLoadAdditionalCount);
    }

    function isCategoryItemVisible(item, selectedCategoriesIds) {
        return item.isTask
            ? isTaskVisible(item, selectedCategoriesIds)
            : isCategoryVisible(item, selectedCategoriesIds);
    }

    function isTaskVisible(task, selectedCategoriesIds) {
        if (hasVisibleParentCategory(task, selectedCategoriesIds) && allowedToShow(task)) {
            if (!task.weeks) {
                task.weeks = getVirtualWeeks();
            }

            return true;
        }

        return false;
    }

    function isCategoryVisible(category, selectedCategoriesIds) {
        return selectedCategoriesIds.length === 0 || selectedCategoriesIds.includes(category.id);
    }

    function hasVisibleParentCategory(task, selectedCategoriesIds) {
        return selectedCategoriesIds.length === 0 || selectedCategoriesIds.includes(task.parentCategoryId);
    }

    function allowedToShow(task) {
        var isVisibleForHousingDepartment = isVisibleForDepartment(task, self.selectedHousingDepartment);
        if (task.isDisabled) {
            return self.showDisabled && isVisibleForHousingDepartment;
        } else if (self.showAll) {
            return !task.isGroupedJob || task.isParentGroupedJob || task.isGroupedJob && isVisibleForHousingDepartment;
        } else {
            return isVisibleForHousingDepartment;
        }
    }

    function isVisibleForDepartment(task, department) {
        var result = (!department && !task.isAssigned) || (department && task.assignedHousingDepartmentIdList.includes(department.id) && isRealWeeks(task));
        return result;
    }

    function isRealWeeks(task) {
        if (task.isGroupedJob) { // we should show child grouped jobs without job assigns, if their parent is unassigned from current HD
            return true;
        }

        return task.weeks && typeof (task.weeks[0]) === "object";
    }

    function getVirtualWeeks() {
        var weeks = [];

        for (var i = 0; i < weeksInYear; i++) {
            weeks.push(i);
        }

        return weeks;
    }

    function getWeeksData(housingDepartmentId) {
        self.isActiveWaitIcon = true;
        yearPlanProvider.getYearPlanWeekData(housingDepartmentId, self.selectedYear).then(function (result) {
            setWeeksData(result.data);

            var selectedCategoryId = getCategoryId(self.selectedCategory);
            updateCategories(selectedCategoryId);

            self.isActiveWaitIcon = false;
        });
    }

    function setWeeksData(weekData) {
        if (!self.departmentYearPlanAll) {
            return;
        }

        self.departmentYearPlanAll.forEach(function (item) {
            if (item.isTask) {
                var data = weekData[item.id];
                item.weeks = data ? data : null;
            }
        });
    }

    function clearDepartmentFilterIfSelectedEmpty() {
        if (!self.selectedHousingDepartment) {
            yearPlanService.clearDepartmentFilter();
        }
        else {
            if (!self.showCategoryTree) {
                self.showCategoryTree = true;
            }
        }
    }

    function getCalendar(year) {
        calendarProvider.getCalendar(year).then(function (result) {
            self.monthWeeks = result;
        });
    }

    function getDepartmentYearPlan() {
        var model = {
            housingDepartmentId: self.selectedHousingDepartment ? self.selectedHousingDepartment.id : '',
            year: self.selectedYear
        };

        if (!isYearPlanFilterChanged(model)) {
            return;
        }

        self.yearPlanFilter = model;

        if (self.selectedHousingDepartment) {
            self.isActiveWaitIcon = true;
            yearPlanProvider.getAllData(self.selectedHousingDepartment.id, self.selectedYear).then(function (result) {
                self.departmentYearPlanAll = result.data.yearPlanItems;
                setWeeksData(result.data.weeksData);

                var selectedCategoryId = getCategoryId(self.selectedCategory);
                updateCategories(selectedCategoryId);

                self.isActiveWaitIcon = false;
            });
        } else {
            yearPlanProvider.getDepartmentYearPlan().then(function (result) {
                self.departmentYearPlanAll = result.data.yearPlanItems;

                if (self.categoryTree) {
                    updateCategories();
                }
            });
        }
    }

    function isYearPlanFilterChanged(newFilter) {
        return !self.yearPlanFilter || self.isManagementDepartmentChanged ||
            self.yearPlanFilter.categoryId != newFilter.categoryId ||
            self.yearPlanFilter.housingDepartmentId != newFilter.housingDepartmentId ||
            self.yearPlanFilter.year != newFilter.year ||
            self.yearPlanFilter.showDisabled != newFilter.showDisabled;
    }

    function storeFilter() {
        var filter = {
            selectedYear: self.selectedYear,
            selectedDepartment: self.selectedHousingDepartment,
            selectedTask: self.selectedTask,
            selectedCategory: self.selectedCategory,
            showAll: self.showAll,
            showDisabled: self.showDisabled
        };

        yearPlanService.storeFilter(filter);
    }

    var managementListener = $scope.$on('managementDepartmentChanged', getAllDepartmentsYearPlan);

    $scope.$on('$destroy', function () {
        managementListener();
    });

    function initControl() {

        checkSecurityPermission();

        self.emptyCellWeekRange = _.range(weeksInYear + emptyCellCount);
        self.isManagementDepartmentChanged = false;
        self.filter = yearPlanService.getFilter();
        self.selectedYear = self.filter && self.filter.year ? self.filter.year : null;
        self.showAll = self.filter.showAll;
        self.showDisabled = self.filter.showDisabled;

        getAllDepartmentsYearPlan();

        calendarProvider.getTotalWeeks().then(function (result) {
            self.totalWeeks = result.totalWeeks;
        });

        initDepartmentPicker();
        departmentProvider.getDepartments().then(getDepartmentsCallback);

        self.yearWeekSelectorConfig = {
            isShowYear: true,
            selectedYear: self.selectedYear,
            onChange: self.changeYear
        }

        var selectedCategory = self.filter.category;
        if (selectedCategory) {
            self.selectedCategoryId = selectedCategory.id;
            self.selectedCategoryColor = selectedCategory.color;
        }
    }

    function checkSecurityPermission() {
        securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.YearPlanPage] }).then(function (result) {
            if (!result.data[ControlSecurityKey.YearPlanPage]) {
                urlService.defaultRedirect();
            }
        });
    }

    initControl();
}

angular.module("boligdrift").controller('yearPlanController',
    [
        '$scope',
        '$state',
        '$stateParams',
        '$filter',
        'departmentProvider',
        'yearPlanProvider',
        'calendarProvider',
        'yearPlanService',
        'treeHelper',
        'securityService',
        'urlService',
        yearPlanController
    ]);