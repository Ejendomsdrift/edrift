(function () {
    var generalController = function ($stateParams,
        $state,
        $filter,
        $rootScope,
        categoryProvider,
        jobProvider,
        securityService,
        memberProvider,
        yearPlanService,
        weekPlanService,
        urlService) {

        var self = this;

        self.hideTask = function () {
            var model = { id: self.facilityTask.id, value: self.facilityTask.isHidden };
            jobProvider.Hide(model).then(function() {
                self.isAllowedTaskGrouping = !self.facilityTask.isHidden;
            });
        }

        self.showDepartmentSelector = function () {
            var result = self.isAllowedTaskGrouping &&
                self.facilityTask &&
                self.isVisibleDepartmentSelector &&
                !hasSingleDepartment();

            return result;
        }

        self.addTaskToRelationGroup = function () {
            if (!self.isVisibleDepartmentSelector && !hasSingleDepartment() && !isChildTask()) {
                self.isVisibleDepartmentSelector = true;
                return;
            }

            var housingDepartmentId = "";
            var isDepartmentValid = self.departmentPickerConfig.triggerValidate() || hasSingleDepartment() || isChildTask();

            if (!self.facilityTask.relationGroupList.length && isDepartmentValid && showConfirm()) {
                housingDepartmentId = hasSingleDepartment() ? self.assignedHousingDepartmentsIdList[0] : self.department.id;
                jobProvider.CreateTaskRelationGroup(self.facilityTask.id, housingDepartmentId).then(function (result) {
                    redirectToGroupedJob(result.data, housingDepartmentId);
                });
            }
            else if (self.facilityTask.relationGroupList.length && isDepartmentValid && showConfirm()) {
                housingDepartmentId = self.department ? self.department.id : self.filter.departmentid;
                jobProvider.AddTaskToRelationGroup(self.facilityTask.id, housingDepartmentId).then(function (result) { 
                    redirectToGroupedJob(result.data, housingDepartmentId);
                });
            }
        }

        function hasSingleDepartment() {
            return self.assignedHousingDepartmentsIdList.length == 1;
        }

        function redirectToGroupedJob(jobId, housingDepartmentId) {
            $state.go(State.FacilityTaskLocation, { id: jobId, department: housingDepartmentId});
        }

        function showConfirm() {
            var msg = $filter('translate')('Are you sure?');
            return confirm(msg);
        }

        function isAllowTaskGrouping() {
            self.isAllowedTaskGrouping = false;
            if ($stateParams.redirectState !== State.YearPlanTaskOverview) {
                return;
            }

            jobProvider.IsAllowedTaskGrouping($stateParams.id).then(function (result) {
                self.isAllowedTaskGrouping = result.data;
            });
        }

        function loadTask() {
            jobProvider.GetFacilityTask($stateParams.id, self.filter.departmentid).then(function (result) {
                var isAllAssignedHousingDepartmentAreGrouped = result.data.isAllAssignedHousingDepartmentAreGrouped;
                self.facilityTask = result.data.job;
                self.assignedHousingDepartmentsIdList = result.data.assignedHousingDepartmentsIdList;
                self.hasAssignedHousingDepartments = self.assignedHousingDepartmentsIdList && self.assignedHousingDepartmentsIdList.length > 0;
                self.isAllChildGroupedTaskHided = result.data.isAllChildGroupedTaskHided;
                self.isPossibleToHideChildTask = result.data.isPossibleToHideChildTask;

                setDirectives(result.data.job);

                jobProvider.IsGroupedTask($stateParams.id).then(function (result) {
                    showHideTaskControl(result.data);
                    if (result.data && !isChildTask()) {
                        self.showGroupingButton = !isAllAssignedHousingDepartmentAreGrouped;
                    }
                });
            });
        }

        function showHideTaskControl(isGroupedTask) {
            if (self.currentUser) {
                self.showHideTaskControl = isTaskBelongsToSelectedHousingDepartment() && isValidRole(isGroupedTask);
            }
        }

        function isTaskBelongsToSelectedHousingDepartment() {
            if (!self.filter) {
                return false;
            }

            var foundedDepartmentId = self.assignedHousingDepartmentsIdList.filter(function (housingDepartmentId) {
                return self.filter.departmentid == housingDepartmentId;
            });

            return foundedDepartmentId.length > 0;
        }

        function isValidRole(isGroupedTask) {
            return isGroupedTask ? isPossibleToHideGroupedTask() : true;
        }

        function isPossibleToHideGroupedTask() {
            return isChildTask() && self.isPossibleToHideChildTask || self.isAllChildGroupedTaskHided;
        }

        function isChildTask() {
            return self.facilityTask.parentId != null;
        }

        function changeCategory(categoryId) {
            var model = { id: self.facilityTask.id, value: categoryId };
            jobProvider.ChangeCategory(model);
        };

        function changeTitle(title) {
            var model = { id: self.facilityTask.id, value: title };
            jobProvider.ChangeTitle(model).then(function() {
                $rootScope.$broadcast('facilityTaskTitleChanged');
            });
        }

        function setDirectives(data) {
            self.titleTextControlConfig.value = data.title;
            self.titleTextControlConfig.triggerRefresh();

            self.categoryTreePickerConfig.value = data.categoryId;
            self.categoryTreePickerConfig.triggerRefresh();
        }

        function manageSecurityPermissions(securityResult) {
            var mode = securityResult[self.titleTextControlConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.titleTextControlConfig.mode = mode;
            self.titleTextControlConfig.triggerRefresh();
            mode = securityResult[self.categoryTreePickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.categoryTreePickerConfig.mode = mode;
            self.categoryTreePickerConfig.triggerRefresh();
            self.isHideTaskCheckboxDisabled = !securityResult[ControlSecurityKey.GeneralHideTask];
            self.showGroupingButton = securityResult[ControlSecurityKey.GeneralGroupTask];
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.General,
                jobId: $stateParams.id,
                page: page,
                dayAssignId: $stateParams.dayAssignId
            };

            securityService.hasAccessByGroupName(model).then(function (result) {
                manageSecurityPermissions(result.data);
            });
        }

        function getPreviousState() {
            if ($stateParams.redirectState.indexOf(Pages.YearPlan) != -1) {
                return Pages.YearPlan;
            } else if ($stateParams.redirectState.indexOf(Pages.WeekPlan) != -1) {
                return Pages.WeekPlan;
            }
        }

        function updateHousingDepartmentHandler(housingDepartment) {
            self.department = housingDepartment;
        }

        function initDirectives() {
            self.isHideTaskCheckboxDisabled = false;
            self.showGroupingButton = false;

            self.titleTextControlConfig = new TextControlModel();
            self.titleTextControlConfig.onSave = changeTitle;
            self.titleTextControlConfig.securityKey = ControlSecurityKey.GeneralTitle;

            self.categoryTreePickerConfig = new CategoryTreePickerModel();
            self.categoryTreePickerConfig.categoryTreeConfig = { simpleView: true };
            self.categoryTreePickerConfig.onSave = changeCategory;
            self.categoryTreePickerConfig.securityKey = ControlSecurityKey.GeneralCategory;

            self.departmentPickerConfig = new DepartmentPickerModel();
            self.departmentPickerConfig.onSelect = updateHousingDepartmentHandler;
            self.departmentPickerConfig.jobId = $stateParams.id;
        }

        function initControl() {

            checkSecurityPermission();

            initDirectives();

            if (getPreviousState() == Pages.YearPlan) {
                self.filter = yearPlanService.getFilter();
            } else {
                self.filter = weekPlanService.getFilter();
            }

            memberProvider.GetCurrentUserContext().then(function(result) {
                self.currentUser = result.data.memberModel;

                initSecurityRules();
                loadTask();
                isAllowTaskGrouping();
            });
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.FacilityTaskEditPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.FacilityTaskEditPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    }
    angular.module('boligdrift').controller('generalController',
        [
            '$stateParams',
            '$state',
            '$filter',
            '$rootScope',
            'categoryProvider',
            'jobProvider',
            'securityService',
            'memberProvider',
            'yearPlanService',
            'weekPlanService',
            'urlService',
            generalController
        ]);
})();