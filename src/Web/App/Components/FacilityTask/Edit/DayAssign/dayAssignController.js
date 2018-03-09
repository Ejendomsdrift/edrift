(function () {
    var dayAssignController = function ($state,
        $stateParams,
        jobProvider,
        calendarProvider,
        cancellingTemplatesProvider,
        weekPlanService,
        $filter,
        securityService,
        dateHelper,
        toastService,
        urlService) {

        var self = this;

        var MINUTES_IN_HOUR = 60;
        var openedJobStatuses = [JobStatus.Opened, JobStatus.Rejected];

        var loadTask = function (assignId) {
            self.selectedDepartment = $stateParams.department;
            self.currentDayNumber = $stateParams.weekDay ? $stateParams.weekDay : weekPlanService.getDayFilter();
            self.currentDate = weekPlanService.getDateFilter();
            fillDayAssignId(assignId);
            var model = {
                jobId: $stateParams.id,
                housingDepartmentId: $stateParams.department,
                dayAssignId: self.dayAssignId,
                currentWeekDay: self.currentDayNumber,
                weekNumber: $stateParams.week,
                date: self.currentDate
            };

            jobProvider.GetWeekTask(model).then(function (result) {
                self.isBacklogJob = result.data.job.dayAssigns.length == 0;
                self.facilityTask = result.data.job;
                changeTeamPickerControlMode(self.facilityTask);
                self.jobAssignId = result.data.jobAssignId;
                loadWeekDays();
            });
        };

        var loadWeekDays = function () {
            calendarProvider.getWeekDays(new Date().getFullYear(), $stateParams.week).then(function (result) {
                self.weekDays = [];
                result.forEach(function (item) {
                    var day = {
                        dayNumber: item.dayNumber,
                        dayOfWeek: item.dayOfWeek,
                        date: item.date
                    }
                    self.weekDays.push(day);
                });
                fillData();
            });
        }

        var fillData = function () {
            self.currentAssign = self.facilityTask.dayAssigns && self.facilityTask.dayAssigns.length > 0
                ? self.facilityTask.dayAssigns[0] : null;
            if (self.currentAssign) {
                self.currentDayNumber = self.currentAssign.weekDay;
                var dayPerWeekList = self.currentAssign && self.currentAssign.weekDay ? [self.currentAssign.weekDay] : [];
                self.isJobOpened = openedJobStatuses.indexOf(self.currentAssign.statusId) > -1;
                self.isCompleted = self.currentAssign.statusId === JobStatus.Completed;
                self.isCanceled = self.currentAssign.statusId === JobStatus.Canceled;
                self.daysPerWeekPickerConfig.value = getDayPerWeekListModel(dayPerWeekList);
                self.daysPerWeekPickerConfig.allowedDaysCount = 1;
                self.daysPerWeekPickerConfig.triggerRefresh();
                setEstimate();
            } else { //when we opened virtual ticket
                self.selectedDay = self.weekDays.filter(function (d) { return d.dayNumber === self.currentDayNumber })[0];
                self.isCanceled = false;
                self.dayPerWeek = self.selectedDay ? [self.selectedDay.dayNumber] : [];
                self.daysPerWeekPickerConfig.value = getDayPerWeekListModel(self.dayPerWeek);
                self.daysPerWeekPickerConfig.allowedDaysCount = 1;
                self.daysPerWeekPickerConfig.triggerRefresh();
            }

            refreshTeamPicker();
        }

        self.onCancel = function () {
            var msg = $filter('translate')('Are you sure to cancel task?');

            if (confirm(msg)) {
                var model = {
                    jobId: $stateParams.id,
                    departmentId: $stateParams.department,
                    dayAssignId: self.dayAssignId,
                    isJobOpen: self.isJobOpened,
                    userIdList: [],
                    currentWeekDay: self.currentDayNumber,
                    weekDay: self.currentDayNumber,
                    weekNumber: $stateParams.week,
                    date: self.currentDate,
                    cancellationReasonId: self.cancellationReasonId
                };

                jobProvider.DayAssignCancel(model).then(function (result) {
                    self.isCanceled = true;
                    $stateParams.dayAssignId = result.data;
                    initSecurityRules();
                });

            }
        }

        self.onReopen = function () {
            var msg = $filter('translate')('Are you sure you want to reopen this task?');
            if (confirm(msg)) {
                jobProvider.ReopenJob(self.dayAssignId).then(function () {
                    self.isReopened = true;
                });
            }
        }

        function setEstimate() {
            self.estimateConfig.value = self.currentAssign.estimatedMinutes / 60;
            if (self.facilityTask.dayAssigns.length && isJobInCertainStatuses() && self.currentAssign.estimatedMinutes == 0) {
                self.estimateConfig.mode = ControlMode.edit;
                self.estimateConfig.triggerValidate();
            }
            else if (self.estimateConfig.mode == ControlMode.edit && self.currentAssign.estimatedMinutes > 0) {
                self.estimateConfig.mode = ControlMode.view;
            }

            self.estimateConfig.triggerRefresh();
        }

        function setCancellingReason() {
            cancellingTemplatesProvider.GetCoordinatorByTaskType(JobType.Facility).then(function (result) {
                self.cancellationReasonConfig.dataList = result.data.map(function (reason) {
                    return {
                        id: reason.id,
                        value: reason.text
                    }
                });

                self.cancellationReasonConfig.triggerUpdateDataList();
            });
        }

        function isJobInCertainStatuses() {
            var dayAssign = self.facilityTask.dayAssigns[0];
            return dayAssign.statusId == JobStatus.Opened || dayAssign.statusId == JobStatus.Assigned;
        }

        function getDayPerWeekListModel(daysPerWeekList) {
            var result = [];
            daysPerWeekList.forEach(function (day) {
                result.push({ id: '', weekDay: day });
            });

            return result;
        }

        function changeDate(day) {
            var selectedDay = day.length ? day[0].weekDay : null;
            var departmentId = getDepartmentId();

            var model = {
                jobId: self.facilityTask.id,
                departmentId: departmentId,
                userIdList: [],
                jobAssignId: self.jobAssignId,
                dayAssignId: self.dayAssignId,
                weekDay: selectedDay,
                currentWeekDay: self.currentDayNumber,
                weekNumber: $stateParams.week,
                year: self.filter.year
            };

            jobProvider.ChangeDayAssignDate(model).then(function (result) {
                loadTask(result.data);
            });

            weekPlanService.storeDayFilter(selectedDay);
        };

        function reasonUpdated(reason) {
            if (reason) {
                self.disableCancelTaskButton = false;
                self.cancellationReasonId = reason.id;
            } else {
                self.disableCancelTaskButton = true;
            }
        }

        function changeEstimate(estimateInHours) {
            var estimateInMinutes = estimateInHours * MINUTES_IN_HOUR;

            var model = {
                jobId: $stateParams.id,
                departmentId: getDepartmentId(),
                userIdList: [],
                dayAssignId: self.dayAssignId,
                estimatedMinutes: estimateInMinutes,
                weekDay: getWeekDay(),
                currentWeekDay: self.currentDayNumber,
                weekNumber: $stateParams.week,
                date: self.currentDate
            };

            jobProvider.ChangeDayAssignEstimate(model).then(function (result) {
                loadTask(result.data);
            });
        }

        function getDepartmentId() {
            return $stateParams.department ? $stateParams.department : self.filter.departmentid;
        }

        function getWeekDay() {
            if (self.selectedDay) {
                return self.selectedDay.dayNumber;
            }

            if (self.dayPerWeek && self.dayPerWeek.length) {
                return self.dayPerWeek[0];
            }

            return null;
        }

        function saveTeam(team) {
            team.departmentId = getDepartmentId();
            team.weekNumber = $stateParams.week;
            team.jobId = $stateParams.id;
            team.dayAssignId = self.dayAssignId;
            team.weekDay = getWeekDay();
            team.currentWeekDay = self.currentDayNumber;
            team.date = self.currentDate;

            jobProvider.AssignsMembersGroup(team).then(function (result) {
                if (result.data.isSuccessful) {
                    loadTask(result.data.dayAssignId);
                    toastService.showToastSaveSuccessMessage();
                } else {
                    var msg = $filter('translate')('Job has already changed status');
                    alert(msg);
                    loadTask(result.data.dayAssignId);
                    disableTeamPicker();
                }
            });
        }

        function teamChangedHandler(isTeamPickerActive) {
            if (!self.currentAssign) {
                return;
            }

            self.currentAssign.groupId = self.teamPickerConfig.groupId;
            self.currentAssign.teamLeadId = self.teamPickerConfig.teamLeadId;
            self.currentAssign.userIdList = self.teamPickerConfig.userIdList;
            self.currentAssign.isAssignedToAllUsers = self.teamPickerConfig.isAssignedToAllUsers;
            self.isTeamPickerActive = isTeamPickerActive;
        }

        function refreshTeamPicker() {
            self.teamPickerConfig.timeViewDayScope = self.currentDayNumber
                ? dateHelper.dateFromYearWeekDay(self.filter.year, $stateParams.week, self.currentDayNumber)
                : null;

            if (!self.currentAssign) {
                self.teamPickerConfig.triggerRefresh();
                return;
            }

            var isAssignedToAllUsers = self.currentAssign.isAssignedToAllUsers || (self.currentAssign.groupId == null && self.currentAssign.userIdList.length == 0);

            self.teamPickerConfig.groupId = self.currentAssign.groupId;
            self.teamPickerConfig.teamLeadId = self.currentAssign.teamLeadId;
            self.teamPickerConfig.isAssignedToAllUsers = isAssignedToAllUsers;
            self.teamPickerConfig.userIdList = self.currentAssign.userIdList ? self.currentAssign.userIdList : [];
            self.teamPickerConfig.isOpenedState = self.isJobOpened;
            self.teamPickerConfig.triggerRefresh();
        }

        function manageSecurityPermissions(securityResult) {
            var mode = securityResult[self.teamPickerConfig.securityKey] && !isPausedOrInProgressStatus() ? ControlMode.view : ControlMode.disable;
            self.teamPickerConfig.mode = mode;

            mode = securityResult[self.estimateConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.estimateConfig.mode = mode;
            self.estimateConfig.triggerRefresh();

            mode = securityResult[self.daysPerWeekPickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.daysPerWeekPickerConfig.mode = mode;
            self.daysPerWeekPickerConfig.triggerRefresh();

            self.disableCancelTaskControl = !securityResult[ControlSecurityKey.DayAssignCancelTask];
        }

        function changeTeamPickerControlMode(task) {
            if (task.dayAssigns[0] && (task.dayAssigns[0].statusId == JobStatus.InProgress || task.dayAssigns[0].statusId == JobStatus.Paused)) {
                disableTeamPicker();
            }
        }

        function disableTeamPicker() {
            self.teamPickerConfig.mode = ControlMode.disable;
            self.teamPickerConfig.triggerRefresh();
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.DayAssign,
                jobId: $stateParams.id,
                page: page,
                dayAssignId: $stateParams.dayAssignId
            };

            securityService.hasAccessByGroupName(model).then(function (result) {
                manageSecurityPermissions(result.data);
                loadTask();
            });
        }

        function getPreviousState() {
            if ($stateParams.redirectState.indexOf(Pages.YearPlan) != -1) {
                return Pages.YearPlan;
            } else if ($stateParams.redirectState.indexOf(Pages.WeekPlan) != -1) {
                return Pages.WeekPlan;
            }
        }

        function initDirectives() {
            self.teamPickerConfig = new TeamPickerModel();
            self.teamPickerConfig.onChange = teamChangedHandler;
            self.teamPickerConfig.onSave = saveTeam;
            self.teamPickerConfig.securityKey = ControlSecurityKey.DayAssignTeam;
            self.teamPickerConfig.isRequired = false;

            self.estimateConfig = new TextControlModel();
            self.estimateConfig.inputType = InputType.number;
            self.estimateConfig.onSave = changeEstimate;
            self.estimateConfig.value = 0;
            self.estimateConfig.securityKey = ControlSecurityKey.DayAssignEstimate;

            self.cancellationReasonConfig = new SelectControlModel();
            self.cancellationReasonConfig.placeholder = $filter('translate')('Task cancelling Templates');
            self.cancellationReasonConfig.onUpdate = reasonUpdated;

            self.daysPerWeekPickerConfig = new DaysPerWeekPickerModel();
            self.daysPerWeekPickerConfig.onSave = changeDate;
            self.daysPerWeekPickerConfig.securityKey = ControlSecurityKey.DayAssignDaysPerWeek;
        }

        function isPausedOrInProgressStatus() {
            return self.currentAssign && (self.currentAssign.statusId === JobStatus.Paused || self.currentAssign.statusId === JobStatus.InProgress);
        }

        function fillDayAssignId(assignId) {
            self.dayAssignId = weekPlanService.getDayAssignId();
            if (!self.dayAssignId) {
                self.dayAssignId = $stateParams.dayAssignId ? $stateParams.dayAssignId : assignId ? assignId : '';
                storeDayAssign(self.dayAssignId);
            }
        }

        function storeDayAssign(dayAssignId) {
            if (dayAssignId) {
                weekPlanService.storeDayAssignId(dayAssignId);
            }
        }

        function initControl() {

            checkSecurityPermission();

            self.disableCancelTaskControl = true;
            self.disableCancelTaskButton = true;

            self.filter = weekPlanService.getFilter();

            initDirectives();
            setCancellingReason();

            initSecurityRules();
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.FacilityTaskEditPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.FacilityTaskEditPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    };

    angular.module('boligdrift').controller('dayAssignController',
        [
            '$state',
            '$stateParams',
            'jobProvider',
            'calendarProvider',
            'cancellingTemplatesProvider',
            'weekPlanService',
            '$filter',
            'securityService',
            'dateHelper',
            'toastService',
            'urlService',
            dayAssignController
        ]);
})();