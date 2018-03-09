(function () {
    var intervalController = function ($filter, $state, $stateParams, jobProvider, securityService, memberProvider, yearPlanService, weekPlanService, urlService) {
        var self = this;

        var FlagsRoleEnum = Object.freeze({ Administrator: 1, Coordinator: 2, Janitor: 4, SuperAdmin: 8 });
        var defaultIntValue = 0;
        self.changedByAdministratorAndCoordinator = 3;
        self.departmentNamePlaceholder = $filter('translate')('Choose department');

        function loadAssigns() {
            jobProvider.GetJobAssigns($stateParams.id).then(function (result) {
                self.currentRole = getFlagsCurrentRole(result.data.currentUser.currentRole);
                self.jobId = $stateParams.id;
                self.assigns = result.data.assigns;
                self.globalAssign = result.data.globalAssign;
                self.assigns.push(self.globalAssign);
                self.assignedDepartments = result.data.assignedDepartments;
                self.addressList = result.data.addressList;
                self.isGroupedJob = result.data.isGroupedJob;
                self.departmentName = self.departmentNamePlaceholder;
                self.globalTillYearSelected = self.globalAssign.tillYear !== defaultIntValue;
                setDirectives();
                if (self.localDepartment) {
                    refreshLocalDepartment();
                }
            });
        }

        self.localDepartmentOnChange = function (department) {
            if (!department) {
                return;
            }

            self.localDepartment = self.assigns.find(function (assign) {
                return assign.housingDepartmentIdList.indexOf(department.id) !== -1 && !assign.isGlobal;
            });

            var departmentName = department.syncDepartmentId + ' ' + department.name;
            localDataOnChange(department.id, departmentName);
        }

        self.localJobAddressOnChange = function () {
            if (!self.localJobAddress) {
                self.localDepartment = null;
                return;
            }

            self.localDepartment = self.assigns.find(function (assign) {
                return assign.jobIdList.indexOf(self.localJobAddress.id) !== -1 && !assign.isGlobal;
            });

            var departmentId = getDepartmentId();
            localDataOnChange(departmentId, null);
        }

        self.isGlobalIntervalLocked = function () {
            if (self.globalAssign) {
                return self.globalAssign.isLocked;
            }
        }

        self.isLocalIntervalLocked = function () {
            if (self.localDepartment && !self.localDepartment.isGlobal) {
                return self.localDepartment.isLocked;
            } else if (self.globalAssign) {
                return self.globalAssign.isLocked;
            }
        }

        self.lockGlobalInterval = function () {
            var model = {
                jobId: self.jobId,
                assignId: self.globalAssign.id,
                departmentId: self.globalAssign.departmentId,
                changedByRole: getChangedByRoleValue(self.globalAssign.changedByRole),
                isLocked: self.globalAssign.isLocked
            };

            jobProvider.LockInterval(model);
        }

        self.lockLocalInterval = function () {
            var localDepartmentName = self.localDepartment.name;

            var model = {
                jobId: self.jobId,
                assignId: self.localDepartment.id,
                departmentId: self.localDepartment.departmentId,
                changedByRole: getChangedByRoleValue(self.localDepartment.changedByRole),
                isLocked: self.localDepartment.isLocked
            };

            jobProvider.LockInterval(model).then(function (result) {
                if (!self.localDepartment.id) {
                    self.assigns.unshift(result.data);
                }
                self.localDepartment = result.data;
                self.localDepartment.name = localDepartmentName;
            });
        }

        self.isAdministration = function () {
            if (self.currentUser) {
                return self.currentUser.currentRole == Role.Administrator || self.currentUser.currentRole == Role.SuperAdmin;
            }
        }

        self.isDaySelected = function (target) {
            if (target && target.dayPerWeekList) {
                return target.dayPerWeekList.length > 0;
            }
        }

        self.globalTillYearChecked = function () {
            var date = new Date();
            if (!self.globalTillYearSelected && self.globalAssign.tillYear > 0) {
                date = null;
            }
            saveGlobalYear(date);
        }

        self.localTillYearChecked = function () {
            var date = new Date();
            if (!self.localTillYearSelected && self.localDepartment.tillYear > 0) {
                date = null;
            }
            saveLocalYear(date);
        }

        function localDataOnChange(departmentId, departmentName) {
            if (!self.localDepartment) {
                self.localDepartment = angular.copy(self.globalAssign);
            }

            if (self.localDepartment && self.localDepartment.isGlobal) {
                self.localDepartment.id = '';
                self.localDepartment.weekList = getLocalWeekList(self.globalAssign.weekList);
            }

            self.localDepartment.departmentId = departmentId;
            self.localDepartment.name = departmentName;
            self.localTillYearSelected = self.localDepartment.tillYear !== defaultIntValue;

            setLocalWeeksPickerModel();
            setLocalJobShedulePickerModel();
            setLocalDatePickerModel();
        }

        function initSecurityRules() {
            self.disableGlobalCheckboxes = true;
            self.disableLocalCheckboxes = true;

            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.Interval,
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

        function manageSecurityPermissions(securityResult) {
            manageGlobalSecurityPermission(securityResult);
            manageLocalSecurityPermission(securityResult);

            if (self.isAdministration()) { 
                // we need provide all permissions to admin for editing local sections, for grouped tasks which were created by coordinator 
                setPermissionsForAdmin();
            }
        }

        function manageGlobalSecurityPermission(securityResult) {
            var mode = securityResult[self.globalWeeksPickerModel.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalWeeksPickerModel.mode = mode;
            self.globalWeeksPickerModel.triggerRefresh();

            mode = securityResult[self.globalJobShedulePickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalJobShedulePickerConfig.mode = mode;
            self.globalJobShedulePickerConfig.triggerRefresh();

            mode = securityResult[self.globalDatePickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalDatePickerConfig.mode = mode;
            self.globalDatePickerConfig.triggerRefresh();

            self.disableGlobalCheckboxes = !securityResult[ControlSecurityKey.IntervalGlobalDate];
        }

        function manageLocalSecurityPermission(securityResult) {
            var mode = securityResult[self.localWeeksPickerModel.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localWeeksPickerModel.mode = mode;
            self.localWeeksPickerModel.triggerRefresh();

            mode = securityResult[self.localJobShedulePickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localJobShedulePickerConfig.mode = mode;
            self.localJobShedulePickerConfig.triggerRefresh();

            mode = securityResult[self.localDatePickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localDatePickerConfig.mode = mode;
            self.localDatePickerConfig.triggerRefresh();

            self.disableLocalCheckboxes = !securityResult[ControlSecurityKey.IntervalLocalDate];
        }

        function setPermissionsForAdmin() {
            self.localWeeksPickerModel.mode = ControlMode.view;
            self.localWeeksPickerModel.triggerRefresh();

            self.localJobShedulePickerConfig.mode = ControlMode.view;
            self.localJobShedulePickerConfig.triggerRefresh();

            self.localDatePickerConfig.mode = ControlMode.view;
            self.localDatePickerConfig.triggerRefresh();

            self.disableLocalCheckboxes = false;
        }

        function initDirectives() {
            initGlobalWeeksPickerModel();
            initLocalWeeksPickerModel();
            initGlobalJobShedulePickerModel();
            initLocalJobShedulePickerModel();
            initGlobalDatePickerModel();
            initLocalDatePickerModel();
        }

        function initGlobalWeeksPickerModel() {
            self.globalWeeksPickerModel = new WeeksPickerModel();

            self.globalWeeksPickerModel.onSave = saveGlobalWeekList;
            self.globalWeeksPickerModel.securityKey = ControlSecurityKey.IntervalGlobalWeeks;
        }

        function initLocalWeeksPickerModel() {
            self.localWeeksPickerModel = new WeeksPickerModel();

            self.localWeeksPickerModel.onSave = saveLocalWeekList;
            self.localWeeksPickerModel.securityKey = ControlSecurityKey.IntervalLocalWeeks;
        }

        function initGlobalJobShedulePickerModel() {
            self.globalJobShedulePickerConfig = new PerWeekJobSchedulePickerModel();
            self.globalJobShedulePickerConfig.onSave = saveGlobalJobShedule;
            self.globalJobShedulePickerConfig.securityKey = ControlSecurityKey.IntervalGlobalShedule;
        }

        function initLocalJobShedulePickerModel() {
            self.localJobShedulePickerConfig = new PerWeekJobSchedulePickerModel();
            self.localJobShedulePickerConfig.onSave = saveLocalJobShedule;
            self.localJobShedulePickerConfig.securityKey = ControlSecurityKey.IntervalLocalShedule;
        }

        function initGlobalDatePickerModel() {
            self.globalDatePickerConfig = new DatePickerModel();
            self.globalDatePickerConfig.date = new Date().getFullYear();
            self.globalDatePickerConfig.view = DatePickerViewType.years;
            self.globalDatePickerConfig.format = DatePickerFormatType.year;
            self.globalDatePickerConfig.onSave = saveGlobalYear;
            self.globalDatePickerConfig.securityKey = ControlSecurityKey.IntervalGlobalDate;
        }

        function initLocalDatePickerModel() {
            self.localDatePickerConfig = new DatePickerModel();
            self.localDatePickerConfig.date = new Date().getFullYear();
            self.localDatePickerConfig.view = DatePickerViewType.years;
            self.localDatePickerConfig.format = DatePickerFormatType.year;
            self.localDatePickerConfig.onSave = saveLocalYear;
            self.localDatePickerConfig.securityKey = ControlSecurityKey.IntervalLocalDate;
        }

        function setDirectives() {
            setGlobalWeeksPickerModel();
            setGlobalJobShedulePickerModel();
            setGlobalDatePickerModel();
        }

        function setGlobalWeeksPickerModel() {
            self.globalWeeksPickerModel.value = self.globalAssign.weekList;
            self.globalWeeksPickerModel.currentRole = self.currentRole;
            self.globalWeeksPickerModel.triggerRefresh();
        }

        function setLocalWeeksPickerModel() {
            self.localWeeksPickerModel.triggerClearWeeks();
            self.localWeeksPickerModel.currentRole = self.currentRole;
            self.localWeeksPickerModel.value = self.localDepartment.weekList;
            self.localWeeksPickerModel.triggerRefresh();
            if (!self.isAdministration() && $stateParams.redirectState.indexOf(Pages.YearPlan) != -1) {
                self.localWeeksPickerModel.triggerLockControl(self.isLocalIntervalLocked());
            }
        }

        function setGlobalJobShedulePickerModel() {
            self.globalJobShedulePickerConfig.dayPerWeekList = self.globalAssign.dayPerWeekList;
            self.globalJobShedulePickerConfig.repeatsPerWeek = self.globalAssign.repeatsPerWeek;
            self.globalJobShedulePickerConfig.triggerRefresh();
        }

        function setLocalJobShedulePickerModel() {
            self.localJobShedulePickerConfig.dayPerWeekList = self.localDepartment.dayPerWeekList;
            self.localJobShedulePickerConfig.repeatsPerWeek = self.localDepartment.repeatsPerWeek;
            self.localJobShedulePickerConfig.triggerRefresh();
        }

        function setGlobalDatePickerModel() {
            self.globalDatePickerConfig.date = self.globalTillYearSelected ? self.globalAssign.tillYear : new Date().getFullYear();
            self.globalDatePickerConfig.triggerRefresh();
        }

        function setLocalDatePickerModel() {
            self.localDatePickerConfig.date = self.localTillYearSelected ? self.localDepartment.tillYear : new Date().getFullYear();
            self.localDatePickerConfig.triggerRefresh();
        }

        function saveGlobalWeekList(weekList) {
            self.globalAssign = self.globalAssign ? self.globalAssign : {};
            self.globalAssign.weekList = weekList;

            var model = {
                jobId: self.jobId,
                assignId: self.globalAssign.id,
                departmentId: self.globalAssign.departmentId,
                changedByRole: getChangedByRoleValue(self.globalAssign.changedByRole),
                weekList: self.globalAssign.weekList
            };

            jobProvider.saveWeekList(model).then(function () {
                self.localDepartment = null;
                loadAssigns();
            });

            if (self.localDepartment && self.localDepartment.isGlobal) {
                self.localWeeksPickerModel.triggerClearWeeks();
                self.localWeeksPickerModel.value = getLocalWeekList(self.globalAssign.weekList);
                self.localWeeksPickerModel.triggerRefresh();
            }
        }

        function saveLocalWeekList(weekList) {
            var localDepartmentName = self.localDepartment.name;
            self.localDepartment.weekList = weekList;
            self.localWeeksPickerModel.triggerClearWeeks();
            self.localWeeksPickerModel.value = self.localDepartment.weekList;
            self.localWeeksPickerModel.triggerRefresh();

            var model = {
                jobId: self.isGroupedJob ? self.localJobAddress.id : self.jobId,
                assignId: self.localDepartment.id,
                departmentId: getDepartmentId(),
                changedByRole: getChangedByRoleValue(self.localDepartment.changedByRole),
                weekList: self.localDepartment.weekList
            };

            jobProvider.saveWeekList(model).then(function (result) {
                if (!self.localDepartment.id) {
                    self.assigns.unshift(result.data);
                }
                self.localDepartment = result.data;
                self.localDepartment.name = localDepartmentName;
            });
        }

        function saveGlobalJobShedule(dayPerWeekList, repeatsPerWeek) {
            self.globalAssign.dayPerWeekList = dayPerWeekList;
            self.globalAssign.repeatsPerWeek = repeatsPerWeek;

            var model = {
                jobId: self.jobId,
                assignId: self.globalAssign.id,
                departmentId: '',
                changedByRole: getChangedByRoleValue(self.globalAssign.changedByRole),
                repeatsPerWeek: self.globalAssign.repeatsPerWeek,
                dayPerWeekList: self.globalAssign.dayPerWeekList
            }

            jobProvider.SaveJobShedule(model).then(function () {
                self.localDepartment = null;
                loadAssigns();
            });

            setGlobalJobShedulePickerModel();

            if (self.localDepartment && self.localDepartment.isGlobal) {
                self.localJobShedulePickerConfig.dayPerWeekList = angular.copy(dayPerWeekList);
                self.localJobShedulePickerConfig.repeatsPerWeek = repeatsPerWeek;
                self.localJobShedulePickerConfig.triggerRefresh();
            }
        }

        function saveLocalJobShedule(dayPerWeekList, repeatsPerWeek) {
            var localDepartmentName = self.localDepartment.name;
            self.localDepartment.dayPerWeekList = dayPerWeekList;
            self.localDepartment.repeatsPerWeek = repeatsPerWeek;

            var model = {
                jobId: self.isGroupedJob ? self.localJobAddress.id : self.jobId,
                assignId: self.localDepartment.id,
                departmentId: getDepartmentId(),
                changedByRole: getChangedByRoleValue(self.localDepartment.changedByRole),
                repeatsPerWeek: self.localDepartment.repeatsPerWeek,
                dayPerWeekList: self.localDepartment.dayPerWeekList
            }

            jobProvider.SaveJobShedule(model).then(function (result) {
                if (!self.localDepartment.id) {
                    self.assigns.unshift(result.data);
                }
                self.localDepartment = result.data;
                self.localDepartment.name = localDepartmentName;
            });
            setLocalJobShedulePickerModel();
        }

        function saveGlobalYear(date) {
            var year = (date ? new Date(date) : new Date()).getFullYear();
            self.globalAssign.tillYear = date ? year : 0;
            var model = {
                jobId: self.jobId,
                assignId: self.globalAssign.id,
                departmentId: self.globalAssign.departmentId,
                changedByRole: getChangedByRoleValue(self.globalAssign.changedByRole),
                tillYear: self.globalAssign.tillYear
            };

            jobProvider.saveTillYear(model).then(function () {
                self.localDepartment = null;
                loadAssigns();
            });

            if (self.localDepartment && self.localDepartment.isGlobal) {
                self.localDatePickerConfig.date = year;
                self.localDatePickerConfig.triggerRefresh(year);
            }
        }

        function saveLocalYear(date) {
            var year = (date ? new Date(date) : new Date()).getFullYear();
            var localDepartmentName = self.localDepartment.name;
            self.localDepartment.tillYear = date ? year : 0;
            var model = {
                jobId: self.isGroupedJob ? self.localJobAddress.id : self.jobId,
                assignId: self.localDepartment.id,
                departmentId: getDepartmentId(),
                changedByRole: getChangedByRoleValue(self.localDepartment.changedByRole),
                tillYear: self.localDepartment.tillYear
            };

            jobProvider.saveTillYear(model).then(function (result) {
                if (!self.localDepartment.id) {
                    self.assigns.unshift(result.data);
                }
                self.localDepartment = result.data;
                self.localDepartment.name = localDepartmentName;
            });
        }

        function getChangedByRoleValue(changedByRole) {
            if (changedByRole === 0) {
                return self.currentRole;
            } else if ((changedByRole & self.currentRole) === self.currentRole) {
                return self.currentRole;
            } else {
                return self.changedByAdministratorAndCoordinator;
            }
        }

        function getLocalWeekList(weekList) {
            var localWeekList = angular.copy(weekList);
            localWeekList.forEach(function (week) {
                week.changedBy = self.changedByAdministratorAndCoordinator;
            });

            return localWeekList;
        }

        function getFlagsCurrentRole(roleId) {
            switch (roleId) {
                case Role.Administrator:
                    return FlagsRoleEnum.Administrator;
                case Role.Coordinator:
                    return FlagsRoleEnum.Coordinator;
                case Role.Janitor:
                    return FlagsRoleEnum.Janitor;
                case Role.SuperAdmin:
                    return FlagsRoleEnum.SuperAdmin;
            }
        }

        function refreshLocalDepartment() {
            var departmentName = self.localDepartment.name;
            var departmentId = self.localDepartment.departmentId;
            self.localDepartment = self.assigns.find(function (assign) {
                return assign.housingDepartmentIdList.indexOf(self.localDepartment.departmentId) !== -1;
            });
            self.localDepartment.name = departmentName;
            self.localDepartment.departmentId = departmentId;
        }

        function getDepartmentId() {
            if (self.isGroupedJob) {
                return $stateParams.department ? $stateParams.department : self.filter.departmentid;
            }

            return self.localDepartment ? self.localDepartment.departmentId : null;
        }

        function initControl() {

            checkSecurityPermission();

            if (getPreviousState() == Pages.YearPlan) {
                self.filter = yearPlanService.getFilter();
            } else {
                self.filter = weekPlanService.getFilter();
            }

            memberProvider.GetCurrentUser().then(function (result) {
                self.currentUser = result.data;
                initSecurityRules();
            });

            loadAssigns();
            initDirectives();
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

    angular.module('boligdrift').controller('editTaskIntervalController',
        [
            '$filter',
            '$state',
            '$stateParams',
            'jobProvider',
            'securityService',
            'memberProvider',
            'yearPlanService',
            'weekPlanService',
            'urlService',
            intervalController
        ]);
})();
