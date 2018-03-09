(function () {
    var adHocController = function (
        $scope,
        $filter,
        $state,
        $stateParams,
        $rootScope,
        operationalTaskProvider,
        uploadDataProvider,
        weekPlanService,
        memberProvider,
        jobProvider,
        cancellingTemplatesProvider,
        dateHelper,
        moment,
        securityService,
        urlService,
        operationalTaskService) {

        var self = this;
        var MINUTES_IN_HOUR = 60;

        self.save = function (nextState) {
            self.isActiveWaitIcon = true;
            if (validateRequiredFields()) {
                self.adHocTask.creatorId = self.currentUser.memberId;
                self.currentUser.currentRole === Role.Janitor ? createJanitorAdHocTask(nextState) : createCoordinatorAdHocTask();
            } else {
                setTimeout(self.scrollToReguiredField, 0);
            }
            self.isActiveWaitIcon = false;
        }

        self.scrollToReguiredField = function () {
            var errorField = document.querySelector("._error");
            var scrollableArea = $(".modal");
            var lengthToScroll = scrollableArea.scrollTop() + $(errorField).offset().top;

            scrollableArea.animate({
                scrollTop: lengthToScroll
            }, 'slow');
            return false;
        }

        self.deleteDocument = function (document) {
            uploadDataProvider.delete(document.fileId, JobType.AdHoc).then(function() {
                var index = self.adHocTask.uploads.map(function (upload) { return upload.fileId; }).indexOf(document.fileId);
                self.adHocTask.uploads.splice(index, 1);
            });
        }

        self.getFormatedCreationDate = function (dateString) {
            return dateHelper.getLocalDateString(dateString);
        }

        self.assignToMe = function () {
            self.adHocTask.teamLeadId = self.currentUser.memberId;
            self.save(State.JanitorTaskDetails);
        }

        self.openTask = function () {
            self.adHocTask.groupId = null;
            self.adHocTask.teamLeadId = null;
            self.adHocTask.userIdList = [];
            self.adHocTask.isAssignedToAllUsers = true;
            self.save(State.JanitorMyTasks);
        }

        self.cancel = function () {
            var msg = $filter('translate')('Are you sure to cancel task?');
            if (confirm(msg)) {

                var model = { dayAssignId: getDayAssignId(), cancellationReasonId: self.cancellationReasonId };
                
                jobProvider.DayAssignCancel(model).then(function () {
                    $state.go(State.WeekPlanGridView, { showDisabled: $stateParams.showDisabled, isEditJobPopupClosed: true });
                });
            }
        }

        function createJanitorAdHocTask(nextState) {
            self.adHocTask.departmentId = self.selectedDepartment.id;

            operationalTaskProvider.createJanitorAdHocTask(self.adHocTask).then(function (result) {
                self.adHocTask = result.data;
                saveUploads();
                $state.go(nextState, { id: self.adHocTask.id, dayAssignId: self.adHocTask.dayAssignId, previousState: State.JanitorMyTasks });
            });
        }

        function createCoordinatorAdHocTask() {
            self.adHocTask.departmentId = $stateParams.departmentId ? $stateParams.departmentId : self.filter.departmentid;
            operationalTaskProvider.createAdHocTask(self.adHocTask).then(function (result) {
                self.adHocTask = result.data;
                saveUploads();
                $state.go(State.OperationalTaskEditAdHoc,
                    {
                        id: self.adHocTask.id,
                        jobType: JobType.AdHoc,
                        dayAssignId: self.adHocTask.dayAssignId,
                        departmentId: self.adHocTask.departmentId
                    });
            });
        }

        function updateHousingDepartmentHandler(housingDepartment) {
            self.selectedDepartment = housingDepartment;
        }

        function loadTask() {
            operationalTaskProvider.GetAdHocTask(getDayAssignId()).then(function (result) {
                self.adHocTask = result.data;
            });
        }

        function getDayAssignId() {
            return self.adHocTask && self.adHocTask.dayAssignId ? self.adHocTask.dayAssignId : $stateParams.dayAssignId;
        }

        function fillTask() {
            if (self.isTaskSaved) {
                operationalTaskProvider.GetAdHocTask(getDayAssignId()).then(function (result) {
                    self.adHocTask = result.data;
                    self.addressForMap = self.adHocTask.address;
                    self.isCanceled = self.adHocTask.isCanceled;
                    setDirectives();
                });
            } else {
                self.adHocTask = getDefaultTask();
                setTeamPicker();
            }
        }

        function changeCategory(categoryId) {
            if (self.isTaskSaved) {
                var model = { id: self.adHocTask.id, value: categoryId };
                operationalTaskProvider.ChangeCategory(model);
            } else {
                self.adHocTask.categoryId = categoryId;
                controlHasChanges();
            }
        }

        function changeDate(date) {
            self.teamPickerConfig.timeViewDayScope = dateHelper.parseDateFromDatePickerFormat(date);
            self.teamPickerConfig.triggerRefresh();

            self.adHocTask.date = date;
            controlHasChanges();
        }

        function saveDate(date) {
            self.teamPickerConfig.timeViewDayScope = dateHelper.parseDateFromDatePickerFormat(date);
            self.teamPickerConfig.triggerRefresh();

            var model = { jobId: self.adHocTask.id, dayAssignId: self.adHocTask.dayAssignId, date: date };
            operationalTaskProvider.ChangeTaskDate(model);
        }

        function changeEstimate(estimateInHours) {
            var estimate = estimateInHours * MINUTES_IN_HOUR;
            if (self.isTaskSaved) {
                var model = { id: self.adHocTask.dayAssignId, value: estimate };
                operationalTaskProvider.ChangeEstimate(model);
            } else {
                self.adHocTask.estimate = estimate;
                controlHasChanges();
            }
        }

        function changeTitle(title) {
            if (self.isTaskSaved) {
                var model = { id: self.adHocTask.id, value: title };
                operationalTaskProvider.ChangeTitle(model).then(function () {
                    $rootScope.$broadcast('operationalTaskTitleChanged');
                });
            } else {
                self.adHocTask.title = title;
                controlHasChanges();
            }
        }

        function changeDescription(description) {
            if (self.isTaskSaved) {
                var model = { id: self.adHocTask.id, value: description };
                operationalTaskProvider.ChangeDescription(model);
            } else {
                self.adHocTask.description = description;
                controlHasChanges();
            }
        }

        function changeAddress(address) {
            if (self.isTaskSaved) {
                var model = { id: self.adHocTask.departmentId, value: address };
                jobProvider.SaveAddress(self.adHocTask.id, model);
            } else {
                if (self.adHocTask) {
                    self.adHocTask.address = address;
                    controlHasChanges();
                }
            }
        }

        function changeTeam() {
            var model = {
                dayAssignId: self.adHocTask.dayAssignId,
                groupId: null,
                teamLeadId: null,
                userIdList: [],
                isAssignedToAllUsers: false,
                isUnassignAll: self.teamPickerConfig.isUnassignAll
            }

            if (self.currentUser.currentRole !== Role.Janitor) {
                model.groupId = self.adHocTask.groupId;
                model.teamLeadId = self.adHocTask.teamLeadId;
                model.userIdList = self.adHocTask.userIdList;
                model.isAssignedToAllUsers = self.adHocTask.isAssignedToAllUsers;
            }

            operationalTaskProvider.assignEmployees(model);
        }

        self.onReopen = function () {
            var msg = $filter('translate')('Are you sure you want to reopen this task?');
            if (confirm(msg)) {
                jobProvider.ReopenJob(self.adHocTask.dayAssignId).then(function () {
                    self.isReopened = true;
                });
            }
        }

        function reasonUpdated(reason) {
            if (reason) {
                self.disableTaskCancelButton = false;
                self.cancellationReasonId = reason.id;
            } else {
                self.disableTaskCancelButton = true;
            }
        }

        function getQueryParamsFileUploader() {
            return {
                jobId: self.adHocTask.id,
                departmentId: self.adHocTask.departmentId,
                assignId: self.adHocTask.jobAssignId,
                isOpertionalTask: true,
                uploaderId: self.currentUser.memberId
            };
        };

        function saveUploads() {
            if (typeof self.ordinalFileUploaderConfig.triggerUploadFile === 'function') {
                self.ordinalFileUploaderConfig.triggerUploadFile();
            }
        }

        function setDirectives() {
            setCategoryTreePicker();
            setDatePicker();
            setEstimatePicker();
            setTitlePicker();
            setDescriptionPicker();
            setAddressPicker();
            setTeamPicker();
            setCancellingReason();
            self.isCompleted = self.adHocTask.statusId === JobStatus.Completed;
        }

        function setCategoryTreePicker() {
            self.categoryTreePickerConfig.categoryTreeConfig = { simpleView: true, showTasks: function () { }, showViewAllButton: function () { } };
            self.categoryTreePickerConfig.value = self.adHocTask.categoryId;
            self.categoryTreePickerConfig.triggerRefresh();
        }

        function setDatePicker() {
            self.datePickerConfig.date = dateHelper.getLocalDateString(self.adHocTask.date);
            self.datePickerConfig.triggerRefresh();
        }

        function setEstimatePicker() {
            self.estimatePickerConfig.value = self.adHocTask.estimate / MINUTES_IN_HOUR;
            self.estimatePickerConfig.triggerRefresh();
        }

        function setTitlePicker() {
            self.titlePickerConfig.value = self.adHocTask.title;
            self.titlePickerConfig.triggerRefresh();
            self.isCompleted = self.adHocTask.statusId === JobStatus.Completed;
        }

        function setDescriptionPicker() {
            self.descriptionPickerConfig.value = self.adHocTask.description;
            self.descriptionPickerConfig.triggerRefresh();
        }

        function setAddressPicker() {
            self.addressControlConfig.address = self.adHocTask.address;
            self.addressControlConfig.triggerRefresh();
        }

        function setTeamPicker() {
            self.teamPickerConfig.groupId = self.adHocTask.groupId;
            self.teamPickerConfig.teamLeadId = self.adHocTask.teamLeadId;
            self.teamPickerConfig.isAssignedToAllUsers = self.adHocTask.isAssignedToAllUsers;
            self.teamPickerConfig.userIdList = self.adHocTask.userIdList ? self.adHocTask.userIdList : [];
            self.teamPickerConfig.isOpenedState = self.adHocTask && (self.adHocTask.statusId != JobStatus.Pending && self.adHocTask.statusId != JobStatus.Expired);
            self.teamPickerConfig.timeViewDayScope = self.datePickerConfig.date ?
                dateHelper.parseDateFromDatePickerFormat(self.datePickerConfig.date) :
                null;

            if (isPausedOrInProgressStatus()) {
                self.teamPickerConfig.mode = ControlMode.disable;
            }
            self.teamPickerConfig.triggerRefresh();
        }

        function setCancellingReason() {
            cancellingTemplatesProvider.GetCoordinatorByTaskType(JobType.AdHoc).then(function (result) {
                self.cancellationReasonConfig.dataList = result.data.map(function (reason) {
                    return {
                        id: reason.id,
                        value: reason.text
                    }
                });

                self.cancellationReasonConfig.triggerUpdateDataList();
            });
        }

        function isPausedOrInProgressStatus() {
            return self.adHocTask && (self.adHocTask.statusId === JobStatus.Paused || self.adHocTask.statusId === JobStatus.InProgress);
        }

        function getDefaultTask() {
            return {
                id: '',
                categoryId: '',
                date: moment.utc(),
                estimate: 0,
                title: '',
                description: '',
                address: '',
                creatorId: '',
                departmentId: ''
            }
        }

        function validateRequiredFields() {
            self.isValidateNotRequired = false;
            return isCategoryValid() & isDateValid() & isTitleValid() & isTeamValid() & isDepartmentValid() & isEstimateValid();
        }

        function isCategoryValid() {
            var result = self.isValidateNotRequired || self.categoryTreePickerConfig.triggerValidate();
            return result;
        }

        function isDateValid() {
            var result = self.isValidateNotRequired || self.datePickerConfig.triggerValidate();
            return result;
        }

        function isTitleValid() {
            var result = self.isValidateNotRequired || self.titlePickerConfig.triggerValidate();
            return result;
        }

        function isTeamValid() {
            var result = self.currentUser.currentRole === Role.Janitor || self.isValidateNotRequired || self.teamPickerConfig.triggerValidate();
            return result;
        }

        function isDepartmentValid() {
            var result = self.currentUser.currentRole !== Role.Janitor || self.isValidateNotRequired || self.departmentPickerConfig.triggerValidate();
            return result;
        }

        function isEstimateValid() {
            var result = self.isValidateNotRequired || self.estimatePickerConfig.triggerValidate();
            return result;
        }

        function getOrdinalFileUploaderConfig() {
            var config = new FileUploaderModel();

            config.contentTypes = [ContentType.Document, ContentType.Image, ContentType.Video];
            config.onGetQueryParams = getQueryParamsFileUploader;
            config.onFileAdded = controlHasChanges;
            config.triggerUploadFile = {};
            config.autoSave = false;
            config.contentUrl = OnDemandUploaderView.Documents;
            config.ngFlowConfig = {
                target: '/api/files/upload',
                permanentErrors: [404, 500, 501],
                chunkSize: 9007199254740992,
                maxChunkRetries: 1,
                chunkRetryInterval: 5000,
                singleFile: false,
                testChunks: false
            }

            return config;
        }

        function getUploads() {
            operationalTaskProvider.GetUploads(self.adHocTask.jobAssignId).then(function (result) {
                self.adHocTask.uploads = result.data;
            });
        }

        function getInstantFileUploaderConfig() {
            var config = new FileUploaderModel();

            config.contentTypes = [ContentType.Document, ContentType.Image, ContentType.Video];
            config.onGetQueryParams = getQueryParamsFileUploader;
            config.onSuccess = getUploads;

            return config;
        }

        function teamChangedHandler() {
            self.adHocTask.groupId = self.teamPickerConfig.groupId;
            self.adHocTask.teamLeadId = self.teamPickerConfig.teamLeadId;
            self.adHocTask.userIdList = self.teamPickerConfig.userIdList;
            self.adHocTask.isAssignedToAllUsers = self.teamPickerConfig.isAssignedToAllUsers;
            controlHasChanges();
        }

        function controlHasChanges() {
            self.rootElement.classList.add('js-user-made-changes');
        }

        function initVariables() {
            self.ordinalFileUploaderConfig = getOrdinalFileUploaderConfig();
            self.instantFileUploaderConfig = getInstantFileUploaderConfig();
            self.filter = weekPlanService.getFilter();
            self.isValidateNotRequired = true;
            self.isCanceled = false;
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? operationalTaskService.getPreviousPage($stateParams.redirectState) : null;

            var model = {
                groupName: TabSecurityKey.OperationalTask,
                jobId: $stateParams.id,
                page: page,
                dayAssignId: $stateParams.dayAssignId
            };

            securityService.hasAccessByGroupName(model).then(function (result) {
                manageSecurityPermissions(result.data);
                fillTask();
            });
        }

        function manageSecurityPermissions(securityResult) {
            var isAccessAllowed = securityResult[self.categoryTreePickerConfig.securityKey];
            self.categoryTreePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.categoryTreePickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.datePickerConfig.securityKey];
            self.datePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);;
            self.datePickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.estimatePickerConfig.securityKey];
            self.estimatePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.estimatePickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.titlePickerConfig.securityKey];
            self.titlePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.titlePickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.descriptionPickerConfig.securityKey];
            self.descriptionPickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.descriptionPickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.addressControlConfig.securityKey];
            self.addressControlConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.addressControlConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.teamPickerConfig.securityKey];
            self.teamPickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.teamPickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.departmentPickerConfig.securityKey];
            self.departmentPickerConfig.isControlDisabled = !isAccessAllowed;

            self.showFileUploader = securityResult[ControlSecurityKey.OperationalTaskFileUploader];
            self.showDeleteDocumentButton = securityResult[ControlSecurityKey.OperationalTaskDeleteDocument];
            self.disableTaskCancelControl = !securityResult[ControlSecurityKey.OperationalTaskCancel];
            self.showSaveButton = securityResult[ControlSecurityKey.OperationalTaskSaveButton];
            self.showJanitorButtons = securityResult[ControlSecurityKey.OperationalTaskAssignButton] && securityResult[ControlSecurityKey.OperationalTaskOpenButton];
        }

        function initCategoryTreePicker() {
            self.categoryTreePickerConfig = new CategoryTreePickerModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.categoryTreePickerConfig.onSave = changeCategory;
            self.categoryTreePickerConfig.securityKey = ControlSecurityKey.OperationalTaskCategory;
        }

        function initDatePicker() {
            self.datePickerConfig = new DatePickerModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.datePickerConfig.date = dateHelper.getLocalDateString(new Date());
            self.datePickerConfig.onSave = saveDate;
            self.datePickerConfig.onChange = changeDate;
            self.datePickerConfig.securityKey = ControlSecurityKey.OperationalTaskDate;
        }

        function initEstimatePicker() {
            self.estimatePickerConfig = new TextControlModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.estimatePickerConfig.onSave = changeEstimate;
            self.estimatePickerConfig.value = '';
            self.estimatePickerConfig.inputType = InputType.number;
            self.estimatePickerConfig.securityKey = ControlSecurityKey.OperationalTaskEstimatedTime;
        }

        function initTitlePicker() {
            self.titlePickerConfig = new TextControlModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.titlePickerConfig.inputType = InputType.text;
            self.titlePickerConfig.onSave = changeTitle;
            self.titlePickerConfig.securityKey = ControlSecurityKey.OperationalTaskTitle;
        }

        function initDescriptionPicker() {
            self.descriptionPickerConfig = new TextAreaControlModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.descriptionPickerConfig.isValidationRequired = false;
            self.descriptionPickerConfig.onSave = changeDescription;
            self.descriptionPickerConfig.securityKey = ControlSecurityKey.OperationalTaskDescription;
        }

        function initAddressPicker() {
            self.addressControlConfig = new AddressControlModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.addressControlConfig.onSave = changeAddress;
            self.addressControlConfig.securityKey = ControlSecurityKey.OperationalTaskAddress;
        }

        function initTeamPicker() {
            self.teamPickerConfig = new TeamPickerModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.teamPickerConfig.onChange = teamChangedHandler;
            self.teamPickerConfig.onSave = changeTeam;
            self.teamPickerConfig.timeViewDayScope = null;
            self.teamPickerConfig.securityKey = ControlSecurityKey.OperationalTaskResponsible;
        }

        function initDepartmentPicker() {
            self.departmentPickerConfig = new DepartmentPickerModel();
            self.departmentPickerConfig.onSelect = updateHousingDepartmentHandler;
            self.departmentPickerConfig.onChange = controlHasChanges;
            self.departmentPickerConfig.isControlDisabled = true;
            self.departmentPickerConfig.securityKey = ControlSecurityKey.OperationalTaskHousingDepartment;
        }

        function initCancellationReason() {
            self.cancellationReasonConfig = new SelectControlModel();
            self.cancellationReasonConfig.placeholder = $filter('translate')('Task cancelling Templates');
            self.cancellationReasonConfig.onUpdate = reasonUpdated;
        }

        function initDirectives() {
            initCategoryTreePicker();
            initDatePicker();
            initEstimatePicker();
            initTitlePicker();
            initDescriptionPicker();
            initAddressPicker();
            initTeamPicker();
            initDepartmentPicker();
            initCancellationReason();
        }

        function initControl() {

            checkSecurityPermission();

            self.rootElement = document.getElementById('adHoc-task');
            self.isTaskSaved = $stateParams.id !== undefined;
            self.disableTaskCancelControl = true;
            self.disableTaskCancelButton = true;

            memberProvider.GetCurrentUser().then(function (result) {
                self.currentUser = result.data;
                self.isAllowAssignMembers = self.currentUser.currentRole !== Role.Janitor;
                self.isAllowChooseDepartment = self.currentUser.currentRole === Role.Janitor;
            });

            initDirectives();
            initVariables();
            initSecurityRules();
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.AdHockTaskPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.AdHockTaskPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    }

    angular.module('boligdrift').controller('adHocController',
        [
            '$scope',
            '$filter',
            '$state',
            '$stateParams',
            '$rootScope',
            'operationalTaskProvider',
            'uploadDataProvider',
            'weekPlanService',
            'memberProvider',
            'jobProvider',
            'cancellingTemplatesProvider',
            'dateHelper',
            'moment',
            'securityService',
            'urlService',
            'operationalTaskService',
            adHocController
        ]);
})();