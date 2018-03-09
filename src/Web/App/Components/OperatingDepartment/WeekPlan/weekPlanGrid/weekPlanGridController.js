(function () {
    var weekPlanGridController = function ($scope, $state, $stateParams, $filter, moment, weekPlanProvider, weekPlanService, jobProvider, janitorService, dateHelper) {
        var self = this;
        var MINUTES_IN_HOUR = 60;

        self.init = function (selectedDepartment, selectedWeek, selectedYear) {
            self.filter = weekPlanService.getFilter();
            self.jobStatusNameList = [];
            fillJobStatusNameList();
            self.selectedWeek = selectedWeek;
            self.selectedYear = selectedYear;
            self.selectedDepartment = selectedDepartment;
            self.selectedMemberIds = weekPlanService.getMemberFilter();

            loadDayAssignIdsForMembers();
            if (selectedWeek && selectedYear) {
                getWeekTasks(false);
            }
        };

        self.getWeekNumber = function (taskItem) {
            if (!taskItem) return null;
            if (taskItem.expiredDayAssignId) {
                return taskItem.expiredWeekNumber;
            } else {
                return taskItem.weekNumber;
            }
        };

        self.getDayNumber = function (dateString) {
            var date = new Date(dateString);
            var day = date.getDate();
            return day;
        }

        self.getMonthName = function (dateString) {
            var date = new Date(dateString);
            var month = date.getMonth();
            return Month[month];
        }

        self.assignDay = function (weekTask, day, date) {
            weekTask.weekDay = day;
            weekPlanService.storeDayFilter(day ? day : 0);
            weekPlanService.storeDateFilter(date);
            redirectTo(weekTask);
        };

        self.showHideCommenBlock = function (e, taskItem) {
            e.preventDefault();
            e.stopPropagation();
            taskItem.isShowCommentBlock = !taskItem.isShowCommentBlock;
        }

        self.isTaskExpired = function (week, currentWeek) {
            return week < currentWeek;
        }

        self.isTaskDoesntHasAssignedUsers = function (job) {
            var result = (job.users == null || !job.users.length) &&
                         (job.groupId == null || job.groupId == '') &&
                         !job.isAssignedToAllUsers;
            return result;
        }

        self.sortableOptions = {
            connectWith: ".apps-container",
            disabled: true,
            items: "li:not(.not-draggable)",
            change: function (e, ui) {
                var task = ui.item.sortable.model;
                ui.item.sortable._connectedSortables.forEach(function (elem) {
                    var day = elem.element.attr('data-day');
                    if (typeof day !== typeof undefined) {
                        if (task.allowedDays.length) {
                            if (!$(task.allowedDays).is(function () { return this == day })) {
                                elem.element.parent().addClass('_notallowed');
                            }
                        }
                    }
                });
            },
            update: function (e, ui) {
                if (!ui.item.sortable.received && ui.item.sortable.droptarget) {
                    var target = $(ui.item.sortable.droptarget[0]);
                    var task = ui.item.sortable.model;
                    var day = target.attr('data-day');
                    if (typeof day !== typeof undefined) {
                        if (task.allowedDays.length) {
                            var isAllowed = $(task.allowedDays).is(function () { return this == day });
                            if (!isAllowed) {
                                ui.item.sortable.cancel();
                            }
                        }
                    }
                }
            },
            stop: function (e, ui) {
                ui.item.sortable._connectedSortables
                    .forEach(function (elem) { elem.element.parent().removeClass('_notallowed'); });
                if (ui.item.sortable.droptarget) {
                    var target = $(ui.item.sortable.droptarget[0]);
                    if (ui.item.sortable.isCanceled()) {
                        return;
                    }

                    assignDay(e, ui, target);
                }
            }
        };

        self.isTaskNotDraggable = function (task) {
            return task.jobType === JobType.Tenant;
        }

        self.getClassIcon = function (status) {
            var resultClass = '';

            switch (status) {
                case JobStatus.Pending:
                    resultClass += 'fa-plus';
                    break;
                case JobStatus.Opened:
                    resultClass += 'fa-clone';
                    break;
                case JobStatus.InProgress:
                    resultClass += 'fa-play';
                    break;
                case JobStatus.Paused:
                    resultClass += 'fa-pause';
                    break;
                case JobStatus.Completed:
                    resultClass += 'fa-check';
                    break;
                case JobStatus.Canceled:
                    resultClass += 'fa-close';
                    break;
                case JobStatus.Assigned:
                    resultClass += 'fa-user';
                    break;
                case JobStatus.Rejected:
                    resultClass += 'fa-minus-circle';
                    break;
            }

            return resultClass;
        }

        self.getTaskTimeString = function (task) {
            if (task.jobType == JobType.Other || task.jobType == JobType.Tenant) {
                return dateHelper.getTimeString(task.dayAssignDate);
            }
        }

        self.isTaskDurationNeeded = function (job) {
            return job && job.dayAssignDate && job.estimate && (job.jobType === JobType.Tenant || job.jobType === JobType.Other);
        }

        self.getEstimateTimeString = function (job) {
            var date = new Date(job.dayAssignDate);
            var estimateInMinutes = job.estimate * MINUTES_IN_HOUR;
            return dateHelper.getTimeString(date.setMinutes(date.getMinutes() + estimateInMinutes));
        }

        self.preventOpenAssignPopup = function ($event, taskItem) {
            $event.stopPropagation();
            if ($event.target.className.indexOf('popup__close pe-7s-close') > -1) {
                taskItem.isShowCommentBlock = !taskItem.isShowCommentBlock;
            }
        }

        self.isAllowedTask = function (task) {
            if (!self.selectedMemberIds || !self.selectedMemberIds.length) {
                return true;
            }
            return task.dayAssignId && self.allowedDayAssignIds.indexOf(task.dayAssignId) > -1;
        }

        self.getStatusTranslation = function (status) {
            return JobStatusPlatformKey.CoordinatorPlatform + self.jobStatusNameList[status];
        }

        self.getAssignToLabel = function (task) {
            if (task.statusId == JobStatus.Opened) {
                return $filter('translate')('opened for all');
            } else if (task.statusId == JobStatus.Assigned) {
                return $filter('translate')('all members');
            }
        }

        function fillJobStatusNameList() {
            for (var name in JobStatus) {
                if (JobStatus.hasOwnProperty(name)) {
                    self.jobStatusNameList.push(name);
                }
            }
        }

        function redirectTo(weekTask) {
            weekPlanService.openTask(weekTask, $state.current.name, null, false);
        }

        function assignDay(e, ui, target) {
            var task = ui.item.sortable.model;
            var newWeekDay = target.attr('data-day');
            var weekDay = newWeekDay ? newWeekDay : null;
            if (weekDay != task.weekDay) {

                var model = {
                    jobId: task.id,
                    departmentId: self.filter.departmentid,
                    userIdList: [],
                    jobAssignId: task.jobAssignId,
                    dayAssignId: task.dayAssignId,
                    weekDay: weekDay,
                    currentWeekDay: task.weekDay,
                    weekNumber: task.weekNumber,
                    year: self.selectedYear
                };

                jobProvider.ChangeDayAssignDate(model).then(function (result) {
                    task.dayAssignId = result.data;
                    task.isBackLogJob = weekDay == null;
                });
            }
        }

        function getWeekTasks(isDateChanged) {
            if (self.selectedDepartment && self.selectedWeek) {
                var model = {
                    housingDepartmentId: self.selectedDepartment.id,
                    week: self.selectedWeek,
                    year: self.selectedYear,
                    memberIds: self.selectedMemberIds
                };

                weekPlanProvider.GetWeekTasks(model).then(function (result) {
                    self.weekendJobCount = result.data.weekendJobCount;
                    informWeekPlanAboutWeekEndJobsCount();
                    self.isShowWeekend = isDateChanged
                        ? result.data.isShowWeekend
                        : $stateParams.isEditJobPopupClosed
                            ? self.filter.isShowWeekend
                            : result.data.isShowWeekend;
                    informIsShowWeekendChanged();
                    self.backLogTasks = result.data.backLogJobs;
                    self.backLogTasks = sortBackLogTasks();

                    self.backLogTasks.forEach(function (item, index) {
                        item.uniqueId = Date.now() + "_" + index;
                    });

                    weekPlanService.getWeekDays(self.selectedYear, self.selectedWeek).then(function (weekDays) {
                        self.weekDays = weekDays;
                        self.weekDays.forEach(function (week) {
                            week.tasks = result.data.weekJobs.filter(function (d, index) {
                                d.uniqueId = Date.now() + "_" + week.dayNumber + "_" + index;
                                return !d.isBackLogJob && d.weekDay == week.dayNumber;
                            });
                        });
                        weekPlanService.storeFilter(self.selectedYear, self.selectedWeek, self.selectedDepartment, self.isShowWeekend);

                        weekPlanService.getCurrentWeek().then(function (currentWeek) {
                            var currentYear = new Date().getFullYear();
                            self.currentWeek = currentWeek;
                            self.isPreviousWeek = self.filter.year < currentYear || (self.filter.year == currentYear && self.filter.week < currentWeek);
                            self.sortableOptions.disabled = self.isPreviousWeek;
                        });
                    });
                });
            }
        }

        function sortBackLogTasks() {
            var sortedTasks = self.backLogTasks.sort(function (a, b) {
                var firstTaskWeekNumber = a.expiredWeekNumber ? a.expiredWeekNumber : a.weekNumber;
                var secondTaskWeekNumber = b.expiredWeekNumber ? b.expiredWeekNumber : b.weekNumber;
                return firstTaskWeekNumber - secondTaskWeekNumber;
            });

            return sortedTasks;
        }

        function loadDayAssignIdsForMembers() {
            if (self.selectedMemberIds && !self.selectedMemberIds.length || !self.selectedDepartment) {
                self.allowedDayAssignIds = [];
                return;
            }

            var model = {
                housingDepartmentId: self.selectedDepartment.id,
                week: self.selectedWeek,
                year: self.selectedYear,
                memberIds: self.selectedMemberIds
            };

            weekPlanProvider.GetDayAssignIdsForMembers(model).then(function (result) {
                self.allowedDayAssignIds = result.data;
            });
        }

        var weekPlanFilterChanged = $scope.$on('weekPlanFilterChanged', function (event, data) {
            var isDataChanged = false;
            if (self.selectedWeek && self.selectedWeek != data.selectedWeek) {
                isDataChanged = true;
            }

            self.selectedWeek = data.selectedWeek;
            self.selectedYear = data.selectedYear;
            self.selectedDepartment = data.selectedDepartment;

            loadDayAssignIdsForMembers();
            getWeekTasks(isDataChanged);
        });

        var refreshWeekPlanGridTasks = $scope.$on('refreshWeekPlanGridTasks', function () {
            getWeekTasks(false);
        });

        var weekPlanMemberFilterChanged = $scope.$on('weekPlanMemberFilterChanged', function (event, data) {
            self.selectedMemberIds = data.selectedMemberIds;
            loadDayAssignIdsForMembers();
        });

        $scope.$on('$destroy', weekPlanFilterChanged);
        $scope.$on('$destroy', weekPlanMemberFilterChanged);
        $scope.$on('$destroy', refreshWeekPlanGridTasks);

        function informIsShowWeekendChanged() {
            $scope.$emit('isShowWeekendChanged', {
                isShowWeekend: self.isShowWeekend
            });
        }

        function informWeekPlanAboutWeekEndJobsCount() {
            $scope.$emit('weekEndJobCountInformationEvent', {
                weekendJobCount: self.weekendJobCount
            });
        }
    };

    angular.module("boligdrift").controller('weekPlanGridController',
        [
            '$scope',
            '$state',
            '$stateParams',
            '$filter',
            'moment',
            'weekPlanProvider',
            'weekPlanService',
            'jobProvider',
            'janitorService',
            'dateHelper',
            weekPlanGridController
        ]);
})();