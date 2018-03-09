(function () {
    var documentController = function (
        $filter,
        $stateParams,
        $state,
        uploadDataProvider,
        jobProvider,
        memberProvider,
        yearPlanService,
        weekPlanService,
        securityService,
        dateHelper,
        urlService) {

        var self = this;

        var confirmMessage = $filter('translate')('Are you sure?');

        var loadJob = function () {
            self.showGlobalSection = true;
            self.showLocalSection = true;
            self.hideBothSection = false;

            self.jobId = $stateParams.id;

            jobProvider.GetJobAssigns(self.jobId, ContentType.Document).then(function (result) {
                self.facilityTask = result.data;
                self.assignedDepartments = result.data.assignedDepartments;
                self.globalAssign = self.facilityTask.globalAssign;
                self.isGroupedJob = result.data.isGroupedJob;
                self.addressList = result.data.addressList;
                self.assigns = self.facilityTask.assigns;
                self.currentUser = result.data.currentUser;

                self.selectedLocalDepartmentAssign = getJobAssign(self.localDepartment);

                if (self.isGroupedJob) {
                    self.localJobAddressOnChange();
                }

                if (result.data.isChildJob) {
                    self.showGlobalSection = self.globalAssign.uploadList.length > 0;
                }

                if (self.isRedirectStateWeekPlan) {
                    initDocumentsSectionShowFlags();
                }
            });
        };

        self.deleteGlobalDocument = function (document) {
            if (confirm(confirmMessage)) {
                uploadDataProvider.delete(document.fileId, JobType.Facility, self.globalAssign.id).then(loadJob);
            }
        }

        self.deleteLocalDocument = function (document) {
            if (confirm(confirmMessage)) {
                uploadDataProvider.delete(document.fileId, JobType.Facility, self.selectedLocalDepartmentAssign.id).then(loadJob);
            }
        }

        self.changeLocalDepartment = function (department) {
            self.localDepartment = department;
            self.selectedLocalDepartmentAssign = getJobAssign(department);
        }

        self.localJobAddressOnChange = function () {
            if (!self.localJobAddress) {
                self.selectedLocalDepartmentAssign = null;
                self.localAssign = null;
                self.isAdressChoosen = false;
                return;
            }

            self.selectedLocalDepartmentAssign = self.assigns.find(function (assign) {
                return assign.jobIdList.indexOf(self.localJobAddress.id) !== -1;
            });

            self.localAssign = self.selectedLocalDepartmentAssign;
            self.isAdressChoosen = true;
        }

        self.getFormatedCreationDate = function (dateString) {
            return dateHelper.getLocalDateString(dateString);
        }

        self.showTable = function (uploadList) {
            return uploadList ? uploadList.length > 0 : false;
        }

        self.showLocalLabel = function () {
            if (self.isRedirectStateWeekPlan && (self.localDepartment || self.localJobAddress)) { 
                return self.selectedLocalDepartmentAssign ? !self.showTable(self.selectedLocalDepartmentAssign.uploadList) : true;
            }
        }

        function initDocumentsSectionShowFlags() {
            if (self.globalAssign) {
                self.showGlobalSection = self.globalAssign.uploadList.length > 0 || !self.isGroupedJob;
            }

            self.showLocalSection = checkLocalAssigns();
            self.hideBothSection = self.showGlobalSection === false && self.showLocalSection === false;
        }

        function checkLocalAssigns() {
            if (self.assigns) {
                for (var i = 0; i < self.assigns.length; i++) {
                    if (self.assigns[i].uploadList.length > 0) {
                        return true;
                    }
                }
            }

            return false;
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.Document,
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

        function manageSecurityPermissions(securityResult) {
            self.showGlobalFileUploader = securityResult[ControlSecurityKey.DocumentGlobalDocument];
            self.showLocalFileUploader = securityResult[ControlSecurityKey.DocumentLocalDocument];

            if (isAdministration()) {
                // we need provide all permissions to admin for editing local sections, for grouped tasks which were created by coordinator 
                self.showLocalFileUploader = true;
            }
        }

        function isAdministration() {
            if (self.currentUser) {
                return self.currentUser.currentRole == Role.Administrator || self.currentUser.currentRole == Role.SuperAdmin;
            }
        }

        function initFileUploaderConfig() {
            initGlobalFileUploaderConfig();
            initLocalFileUploaderConfig();
        }

        function initGlobalFileUploaderConfig() {
            self.globalFileUploaderConfig = new FileUploaderModel();

            self.globalFileUploaderConfig.contentTypes = [ContentType.Document];
            self.globalFileUploaderConfig.onSuccess = loadJob;
            self.globalFileUploaderConfig.onGetQueryParams = getGlobalQueryParams;
        }

        function initLocalFileUploaderConfig() {
            self.localFileUploaderConfig = new FileUploaderModel();

            self.localFileUploaderConfig.contentTypes = [ContentType.Document];
            self.localFileUploaderConfig.onSuccess = loadJob;
            self.localFileUploaderConfig.onGetQueryParams = getLocalQueryParams;
        }

        function getJobAssign(department) {
            return self.localAssign = department ? self.assigns.filter(function (d) { return d.housingDepartmentIdList.indexOf(department.id) >= 0 })[0] : null;
        }

        function getDepartmentId() {
            if (self.isGroupedJob) {
                return $stateParams.department ? $stateParams.department : self.filter.departmentid;
            }

            return self.localDepartment ? self.localDepartment.id : null;
        }

        function getGlobalQueryParams() {
            return {
                jobId: self.jobId,
                isLocalChanged: false,
                assignId: self.globalAssign.id,
                departmentId: null,
                uploaderId: self.currentUser.memberId
            };
        };

        function getLocalQueryParams() {
            return {
                jobId: self.isGroupedJob ? self.localJobAddress.id : self.jobId,
                isLocalChanged: true,
                assignId: self.selectedLocalDepartmentAssign ? self.selectedLocalDepartmentAssign.id : null,
                departmentId: getDepartmentId(),
                uploaderId: self.currentUser.memberId
            };
        };

        function initControl() {

            checkSecurityPermission();

            if (getPreviousState() == Pages.YearPlan) {
                self.filter = yearPlanService.getFilter();
            } else {
                self.filter = weekPlanService.getFilter();
            }

            self.selectedLocalDepartmentAssign = null;
            self.showGlobalFileUploader = false;
            self.showLocalFileUploader = true;
            self.isRedirectStateWeekPlan = $stateParams.redirectState.indexOf(Pages.WeekPlan) != -1;

            loadJob();
            initFileUploaderConfig();
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

    angular.module('boligdrift').controller('documentController',
        [
            '$filter',
            '$stateParams',
            '$state',
            'uploadDataProvider',
            'jobProvider',
            'memberProvider',
            'yearPlanService',
            'weekPlanService',
            'securityService',
            'dateHelper',
            'urlService',
            documentController
        ]);
})();
