(function () {
    var responsibleController = function ($state, $stateParams, jobProvider, securityService, urlService) {
        var self = this;

        var MINUTES_IN_HOUR = 60;

        self.estimate = 60;

        self.selectDepartment = function() {
            self.showControls = self.selectedDepartment ? true : false;
            fillEstimate();
            fillTeam();
        };

        self.showControls = false;

        function initDirectives() {
            initDepartments();
            initEstimate();
            initTeamPicker();
        };


        function initTeamPicker() {
            self.teamPickerConfig = new TeamPickerModel();
            self.teamPickerConfig.securityKey = ControlSecurityKey.AssignTeam;
            self.teamPickerConfig.isRequired = false;
            self.teamPickerConfig.onSave = saveTeam;
        }

        function saveTeam(team) {
            var model = {
                JobId: $stateParams.id,
                HousingDepartmentId: self.selectedDepartment.id,
                IsAssignedToAllUsers: team.isAssignedToAllUsers,
                UserIdList: team.userIdList,
                GroupId: team.groupId,
                teamLeadId: team.teamLeadId
            };

            jobProvider.ChangeJobAssignTeam(model);
        };

        function initEstimate() {
            self.estimateConfig = new TextControlModel();
            self.estimateConfig.inputType = InputType.number;
            self.estimateConfig.isValidationRequired = false;
            self.estimateConfig.value = 0;
            self.estimateConfig.onSave = changeEstimate;
            self.estimateConfig.securityKey = ControlSecurityKey.AssignEstimate;
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.Responsible,
                jobId: $stateParams.id,
                page: page
            }

            securityService.hasAccessByGroupName(model).then(function (result) {
                manageSecurityPermissions(result.data);
            });
        };

        function manageSecurityPermissions(securityResult) {
            var mode = securityResult[self.estimateConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.estimateConfig.mode = mode;

            mode = securityResult[self.teamPickerConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.teamPickerConfig.mode = mode;
        };

        function getPreviousState() {
            if ($stateParams.redirectState.indexOf(Pages.YearPlan) !== -1) {
                return Pages.YearPlan;
            } else if ($stateParams.redirectState.indexOf(Pages.WeekPlan) !== -1) {
                return Pages.WeekPlan;
            }
        }

        function fillEstimate() {
            jobProvider.GetJobAssignEstimate($stateParams.id, self.selectedDepartment.id).then(function(result) {
                self.estimateConfig.value = result.data / 60;
                self.estimateConfig.triggerRefresh();
            });
        }

        function fillTeam() {
            jobProvider.GetJobAssignTeam($stateParams.id, self.selectedDepartment.id).then(function(result) {
                if (result.data) {
                    self.teamPickerConfig.groupId = result.data.groupId;
                    self.teamPickerConfig.teamLeadId = result.data.teamLeadId;
                    self.teamPickerConfig.isAssignedToAllUsers = result.data.isAssignedToAllUsers;
                    self.teamPickerConfig.userIdList = result.data.userIdList ? result.data.userIdList : [];
                    self.teamPickerConfig.isOpenedState = false;
                    self.teamPickerConfig.triggerRefresh();
                }
            });
        }

        function changeEstimate(estimateInHours) {
            var estimateInMinutes = estimateInHours * MINUTES_IN_HOUR;
            
            var model = {
                JobId: $stateParams.id,
                EstimateInMinutes: estimateInMinutes,
                HousingDepartmentId: self.selectedDepartment.id
            }
            
            jobProvider.ChangeJobAssignEstimate(model);
        }

        function initDepartments() {
            getDepartments();
        }

        function getDepartments() {
            jobProvider.GetLocationModel($stateParams.id).then(function(result) {
                self.departments = result.data.departments;
            });
        }

        function initControl() {
            checkSecurityPermission();
            initDirectives();
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

    angular.module('boligdrift').controller('responsibleController',
        [
            '$state',
            '$stateParams',
            'jobProvider',
            'securityService',
            'urlService',
            responsibleController
        ]);
})();