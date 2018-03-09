(function () {
    var operatingTaskController = function ($state, $stateParams, $filter, $scope, operationalTaskProvider, dateHelper, memberProvider, departmentProvider, weekPlanService) {
        var self = this;
        self.state = State;
        self.taskId = $stateParams.id;
        self.showHousingDepartment = false;
        var currentJobType = $stateParams.jobType;

        self.isTaskSaved = function() {
            return self.taskId != undefined;
        }

        self.getFormattedDateString = function() {
            if (self.taskCreationInfo) {
                return dateHelper.getLocalDateString(self.taskCreationInfo.creationDate);
            }
        }

        self.redirect = function () {
            if (confirmRedirect()) {
                $state.go(self.redirectState, { isEditJobPopupClosed: true, listViewCurrentTab: $stateParams.listViewCurrentTab });
            }
        }

        self.redirectToAdhoc = function () {
            if (confirmRedirect()) {
                currentJobType = JobType.AdHoc;
                $state.go(State.OperationalTaskCreateAdHoc, { redirectState: self.redirectState, jobType: JobType.AdHoc, departmentId: $stateParams.departmentId });
            }
        }

        self.redirectToTenant = function () {
            if (confirmRedirect()) {
                currentJobType = JobType.Tenant;
                $state.go(State.OperationalTaskCreateTenant, { redirectState: self.redirectState, jobType: JobType.Tenant, departmentId: $stateParams.departmentId });
            }
        }

        self.redirectToOtherTypeTask = function () {
            if (confirmRedirect()) {
                currentJobType = JobType.Other;
                $state.go(State.OperationalTaskCreateOther, { redirectState: self.redirectState, jobType: JobType.Other, departmentId: $stateParams.departmentId });
            }
        }

        self.isTenant = function() {
            return $stateParams.jobType === JobType.Tenant;
        }

        self.isAdHoc = function () {
            return $stateParams.jobType === JobType.AdHoc;
        }

        self.isOther = function() {
            return $stateParams.jobType === JobType.Other;
        }

        self.redirectToEditMode = function () {
            if (self.isAdHoc()) {
                redirectToAdHocEditMode();
            } else if (self.isTenant()) {
                redirectToTenantEditMode();
            } else if (self.isOther()) {
                redirectToOtherEditMode();
            }
        }

        self.redirectToHistory = function () {
            if (self.isAdHoc()) {
                redirectToAdHocHistoryTab();
            } else if (self.isTenant()) {
                redirectToTenantHistoryTab();
            } else if (self.isOther()) {
                redirectToOtherHistoryTab();
            }
        }

        self.isEditMode = function() {
            return $state.current.name.indexOf(Pages.History.toLowerCase()) === -1;
        }

        self.isHistoryMode = function () {
            return $state.current.name.indexOf(Pages.History.toLowerCase()) !== -1;
        }

        function confirmRedirect() {
            var changedElements = document.querySelectorAll('.js-user-made-changes');
            if (changedElements.length) {
                var msg = $filter('translate')('Are you sure?');
                if (!self.taskId && !confirm(msg)) {
                    return false;
                }
            }

            return true;
        }

        function redirectToAdHocEditMode() {
            $state.go(State.OperationalTaskEditAdHoc,
            {
                id: $stateParams.id,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                jobType: JobType.AdHoc,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function redirectToTenantEditMode() {
            $state.go(State.OperationalTaskEditTenant,
            {
                id: $stateParams.id,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                jobType: JobType.Tenant,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function redirectToOtherEditMode() {
            $state.go(State.OperationalTaskEditOther,
            {
                id: $stateParams.id,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                jobType: JobType.Other,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function redirectToAdHocHistoryTab() {
            $state.go(State.OperationalTaskHistoryAdHoc,
            {
                id: $stateParams.id,
                jobType: JobType.AdHoc,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function redirectToTenantHistoryTab() {
            $state.go(State.OperationalTaskHistoryTenant,
            {
                id: $stateParams.id,
                jobType: JobType.Tenant,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function redirectToOtherHistoryTab() {
            $state.go(State.OperationalTaskHistoryOther,
            {
                id: $stateParams.id,
                jobType: JobType.Other,
                dayAssignId: $stateParams.dayAssignId,
                departmentId: $stateParams.departmentId,
                redirectState: self.redirectState,
                listViewCurrentTab: $stateParams.listViewCurrentTab
            });
        }

        function getCurrentUserContext() {
            memberProvider.GetCurrentUserContext().then(function (result) {
                self.currentUserContext = result.data;
                self.currentUser = result.data.memberModel;
                getDepartmentNameForOperationalJob();
            });
        }

        function getTaskCreationInfo() {
            if (self.taskId) {
                operationalTaskProvider.getTaskCreationInfo(self.taskId).then(function (result) {
                    self.taskCreationInfo = result.data;
                    self.isTaskCanceled = result.data.isTaskCanceled;
                    self.isUrgent = result.data.isUrgent;
                    self.title = result.data.title;
                });
            }
        }

        var titleListener = $scope.$on('operationalTaskTitleChanged', getTaskCreationInfo);
        $scope.$on('$destroy', function () {
            titleListener();
        });

        function getDepartmentNameForOperationalJob() {
            self.currentHousingDepartmentDisplayName = null;

            if (self.currentUser.currentRole === Role.Coordinator) {
                departmentProvider.getDepartmentInfoById($stateParams.departmentId).then(function (result) {
                    var currentHousingDepartmentName = result.data;
                    self.currentHousingDepartmentDisplayName = currentHousingDepartmentName.displayName;                    
                });   
            }
        }

        function initControl() {
            self.filter = weekPlanService.getFilter();
            self.redirectState = $stateParams.redirectState;
            getCurrentUserContext();
            getTaskCreationInfo();
            self.isTaskOpenedFromHistoryPage = $stateParams.redirectState && $stateParams.redirectState.indexOf(State.History) > -1;
        }

        initControl();
    };

    angular.module('boligdrift').controller('operatingTaskController',
        [
            '$state',
            '$stateParams',
            '$filter',
            '$scope',
            'operationalTaskProvider',
            'dateHelper',
            'memberProvider',
            'departmentProvider',
            'weekPlanService',
            operatingTaskController
        ]);
})();