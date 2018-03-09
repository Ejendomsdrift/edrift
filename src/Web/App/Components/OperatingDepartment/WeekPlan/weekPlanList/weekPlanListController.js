(function () {
    var weekPlanListController = function ($scope, weekPlanProvider, weekPlanService, $state, $filter, dateHelper, $stateParams) {
        var jobStatusesWithTime = [JobType.Other, JobType.Tenant];
        var self = this;
        self.state = State;
        self.weekedTasks = [];
        self.loadMoreTitle = $filter('translate')('load_more');
        self.weekPlanListViewTabEnum = WeekPlanListViewTab;

        self.currentTab = $stateParams.listViewCurrentTab ? $stateParams.listViewCurrentTab : WeekPlanListViewTab.current;

        self.editTask = function (task) {
            weekPlanService.openTask(task, $state.current.name, self.currentTab, false);
        }

        self.editTaskHistoryTab = function (task) {
            weekPlanService.openTask(task, $state.current.name, self.currentTab, true);
        }

        self.isActive = function (selectedTab) {
            return selectedTab === self.currentTab;
        }

        self.init = function (selectedDepartment, selectedWeek, selectedYear) {
            self.jobStatusNameList = [];
            fillJobStatusNameList();
            self.selectedDepartment = selectedDepartment;
            self.selectedWeek = selectedWeek;
            self.selectedYear = selectedYear;
            self.selectedMemberIds = weekPlanService.getMemberFilter();

            loadJobsForWeek(true);

            loadDayAssignIdsForMembers();

            $state.go(State.WeekPlanListViewCurrentTasks);
        };

        self.loadMore = function () {
            loadJobsForWeek(false);
        }

        self.loadNotCompletedTasks = function () {
            self.previousNotEmptyWeekNumber = null;
            self.currentTab = WeekPlanListViewTab.notCompleted;
            loadJobsForWeek(true);
        };

        self.loadCompletedTasks = function () {
            self.previousNotEmptyWeekNumber = null;
            self.currentTab = WeekPlanListViewTab.completed;
            loadJobsForWeek(true);
        };

        self.loadCurrentTasks = function () {
            self.previousNotEmptyWeekNumber = null;
            self.currentTab = WeekPlanListViewTab.current;
            loadJobsForWeek(true);
        };

        self.getFormattedDateString = function (dateString) {
            if (dateString) {
                return dateHelper.formatUtcDateString(dateString);
            }
        }

        self.getTime = function (task) {
            if (jobStatusesWithTime.indexOf(task.jobType) == -1 || !task.timeDate) {
                return '';
            }
            return dateHelper.getTimeString(task.timeDate);
        }

        self.getJobTypeById = function (jobTypeId) {
            switch (jobTypeId) {
                case JobType.AdHoc:
                    {

                        return $filter('translate')('AdHoc task');
                    }
                case JobType.Tenant:
                    {
                        return $filter('translate')('Tenant task');
                    }
                case JobType.Other:
                    {
                        return $filter('translate')('Other');
                    }
                case JobType.Facility:
                    {
                        return $filter('translate')('Facility');
                    }
                default:
                    {
                        return $filter('translate')('Undefined job type');
                    }
            }
        }

        self.isAllowedTask = function (task) {
            if (!self.selectedMemberIds || !self.selectedMemberIds.length) {
                return true;
            }

            return task.dayAssignId && task.users.find(function (user) {
                for (var i = 0; i < self.selectedMemberIds.length; i++) {
                    if (self.selectedMemberIds[i] == user.memberId) {
                        return true;
                    }
                }
            });
        }

        self.showWeekTitle = function (week) {
            var allowedTask = self.weekedTasks
                .filter(function(weekedTask) { return weekedTask.week == week; })
                .map(function (weekedTask) { return weekedTask.jobs; })
                .find(function(i) { return self.isAllowedTask(i); });

            return allowedTask ? true : false;
        }

        self.getStatusTranslation = function (statusId) {
            return JobStatusPlatformKey.CoordinatorPlatform + self.jobStatusNameList[statusId];
        }

        function loadJobsForWeek(isClearPreviousData) {
            if (!self.selectedDepartment || !self.selectedWeek) {
                return;
            }

            var model = {
                memberIds: self.selectedMemberIds,
                housingDepartmentId: self.selectedDepartment.id || null,
                week: self.previousNotEmptyWeekNumber ? self.previousNotEmptyWeekNumber : self.selectedWeek,
                year: self.selectedYear,
                listViewCurrentTab: self.currentTab
            };

            weekPlanProvider.GetJobsForWeek(model).then(function (result) {
                getJobsForWeekCallback(result, isClearPreviousData);
            });
        }

        function getJobsForWeekCallback(result, isClearPreviousData) {
            self.previousNotEmptyWeekNumber = result.data.previousNotEmptyWeekNumber;
            self.isAllowedPreviousWeeks = result.data.isAllowedPreviousWeeks;

            if (isClearPreviousData) {
                self.weekedTasks = [];
            }

            if (!result.data.jobs.length) {
                return;
            }

            var model = {
                week: result.data.jobs[0].weekNumber,
                jobs: result.data.jobs
            };

            self.weekedTasks.push(model);
        }

        function loadDayAssignIdsForMembers() {
            if (!self.selectedMemberIds.length || !self.selectedDepartment) {
                self.allowedDayAssignIds = [];
                return;
            }

            var model = {
                housingDepartmentId: self.selectedDepartment.id,
                startWeek: self.selectedWeek,
                year: self.selectedYear,
                memberIds: self.selectedMemberIds
            };

            weekPlanProvider.GetDayAssignIdsForMembers(model).then(function (result) {
                self.allowedDayAssignIds = result.data;
            });
        }

        function fillJobStatusNameList() {
            for (var name in JobStatus) {
                if (JobStatus.hasOwnProperty(name)) {
                    self.jobStatusNameList.push(name);
                }
            }
        }

        var weekPlanFilterChanged = $scope.$on('weekPlanFilterChanged', function (event, data) {
            self.selectedWeek = data.selectedWeek;
            self.selectedYear = data.selectedYear;
            self.selectedDepartment = data.selectedDepartment;
            self.previousNotEmptyWeekNumber = null;
            loadDayAssignIdsForMembers();

            loadJobsForWeek(true);
        });

        var weekPlanMemberFilterChanged = $scope.$on('weekPlanMemberFilterChanged', function (event, data) {
            self.selectedMemberIds = data.selectedMemberIds;
            loadJobsForWeek(true);
        });

        var refreshWeekPlanListTasks = $scope.$on('refreshWeekPlanListTasks', function () {
            loadDayAssignIdsForMembers();
            loadJobsForWeek(true);
        });

        $scope.$on('$destroy', weekPlanFilterChanged);
        $scope.$on('$destroy', weekPlanMemberFilterChanged);
        $scope.$on('$destroy', refreshWeekPlanListTasks);
    };

    angular.module("boligdrift").controller('weekPlanListController', ['$scope', 'weekPlanProvider', 'weekPlanService', '$state', '$filter', 'dateHelper', '$stateParams', weekPlanListController]);
})();