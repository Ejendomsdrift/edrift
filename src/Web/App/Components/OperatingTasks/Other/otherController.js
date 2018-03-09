(function () {
    var otherController = function (
        $filter,
        $scope,
        $state,
        $stateParams,
        $rootScope,
        memberProvider,
        uploadDataProvider,
        operationalTaskProvider,
        weekPlanService,
        jobProvider,
        cancellingTemplatesProvider,
        moment,
        dateHelper,
        securityService,
        urlService,
        operationalTaskService) {

        var self = this;
        var MINUTES_IN_HOUR = 60;

        self.save = function (nextState) {
            self.isActiveWaitIcon = true;
            if (validateRequiredFields()) {
                self.task.creatorId = self.currentUser.memberId;
                self.currentUser.currentRole === Role.Janitor ? createJanitorOtherTask(nextState) : createCoordinatorOtherTask();
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

        self.cancel = function () {
            var msg = $filter('translate')('Are you sure to cancel task?');

            if (confirm(msg)) {
                var model = { dayAssignId: self.task.dayAssignId, cancellationReasonId: self.cancellationReasonId };

                jobProvider.DayAssignCancel(model).then(function () {
                    $state.go(State.WeekPlanGridView, { showDisabled: $stateParams.showDisabled, isEditJobPopupClosed: true });
                });
            }
        }

        self.onReopen = function () {
            var msg = $filter('translate')('Are you sure you want to reopen this task?');
            if (confirm(msg)) {
                jobProvider.ReopenJob(self.task.dayAssignId).then(function () {
                    self.isReopened = true;
                });
            }
        }

        self.getFormatedCreationDate = function (dateString) {
            return dateHelper.getLocalDateString(dateString);
        }

        self.deleteDocument = function (document) {
            uploadDataProvider.delete(document.fileId, JobType.Other).then(function() {
                var index = self.task.uploads.map(function (upload) { return upload.fileId; }).indexOf(document.fileId);
                self.task.uploads.splice(index, 1);
            });
        }

        self.assignToMe = function () {
            self.task.teamLeadId = self.currentUser.memberId;
            self.save(State.JanitorTaskDetails);
        }

        self.openTask = function () {
            self.task.groupId = null;
            self.task.teamLeadId = null;
            self.task.userIdList = [];
            self.task.isAssignedToAllUsers = true;
            self.save(State.JanitorMyTasks);
        }

        function createJanitorOtherTask(nextState) {
            self.task.departmentId = self.selectedDepartment.id;

            operationalTaskProvider.createJanitorOtherTask(self.task).then(function (result) {
                self.task = result.data;
                saveUploads();
                self.teamPickerConfig.triggerCopySavedData();
                $state.go(nextState, { id: result.data.id, dayAssignId: result.data.dayAssignId, previousState: State.JanitorMyTasks });
            });
        }

        function createCoordinatorOtherTask() {
            self.task.departmentId = $stateParams.departmentId ? $stateParams.departmentId : self.filter.departmentid;
            operationalTaskProvider.createOtherTask(self.task).then(function (result) {
                self.task = result.data;
                saveUploads();
                self.teamPickerConfig.triggerCopySavedData();
                $state.go(State.OperationalTaskEditOther,
                    {
                        id: result.data.id,
                        jobType: JobType.Other,
                        dayAssignId: result.data.dayAssignId,
                        departmentId: self.task.departmentId
                    });
            });
        }

        function getDayAssignId() {
            return self.task && self.task.dayAssignId ? self.task.dayAssignId : $stateParams.dayAssignId;
        }

        function validateRequiredFields() {
            self.isValidateNotRequired = false;
            return isDateValid() & isTimeValid() & isTitleValid() & isTeamValid() & isDepartmentValid() & isEstimateValid();
        }

        function isDateValid() {
            return self.isValidateNotRequired || self.datePickerConfig.triggerValidate();
        }

        function isTimeValid() {
            return self.isValidateNotRequired || validateDateAndTime();
        }

        function validateDateAndTime() {
            var selectedDate = self.datePickerConfig.getDate().local();
            var selectedTime = self.timePickerConfig.getTime().local();
            var dateInMilliseconds = dateHelper.getTotalMillisecondsFromDateAndTime(selectedDate, selectedTime);
            return self.timePickerConfig.triggerValidate(dateInMilliseconds);
        }

        function isTitleValid() {
            return self.isValidateNotRequired || self.titlePickerConfig.triggerValidate();
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

        function controlHasChanges() {
            self.rootElement.classList.add('js-user-made-changes');
        }

        function changeDate(date) {
            self.teamPickerConfig.timeViewDayScope = dateHelper.parseDateFromDatePickerFormat(date);
            self.teamPickerConfig.triggerRefresh();

            self.task.date = date;
            controlHasChanges();
        }

        function saveDate(date) {
            self.teamPickerConfig.timeViewDayScope = dateHelper.parseDateFromDatePickerFormat(date);
            self.teamPickerConfig.triggerRefresh();

            var model = { jobId: self.task.id, dayAssignId: self.task.dayAssignId, date: date };
            operationalTaskProvider.ChangeTaskDate(model);
        }

        function changeTime(time) {
            if (self.isTaskSaved) {
                if (validateDateAndTime()) {
                    var model = { id: self.task.dayAssignId, value: time };
                    operationalTaskProvider.ChangeTaskTime(model);
                }
            } else {
                self.task.time = time;
                controlHasChanges();
            }
        }

        function changeEstimate(estimateInHours) {
            var estimate = estimateInHours * MINUTES_IN_HOUR;
            if (self.isTaskSaved) {
                var model = { id: self.task.dayAssignId, value: estimate };
                operationalTaskProvider.ChangeEstimate(model);
            } else {
                self.task.estimate = estimate;
                controlHasChanges();
            }
        }

        function changeTitle(title) {
            if (self.isTaskSaved) {
                var model = { id: self.task.id, value: title };
                operationalTaskProvider.ChangeTitle(model).then(function () {
                    $rootScope.$broadcast('operationalTaskTitleChanged');
                });
            } else {
                self.task.title = title;
                controlHasChanges();
            }
        }

        function changeDescription(description) {
            if (self.isTaskSaved) {
                var model = { id: self.task.id, value: description };
                operationalTaskProvider.ChangeDescription(model);
            } else {
                self.task.description = description;
                controlHasChanges();
            }
        }

        function changeAddress(address) {
            if (self.isTaskSaved) {
                var model = { id: self.task.departmentId, value: address };
                jobProvider.SaveAddress(self.task.id, model);
            } else {
                if (self.task) {
                    self.task.address = address;
                    controlHasChanges();
                }
            }
        }

        function changeTeam() {
            var model = {
                dayAssignId: self.task.dayAssignId,
                groupId: null,
                teamLeadId: null,
                userIdList: [],
                isAssignedToAllUsers: false,
                isUnassignAll: self.teamPickerConfig.isUnassignAll
            }

            if (self.currentUser.currentRole !== Role.Janitor) {
                model.groupId = self.task.groupId;
                model.teamLeadId = self.task.teamLeadId;
                model.userIdList = self.task.userIdList;
                model.isAssignedToAllUsers = self.task.isAssignedToAllUsers;
            }

            operationalTaskProvider.assignEmployees(model);
        }

        function teamChangedHandler() {
            self.task.groupId = self.teamPickerConfig.groupId;
            self.task.teamLeadId = self.teamPickerConfig.teamLeadId;
            self.task.userIdList = self.teamPickerConfig.userIdList;
            self.task.isAssignedToAllUsers = self.teamPickerConfig.isAssignedToAllUsers;
            controlHasChanges();
        }

        function saveUploads() {
            if (typeof self.ordinalFileUploaderConfig.triggerUploadFile === 'function') {
                self.ordinalFileUploaderConfig.triggerUploadFile();
            }
        }

        function fillTask() {
            if (self.isTaskSaved) {
                operationalTaskProvider.GetOtherTask(getDayAssignId()).then(function (result) {
                    self.task = result.data;
                    self.isCompleted = self.task.statusId === JobStatus.Completed;
                    setDirectives();
                });
            } else {
                self.task = getDefaultTask();
                setDatePicker();
                setTeamPicker();
            }
        }

        function setDirectives() {
            setDatePicker();
            setTimePicker();
            setEstimatePicker();
            setTitlePicker();
            setDescriptionPicker();
            setAddressPicker();
            setTeamPicker();
            setCancellingReason();
        }

        function setDatePicker() {
            self.datePickerConfig.date = dateHelper.getLocalDateString(self.task.date);
            self.datePickerConfig.triggerRefresh();
        }

        function setTimePicker() {
            self.timePickerConfig.time = moment(self.task.time).local();
            self.timePickerConfig.triggerRefresh();
        }

        function setEstimatePicker() {
            self.estimatePickerConfig.value = self.task.estimate / MINUTES_IN_HOUR;
            self.estimatePickerConfig.triggerRefresh();
        }

        function setTitlePicker() {
            self.titlePickerConfig.value = self.task.title;
            self.titlePickerConfig.triggerRefresh();
        }

        function setDescriptionPicker() {
            self.descriptionPickerConfig.value = self.task.description;
            self.descriptionPickerConfig.triggerRefresh();
        }

        function setAddressPicker() {
            self.addressControlConfig.address = self.task.address;
            self.addressControlConfig.triggerRefresh();
        }

        function setTeamPicker() {
            self.teamPickerConfig.groupId = self.task.groupId;
            self.teamPickerConfig.teamLeadId = self.task.teamLeadId;
            self.teamPickerConfig.isAssignedToAllUsers = self.task.isAssignedToAllUsers;
            self.teamPickerConfig.userIdList = self.task.userIdList ? self.task.userIdList : [];
            self.teamPickerConfig.isOpenedState = self.task && self.task.statusId != JobStatus.Pending && self.task.statusId != JobStatus.Expired;
            self.teamPickerConfig.timeViewDayScope = self.datePickerConfig.date ?
                dateHelper.parseDateFromDatePickerFormat(self.datePickerConfig.date) :
                null;

            if (isPausedOrInProgressStatus()) {
                self.teamPickerConfig.mode = ControlMode.disable;
            }
            self.teamPickerConfig.triggerRefresh();
        }

        function setCancellingReason() {
            cancellingTemplatesProvider.GetCoordinatorByTaskType(JobType.Other).then(function (result) {
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
            return self.task && (self.task.statusId === JobStatus.Paused || self.task.statusId === JobStatus.InProgress);
        }

        function getDefaultTask() {
            return {
                id: '',
                assignId: '',
                departmentId: '',
                creatorId: '',
                title: '',
                description: '',
                address: '',
                estimate: 0,
                date: moment.utc(),
                time: moment.utc()
            }
        }

        function getQueryParamsFileUploader() {
            return {
                jobId: self.task.id,
                departmentId: self.task.departmentId,
                assignId: self.task.jobAssignId,
                isOpertionalTask: true,
                uploaderId: self.currentUser.memberId
            };
        }

        function reasonUpdated(reason) {
            if (reason) {
                self.disableCancelTaskButton = false;
                self.cancellationReasonId = reason.id;
            } else {
                self.disableCancelTaskButton = true;
            }
        }

        function getUploads() {
            operationalTaskProvider.GetUploads(self.task.jobAssignId).then(function (result) {
                self.task.uploads = result.data;
            });
        }

        function initDatePicker() {
            self.datePickerConfig = new DatePickerModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.datePickerConfig.date = dateHelper.getLocalDateString(new Date());
            self.datePickerConfig.onChange = changeDate;
            self.datePickerConfig.onSave = saveDate;
            self.datePickerConfig.securityKey = ControlSecurityKey.OperationalTaskDate;
        }

        function initTimePicker() {
            self.timePickerConfig = new TimePickerModel(self.isTaskSaved ? ControlMode.disable : ControlMode.create);
            self.timePickerConfig.onSave = changeTime;
            self.timePickerConfig.securityKey = ControlSecurityKey.OperationalTaskTime;
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

        function initOrdinalFileUploaderConfig() {
            self.ordinalFileUploaderConfig = new FileUploaderModel();

            self.ordinalFileUploaderConfig.contentTypes = [ContentType.Document, ContentType.Image, ContentType.Video];
            self.ordinalFileUploaderConfig.onGetQueryParams = getQueryParamsFileUploader;
            self.ordinalFileUploaderConfig.onFileAdded = controlHasChanges;
            self.ordinalFileUploaderConfig.triggerUploadFile = saveUploads;
            self.ordinalFileUploaderConfig.autoSave = false;
            self.ordinalFileUploaderConfig.contentUrl = OnDemandUploaderView.Documents;
            self.ordinalFileUploaderConfig.ngFlowConfig = {
                target: '/api/files/upload',
                permanentErrors: [404, 500, 501],
                chunkSize: 9007199254740992,
                maxChunkRetries: 1,
                chunkRetryInterval: 5000,
                singleFile: false,
                testChunks: false
            }
        }

        function initInstantFileUploaderConfig() {
            self.instantFileUploaderConfig = new FileUploaderModel();

            self.instantFileUploaderConfig.contentTypes = [ContentType.Document, ContentType.Image, ContentType.Video];
            self.instantFileUploaderConfig.onGetQueryParams = getQueryParamsFileUploader;
            self.instantFileUploaderConfig.onSuccess = getUploads;
        }

        function initTeamPicker() {
            self.teamPickerConfig = new TeamPickerModel(self.isTaskSaved ? ControlMode.view : ControlMode.create);
            self.teamPickerConfig.timeViewDayScope = dateHelper.parseDateFromDatePickerFormat(self.datePickerConfig.date);
            self.teamPickerConfig.onChange = teamChangedHandler;
            self.teamPickerConfig.onSave = changeTeam;
            self.teamPickerConfig.securityKey = ControlSecurityKey.OperationalTaskResponsible;
        }

        function updateHousingDepartmentHandler(housingDepartment) {
            self.selectedDepartment = housingDepartment;
        }

        function initDepartmentPicker() {
            self.departmentPickerConfig = new DepartmentPickerModel();
            self.departmentPickerConfig.onSelect = updateHousingDepartmentHandler;
            self.departmentPickerConfig.isControlDisabled = true;
            self.departmentPickerConfig.onChange = controlHasChanges;
            self.departmentPickerConfig.securityKey = ControlSecurityKey.OperationalTaskHousingDepartment;
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
            var isAccessAllowed = securityResult[self.datePickerConfig.securityKey];
            self.datePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.datePickerConfig.triggerRefresh();

            isAccessAllowed = securityResult[self.timePickerConfig.securityKey];
            self.timePickerConfig.mode = operationalTaskService.getMode(isAccessAllowed, self.isTaskSaved);
            self.timePickerConfig.triggerRefresh();

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
            self.disableCancelTaskControl = !securityResult[ControlSecurityKey.OperationalTaskCancel];
            self.showSaveButton = securityResult[ControlSecurityKey.OperationalTaskSaveButton];
            self.showJanitorButtons = securityResult[ControlSecurityKey.OperationalTaskAssignButton] && securityResult[ControlSecurityKey.OperationalTaskOpenButton];
        }

        function initCancellationReason() {
            self.cancellationReasonConfig = new SelectControlModel();
            self.cancellationReasonConfig.placeholder = $filter('translate')('Task cancelling Templates');
            self.cancellationReasonConfig.onUpdate = reasonUpdated;
        }

        function initDirectives() {
            initDatePicker();
            initTimePicker();
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

            self.rootElement = document.getElementById('other-task');

            memberProvider.GetCurrentUser().then(function (result) {
                self.currentUser = result.data;
                self.isAllowAssignMembers = self.currentUser.currentRole !== Role.Janitor;
                self.isAllowChooseDepartment = self.currentUser.currentRole === Role.Janitor;
            });

            self.isTaskSaved = $stateParams.id !== undefined;
            self.isValidateNotRequired = true;
            self.isDateWasChanged = false;
            self.isTimeWasChanged = false;

            self.disableCancelTaskControl = true;
            self.disableCancelTaskButton = true;

            self.filter = weekPlanService.getFilter();

            initDirectives();

            initOrdinalFileUploaderConfig();
            initInstantFileUploaderConfig();

            initSecurityRules();
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.OtherTaskPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.OtherTaskPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    }

    angular.module('boligdrift').controller('otherController',
        [
            '$filter',
            '$scope',
            '$state',
            '$stateParams',
            '$rootScope',
            'memberProvider',
            'uploadDataProvider',
            'operationalTaskProvider',
            'weekPlanService',
            'jobProvider',
            'cancellingTemplatesProvider',
            'moment',
            'dateHelper',
            'securityService',
            'urlService',
            'operationalTaskService',
            otherController
        ]);
})();