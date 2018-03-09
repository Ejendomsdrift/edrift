(function () {
    var editController = function ($state, $stateParams, $timeout, $scope, operationalTaskProvider, jobProvider, weekPlanService, dateHelper, securityService, urlService) {
        var self = this;

        self.state = State;
        self.taskId = $stateParams.id;

        self.redirect = function () {
            weekPlanService.clearStoredDayAssign();
            $timeout(function () {
                $state.go($stateParams.redirectState, {
                    showDisabled: $stateParams.showDisabled,
                    showAll: $stateParams.showAll,
                    isEditJobPopupClosed: true,
                    isShowCategoryPanel: true,
                    listViewCurrentTab: $stateParams.listViewCurrentTab
                });
            });
        }

        self.getFormattedDateString = function () {
            if (self.taskCreationInfo) {
                return dateHelper.getLocalDateString(self.taskCreationInfo.creationDate);
            }
        }

        function getTaskCreationInfo() {
            operationalTaskProvider.getTaskCreationInfo(self.taskId).then(function (result) {
                self.taskCreationInfo = result.data;
                self.title = result.data.title;
            });
        }

        function isGroupedTask() {
            self.isChildGroupedTask = false;
            jobProvider.IsChildGroupedTask($stateParams.id).then(function (result) {
                self.isChildGroupedTask = result.data;
            });
        }

        function getPreviousState() {
            if ($stateParams.redirectState.indexOf(Pages.YearPlan) != -1) {
                return Pages.YearPlan;
            } else if ($stateParams.redirectState.indexOf(Pages.WeekPlan) != -1) {
                return Pages.WeekPlan;
            }
        }

        var titleListener = $scope.$on('facilityTaskTitleChanged', getTaskCreationInfo);
        $scope.$on('$destroy', function () {
            titleListener();
        });

        function initControl() {
            checkSecurityPermission();

            self.page = getPreviousState();
            getTaskCreationInfo();
            isGroupedTask();
            self.isTaskOpenedFromWeekPlan = $stateParams.redirectState.indexOf(State.WeekPlan) != -1;
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

    angular.module('boligdrift').controller('editController',
        [
            '$state',
            '$stateParams',
            '$timeout',
            '$scope',
            'operationalTaskProvider',
            'jobProvider',
            'weekPlanService',
            'dateHelper',
            'securityService',
            'urlService',
            editController
        ]);
})();
