(function () {
    var openTasksController = function ($scope, $state, $filter, jobProvider, janitorService) {
        var self = this;
        self.jobStatusNameList = [];
        self.isVisibleButton = false;

        var refreshOpenTasks = $scope.$on('refreshOpenTasks', function () {
            loadJobs();
            updateTaskCount();
        });

        self.isHaveEstimations = function (jobHeader) {
            return jobHeader.estimate > 0;
        }

        self.getEstimateTimeInHoursString = function (jobHeader) {
            return janitorService.getEstimateTimeInHoursString(jobHeader.estimate);
        }

        self.goToTaskDetails = function (jobDetails) {
            $state.go(State.JanitorTaskDetails, { id: jobDetails.name, dayAssignId: jobDetails.id, previousState: $state.current.name, jobType: jobDetails.jobType });
        }

        self.getDateString = function (dateString) {
            return janitorService.getFormatedDateString(dateString);
        }

        self.getTimeBeforeStartString = function (date) {
            return janitorService.getTimeBeforeStartString(date);
        }

        self.getTimeBeforeStartLabel = function (task) {
            return janitorService.getTimeBeforeStartLabel(task);
        }

        self.scrollToTop = function () {
            $("body,html").animate({
                scrollTop: 0
            }, 400);
            return false;
        }

        self.isTaskOverdue = function (task) {
            return task && janitorService.isTaskOverdue(task);
        }

        self.isStartTimeLabelVisible = function (task) {
            return task && janitorService.isStartTimeLabelVisible(task);
        }

        self.isTenant = function (jobHeader) {
            return jobHeader.jobType === JobType.Tenant;
        }

        self.getStatusTranslation = function (status) {
            return JobStatusPlatformKey.JanitorPlatform + self.jobStatusNameList[status];
        }

        angular.element(window).on("scroll", function (e) {
            $scope.$apply(function () {
                self.isVisibleButton = window.scrollY > 80;
            });
        });

        function updateTaskCount() {
            $scope.$emit('janitorTaskCountChanged', {});
        }

        function fillJobStatusNameList() {
            for (var name in JobStatus) {
                if (JobStatus.hasOwnProperty(name)) {
                    self.jobStatusNameList.push(name);
                }
            }
        }

        function loadJobs() {
            fillJobStatusNameList();
            jobProvider.GetOpenedJobsHeaderList().then(function (result) {
                self.jobHeaderList = result.data;
                updateTaskCount(self.jobHeaderList);
            });
        }

        loadJobs();

        $scope.$on('$destroy', refreshOpenTasks);
    };

    angular.module('boligdrift').controller('openTasksController', ['$scope', '$state', '$filter', 'jobProvider', 'janitorService', openTasksController]);
})();