(function () {
    var locationController = function ($stateParams, jobProvider, securityService, yearPlanService, weekPlanService, urlService) {
        var self = this;

        self.showSelector = $stateParams.redirectState === State.YearPlanTaskOverview;

        self.updateAddress = function () {
            var jobId = getJobId();
            var departmentId = getDepartmentId();

            var address = null;
            if (jobId && departmentId && self.locationModel.addressList[jobId]) {
                address = self.locationModel.addressList[jobId][departmentId];
            }

            self.addressControlConfig.address = address;
            self.addressControlConfig.triggerRefresh();
        }

        function saveHandler() {
            var jobId = getJobId();
            var departmentId = getDepartmentId();
            var model = { id: departmentId, value: self.addressControlConfig.address };

            jobProvider.SaveAddress(jobId, model).then(function () {
                if (!self.locationModel.addressList[jobId]) {
                    self.locationModel.addressList[jobId] = [];
                }

                self.locationModel.addressList[jobId][departmentId] = self.addressControlConfig.address;
                if (self.locationModel.isGroupedJob) {
                    self.selectedGroupedJob.value = self.addressControlConfig.address;
                }
            });
        }

        function loadLocationModel() {
            var housingDepartmentId = $stateParams.department ? $stateParams.department : self.filter.departmentid;
            jobProvider.GetLocationModel($stateParams.id, housingDepartmentId).then(function (result) {
                self.locationModel = result.data;

                if (!self.showSelector) {
                    self.updateAddress();
                }
                else if (self.locationModel.isGroupedJob) {
                    self.selectedGroupedJob = self.locationModel.groupedTasks.find(function (i) { return i.id == $stateParams.id; });
                    self.updateAddress();
                }
            });
        }

        function getJobId() {
            if (!self.showSelector || !self.locationModel.isGroupedJob) {
                return $stateParams.id;
            }
            return self.selectedGroupedJob ? self.selectedGroupedJob.id : null;
        }

        function getDepartmentId() {
            if (!self.showSelector) {
                return $stateParams.department;
            } else if (self.locationModel.isGroupedJob) {
                return $stateParams.department ? $stateParams.department : self.filter.departmentid;
            }

            return self.selectedDepartment ? self.selectedDepartment.id : null;
        }

        function manageSecurityPermissions(securityResult) {
            var mode = securityResult[self.addressControlConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.addressControlConfig.mode = mode;
            self.addressControlConfig.triggerRefresh();
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.Location,
                jobId: $stateParams.id,
                page: page,
                dayAssignId: $stateParams.dayAssignId
            };

            securityService.hasAccessByGroupName(model).then(function (result) {
                manageSecurityPermissions(result.data);
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
            self.addressControlConfig = new AddressControlModel();
            self.addressControlConfig.onSave = saveHandler;
            self.addressControlConfig.securityKey = ControlSecurityKey.LocationAddress;
        }

        function initControl() {

            checkSecurityPermission();

            if (getPreviousState() == Pages.YearPlan) {
                self.filter = yearPlanService.getFilter();
            } else {
                self.filter = weekPlanService.getFilter();
            }

            loadLocationModel();
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

    angular.module('boligdrift').controller('locationController',
        [
            '$stateParams',
            'jobProvider',
            'securityService',
            'yearPlanService',
            'weekPlanService',
            'urlService',
            locationController
        ]);
})();
