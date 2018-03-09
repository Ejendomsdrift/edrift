var EditTaskAssignController = function ($stateParams, $state, jobProvider, securityService, urlService) {
    var self = this;

    var loadTask = function () {
        jobProvider.GetJobDepartments($stateParams.id).then(function (result) {
            self.assignDepartments = result.data.assignedDepartments;
            self.assignDepartmentPickerConfig.value = self.assignDepartments;
            self.assignDepartmentPickerConfig.hidden = result.data.groupedDepartments;
            self.isAssignsDataResived = true;
            self.assignDepartmentPickerConfig.triggerRefresh();
            self.assignDepartments.sort(function (a, b) {
                return a.name > b.name;
            });
        });
    };

    function saveAssigns(assignedDepartmentIds, unassignedDepartmentIds) {
        var model = {
            jobId: $stateParams.id,
            assignedDepartmentIds: assignedDepartmentIds,
            unassignedDepartmentIds: unassignedDepartmentIds
        };

        jobProvider.Assign(model).then(loadTask);
    }

    function manageSecurityPermissions(securityResult) {
        var mode = securityResult[self.assignDepartmentPickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
        self.assignDepartmentPickerConfig.mode = mode;
        self.assignDepartmentPickerConfig.triggerRefresh();
    }

    function initSecurityRules() {
        var page = $stateParams.redirectState ? getPreviousState() : null;

        var model = {
            groupName: TabSecurityKey.Assign,
            jobId: $stateParams.id,
            page: page,
            dayAssignId: $stateParams.dayAssignId
        };

        securityService.hasAccessByGroupName(model).then(function (result) {
            var securtityRules = result.data;
            jobProvider.IsChildGroupedTask($stateParams.id).then(function (result) {
                if (!result.data) {
                    manageSecurityPermissions(securtityRules);
                }
            });
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
        self.assignDepartmentPickerConfig = new AssignDepartmentPickerModel();
        self.assignDepartmentPickerConfig.onSave = saveAssigns;
        self.assignDepartmentPickerConfig.securityKey = ControlSecurityKey.AssignDepartments;
    }

    function initControl() {

        checkSecurityPermission();

        self.assignDepartments = [];

        initDirectives();
        initSecurityRules();
        loadTask();
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

angular.module('boligdrift')
    .controller('editTaskAssignController', ['$stateParams', '$state', 'jobProvider', 'securityService', 'urlService', EditTaskAssignController]);
