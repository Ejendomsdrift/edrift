(function() {
    var completedTasksController = function ($scope, $state, $filter, jobProvider, janitorService) {
        var self = this;
        self.jobStatusNameList = [];

        var refreshCompletedTasks = $scope.$on('refreshCompletedTasks', function () {
            loadJobs();
        });

        self.goToTaskDetails = function (jobDetails) {
            $state.go(State.JanitorTaskDetails, { id: jobDetails.name, dayAssignId: jobDetails.id, previousState: $state.current.name });
        }

        self.getDateString = function (dateString) {
            return janitorService.getFormatedDateString(dateString);
        }

        self.scrollToTop = function () {
            janitorService.scrollToTop();
        }

        self.getStatusTranslation = function (status) {
            return JobStatusPlatformKey.JanitorPlatform + self.jobStatusNameList[status];
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
            jobProvider.GetClosedJobsHeaderList().then(function (result) {
                self.jobHeaderList = result.data;
            });
        }

        loadJobs();

        $scope.$on('$destroy', refreshCompletedTasks);
    };

    angular.module('boligdrift').controller('completedTasksController', ['$scope', '$state', '$filter', 'jobProvider', 'janitorService', completedTasksController]);
})();