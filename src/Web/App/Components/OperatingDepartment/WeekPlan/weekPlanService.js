(function () {
    var weekPlanService = function (localStorageService, calendarProvider, $state) {
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

        self.getWeekDays = function (selectedYear, selectedWeek) {
            return calendarProvider.getWeekDays(selectedYear, selectedWeek);
        };

        self.getCurrentWeek = function () {
            return calendarProvider.getCurrentWeek();
        };

        self.storeFilter = function (selectedYear, selectedWeek, selectedDepartment, isShowWeekend) {
            var weekPlanFilter = localStorageService.get("weekPlan") || {};
            if (selectedDepartment) {
                weekPlanFilter.departmentid = selectedDepartment.id;
            }
            if (selectedYear) {
                weekPlanFilter.year = selectedYear;
            }
            if (selectedWeek) {
                weekPlanFilter.week = selectedWeek;
            }
            if (typeof isShowWeekend !== typeof undefined) {
                weekPlanFilter.isShowWeekend = isShowWeekend;
            }

            localStorageService.set("weekPlan", weekPlanFilter);
        };

        self.getFilter = function () {
            return localStorageService.get("weekPlan") || {};
        }

        self.storeDayFilter = function (currentDay) {
            localStorageService.set("dayFilter", currentDay);
        }

        self.storeDateFilter = function (currentDate) {
            localStorageService.set("dateFilter", currentDate);
        }

        self.storeDayAssignId = function (dayAssignId) {
            localStorageService.set("dayAssignId", dayAssignId);
        }

        self.storeMemberFilter = function (selectedIds) {
            localStorageService.set(getMemberFilterKey(), selectedIds);
        }

        self.getMemberFilter = function () {
            var data = localStorageService.get(getMemberFilterKey());
            return data ? data : [];
        }

        self.getDayAssignId = function () {
            return localStorageService.get("dayAssignId");
        }

        self.clearStoredDayAssign = function () {
            localStorageService.set("dayAssignId", null);
        }

        self.getDayFilter = function () {
            return localStorageService.get("dayFilter") || 0;
        }

        self.getDateFilter = function () {
            return localStorageService.get("dateFilter") || new Date();
        }

        self.clearDepartmentFilter = function () {
            var filter = localStorageService.get("weekPlan") || {};
            filter.departmentid = null;
            localStorageService.set("weekPlan", filter);
        };

        self.storeHistoryAddress = function(historyAddress) {
            var filter = localStorageService.get("historyAddress") || {};
            filter.historyAddress = historyAddress;
            localStorageService.set("historyAddress", filter);
        }

        self.getHistoryAddress = function () {
            return localStorageService.get("historyAddress");
        }

        self.openTask = function (weekTask, redirectUrl, currentListViewTab, isHistoryTab) {
            var filter = localStorageService.get("weekPlan") || {};

            switch (weekTask.jobType) {
                case JobType.Facility:
                    redirectToFacilityTask(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab);
                    break;
                case JobType.AdHoc:
                    redirectToAdHoc(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab);
                    break;
                case JobType.Tenant:
                    redirectToTenant(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab);
                    break;
                case JobType.Other:
                    redirectToOtherTypeTask(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab);
                    break;
            }
        }

        function redirectToFacilityTask(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab) {

            var redirectState = isHistoryTab ? State.FacilityTaskHistory : State.FacilityTaskDayAssign;
            $state.go(redirectState,
            {
                id: weekTask.id,
                department: filter.departmentid,
                dayAssignId: weekTask.dayAssignId ? weekTask.dayAssignId : '',
                week: weekTask.weekNumber,
                redirectState: redirectUrl,
                isBackLogJob: weekTask.isBackLogJob,
                listViewCurrentTab: currentListViewTab,
                weekDay: weekTask.weekDay
            });
        }

        function redirectToAdHoc(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab) {
            var redirectState = isHistoryTab ? State.OperationalTaskHistoryAdHoc : State.OperationalTaskEditAdHoc;
            $state.go(redirectState,
            {
                id: weekTask.id,
                dayAssignId: weekTask.dayAssignId,
                departmentId: filter.departmentid,
                jobType: weekTask.jobType,
                redirectState: redirectUrl,
                listViewCurrentTab: currentListViewTab
            });
        }

        function redirectToTenant(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab) {
            var redirectState = isHistoryTab ? State.OperationalTaskHistoryTenant : State.OperationalTaskEditTenant;
            $state.go(redirectState,
            {
                id: weekTask.id,
                dayAssignId: weekTask.dayAssignId,
                departmentId: filter.departmentid,
                jobType: weekTask.jobType,
                redirectState: redirectUrl,
                listViewCurrentTab: currentListViewTab
            });
        }

        function redirectToOtherTypeTask(weekTask, filter, redirectUrl, currentListViewTab, isHistoryTab) {
            var redirectState = isHistoryTab ? State.OperationalTaskHistoryOther : State.OperationalTaskEditOther;
            $state.go(redirectState,
            {
                id: weekTask.id,
                dayAssignId: weekTask.dayAssignId,
                departmentId: filter.departmentid,
                jobType: weekTask.jobType,
                redirectState: redirectUrl,
                listViewCurrentTab: currentListViewTab
            });
        }

        function getMemberFilterKey() {
            return $state.current.name === State.WeekPlanTimeView ?
                   "timeViewMemberFilterSelectedIds" :
                   "taskMemberFilterSelectedIds";
        }
    };

    angular.module('boligdrift').service('weekPlanService', ['localStorageService', 'calendarProvider', '$state', weekPlanService]);

})();