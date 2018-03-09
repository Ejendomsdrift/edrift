(function () {
    var myTasksController = function ($timeout, $scope, $state, $filter, jobProvider, managementProvider, memberProvider, janitorService, securityService, urlService) {
        var self = this;

        self.jobStatusNameList = [];
        self.isVisibleButton = false;
        self.state = State;

        var refreshJanitorTasks = $scope.$on('refreshJanitorTasks', function () {
            loadJobs();
            updateTaskCount();
        });

        self.goToTaskDetails = function (event, jobDetails) {
            $state.go(State.JanitorTaskDetails, { id: jobDetails.name, dayAssignId: jobDetails.id, previousState: $state.current.name, jobType: jobDetails.jobType });
            saveScrollPosition(event, jobDetails);
        }

        self.getDateString = function (dateString) {
            return janitorService.getFormatedDateString(dateString);
        }

        self.getTimeString = function (time) {
                return janitorService.getFormatedTimeString(time);
        }

        self.isTaskTenantOrOther = function (jobType) {
            return jobType === JobType.Tenant || jobType === JobType.Other;
        }

        self.isTenant = function (jobHeader) {
            return jobHeader.jobType === JobType.Tenant;
        }

        self.isTaskDurationNeeded = function (jobHeader) {
            return jobHeader && jobHeader.date && jobHeader.estimate && (jobHeader.jobType === JobType.Tenant || jobHeader.jobType === JobType.Other);
        }

        self.getEstimateTimeString = function (jobHeader) {
            return janitorService.getEstimateTimeString(jobHeader.date, jobHeader.estimate);
        }

        self.getEstimateTimeInHoursString = function (jobHeader) {
            return janitorService.getEstimateTimeInHoursString(jobHeader.estimate);
        }

        self.isHaveEstimations = function(jobHeader) {
            return jobHeader.estimate > 0;
        }

        self.isTaskOverdue = function (task) {
            return task && janitorService.isTaskOverdue(task);
        }

        self.isStartTimeLabelVisible = function (task) {
            return task && janitorService.isStartTimeLabelVisible(task);
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

        self.createOperationalTask = function () {
            $state.go(State.OperationalTaskCreateAdHoc, { redirectState: $state.current.name, jobType: JobType.AdHoc });
        }

        self.getStatusTranslation = function (status) {
            return JobStatusPlatformKey.JanitorPlatform + self.jobStatusNameList[status];
        }

        function saveScrollPosition(event, jobDetails) {
            var positionToScrolableArea = $(event.currentTarget).offset().top;
            sessionStorage.setItem('positionToScrolableArea', positionToScrolableArea);
        }

        function onTasksPageLoad() {
            var positionToScrolableArea = Number(sessionStorage.getItem('positionToScrolableArea'));
            if (positionToScrolableArea) {
                $('body,html').scrollTop(positionToScrolableArea);
                sessionStorage.setItem('positionToScrolableArea', 0);
            }
        }

        function fillJobStatusNameList() {
            for (var name in JobStatus) {
                if (JobStatus.hasOwnProperty(name)) {
                    self.jobStatusNameList.push(name);
                }
            }
        }

        function loadJobs() {
            checkSecurityPermission();
            fillJobStatusNameList();
            jobProvider.GetMyJobsHeaderList().then(function (result) {
                self.jobHeaderList = result.data;
                updateTaskCount(self.jobHeaderList);
                $timeout(onTasksPageLoad, 0);
            });
        }

        function updateTaskCount() {
            $scope.$emit('janitorTaskCountChanged', {});
        }

        angular.element(window).on("scroll", function (e) {
            $scope.$apply(function () {
                self.isVisibleButton = window.scrollY > 80;
            });
        });

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.MyTasksPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.MyTasksPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        loadJobs();

        $scope.$on('$destroy', refreshJanitorTasks);
    }

    angular.module('boligdrift').controller(
        'myTasksController',
        ['$timeout',
        '$scope',
        '$state',
        '$filter',
        'jobProvider',
        'managementProvider',
        'memberProvider',
        'janitorService',
        'securityService',
        'urlService',
        myTasksController]);
})();