(function () {
    var taskDetailsController = function (
        $state,
        $stateParams,
        $filter,
        jobProvider,
        janitorService,
        operationalTaskProvider,
        managementProvider,
        uploadDataProvider,
        cancellingTemplatesProvider,
        memberProvider,
        historyProvider,
        dateHelper,
        $timeout) {

        var self = this;
        self.jobStatusNameList = [];
        self.isMapToogled = true;
        self.isGlobalDescriptionToogled = true;
        self.isLocalDescriptionToogled = true;
        self.isGlobalDocumentsToogled = true;
        self.isLocalDocumentsToogled = true;
        self.isOtherInformationToogled = true;
        self.isGuideCommentBlockToogled = true;
        self.isRelatedJobsByAddressBlockToogled = true;
        self.isMyJob = $stateParams.previousState === State.JanitorMyTasks;
        self.isOpenedJob = $stateParams.previousState === State.JanitorOpenedTasks;
        self.jobStatus = JobStatus;
        self.showUnassignBlock = false;
        self.uploadedFileIds = [];
        self.jobCancelingHistory = [];
        self.isActiveWaitIcon = true;

        self.getFormattedCreateDateString = function () {
            if (self.taskCreationInfo) {
                return self.getDateString(self.taskCreationInfo.creationDate);
            }
        }

        self.getFormattedDateString = function(dateTime) {
            if (dateTime) {
                return dateHelper.formatUtcDateString(dateTime);
            }
        }

        self.goToPreviousState = function () {
            var previousState = $stateParams.previousListPage ? $stateParams.previousListPage : $stateParams.previousState;
            $state.go(previousState, { id: $stateParams.id, dayAssignId: $stateParams.dayAssignId, previousState: $state.current.name, jobType: $stateParams.jobType });
        }

        self.getDateString = function (date) {
            return janitorService.getFormatedDateString(date);
        }

        self.isAllowedTaskType = function () {
            return self.jobDetails.jobType == JobType.Other || self.jobDetails.jobType == JobType.Tenant;
        }

        self.getTimeString = function (date) {
            return janitorService.getFormatedTimeString(date);
        }

        self.getTimeBeforeStartString = function () {
            return janitorService.getTimeBeforeStartString(self.jobDetails.date);
        }

        self.isTaskOverdue = function (task) {
            return task && janitorService.isTaskOverdue(task);
        }

        self.isStartTimeLabelVisible = function (task) {
            return task && janitorService.isStartTimeLabelVisible(task);
        }

        self.showGlobalGuideBlock = function () {
            if (self.jobDetails) {
                var documents = self.jobDetails.globalUploadList.filter(function (upload) {
                    return upload.contentType !== ContentType.Document;
                });

                return self.jobDetails.globalDescription || documents.length > 0;
            } else {
                return false;
            }
        }

        self.showLocalGuideBlock = function () {
            if (self.jobDetails) {
                var documents = self.jobDetails.localUploadList.filter(function (upload) {
                    return upload.contentType !== ContentType.Document;
                });

                return self.jobDetails.localDescription || documents.length > 0;
            } else {
                return false;
            }
        }

        self.showGlobalDocumentsBlock = function () {
            if (self.jobDetails && self.jobDetails.globalUploadList.length > 0) {
                for (var i = 0; i < self.jobDetails.globalUploadList.length; i++) {
                    if (self.jobDetails.globalUploadList[i].contentType === ContentType.Document) {
                        return true;
                    }
                }
            } else {
                return false;
            }
        }

        self.showLocalDocumentsBlock = function () {
            if (self.jobDetails && self.jobDetails.localUploadList.length > 0) {
                for (var i = 0; i < self.jobDetails.localUploadList.length; i++) {
                    if (self.jobDetails.localUploadList[i].contentType === ContentType.Document) {
                        return true;
                    }
                }
            } else {
                return false;
            }
        }

        self.toogleMap = function () {
            self.isMapToogled = !self.isMapToogled;
        }

        self.toogleGlobalDescription = function () {
            self.isGlobalDescriptionToogled = !self.isGlobalDescriptionToogled;
        }

        self.toogleLocalDescription = function () {
            self.isLocalDescriptionToogled = !self.isLocalDescriptionToogled;
        }

        self.toogleGuideCommentBlock = function () {
            self.isGuideCommentBlockToogled = !self.isGuideCommentBlockToogled;
        }

        self.toogleRelatedJobsByAddressBlock = function () {
            self.isRelatedJobsByAddressBlockToogled = !self.isRelatedJobsByAddressBlockToogled;
        }

        self.toogleGlobalDocuments = function () {
            self.isGlobalDocumentsToogled = !self.isGlobalDocumentsToogled;
        }

        self.toogleLocalDocuments = function () {
            self.isLocalDocumentsToogled = !self.isLocalDocumentsToogled;
        }

        self.toogleOtherInformation = function () {
            self.isOtherInformationToogled = !self.isOtherInformationToogled;
        }

        self.videoCarouselConfig = {
            filter: { contentType: 'Video' },
            notFoundtitle: $filter('translate')('Videos are not uploaded'),
            getDescription: function (slide) { return slide.description; },
            getSrc: function (slide) { return slide.path + appendResizeParamsToUrl(); },
            getType: function (slide) { return 'video'; }
        };

        self.imageCarouselConfig = {
            filter: { contentType: 'Image' },
            notFoundtitle: $filter('translate')('Images are not uploaded'),
            getDescription: function (slide) { return slide.description; },
            getSrc: function (slide) { return slide.path + appendResizeParamsToUrl(); },
            getType: function (slide) { return 'image'; }
        };

        self.getCreationDate = function (dateString) {
            return self.getDateString(dateString);
        }

        self.isCustomTemplateMode = function () {
            return self.selectedCancellingTemplate && !self.selectedCancellingTemplate.id;
        }

        self.startJob = function () {
            self.cancelActiveMode();
            jobProvider.StartJob($stateParams.dayAssignId).then(function () {
                loadJobDetails($stateParams.id, $stateParams.dayAssignId);
            }).catch(function (error) {
                if (error.data.exceptionMessage == "Not allowed status") {
                    var msg = $filter('translate')('Status has been changed. Please, refresh the page.');
                    alert(msg);
                }
            });
        }

        self.unassignJob = function (newJobStatus) {
            if (!self.isActiveUnassignMode || angular.isUndefined(newJobStatus) || !self.selectedCancellingTemplate) {
                self.changeStatusCommentModel = null;
                self.selectedCancellingTemplate = null;
                self.isActiveUnassignMode = true;
                self.teamViewerModel.showTimeReportControl = !self.isOtherJob;

                if (angular.isUndefined(newJobStatus) && $stateParams.jobType != JobType.Other) {
                    var currentDate = new Date();
                    var model = { id: $stateParams.dayAssignId, value: currentDate };
                    jobProvider.getApproximateSpentTime(model).then(function(result) {
                        refreshMemberSpentTime(result.data);
                        return;
                    });
                }
            } else {
                unassignJob(newJobStatus);
            }
        }

        self.pauseJob = function () {
            jobProvider.PauseJob($stateParams.dayAssignId).then(function () {
                loadJobDetails($stateParams.id, $stateParams.dayAssignId);
            }).catch(function (error) {
                if (error.data.exceptionMessage == "Not allowed status") {
                    var msg = $filter('translate')('Status has been changed. Please, refresh the page.');
                    alert(msg);
                }
            });
        }

        self.completeJob = function () {
            if (!self.isActiveCompletedMode) {
                self.changeStatusCommentModel = null;
                self.isActiveCompletedMode = true;
                self.teamViewerModel.showTimeReportControl = !self.isOtherJob;

                var currentDate = new Date();
                var model = { id: $stateParams.dayAssignId, value: currentDate };
                jobProvider.getApproximateSpentTime(model).then(function (result) {
                    refreshMemberSpentTime(result.data);
                    return;
                });
            } else {
                completeJob();
            }
        }

        self.cancelActiveMode = function () {
            self.isActiveCompletedMode = false;
            self.isActiveUnassignMode = false;
            self.teamViewerModel.showTimeReportControl = false;
        }

        self.assignJob = function () {
            self.cancelActiveMode();
            jobProvider.AssignJob(self.jobDetails.dayAssignId).then(function () {
                goToMyTasks();
            });
        }

        self.reopenJob = function () {
            self.cancelActiveMode();
            jobProvider.ReopenJob(self.jobDetails.dayAssignId).then(function () {
                goToMyTasks();
            });
        }

        self.IsAssignedStatus = function () {
            return self.currentStatus === self.jobStatus.Assigned;
        }

        self.isJobTypeAndUserValid = function () {
            if (self.isOtherTypeTask()) {
                return isCurrentUserTeamLead();
            }
            return true;
        }

        self.canChangeJobWithAssignedStatus = function () {
            return self.currentStatus === self.jobStatus.Assigned && self.jobDetails.allowChangeStatus;
        }

        self.IsInProgressStatus = function () {
            return self.currentStatus === self.jobStatus.InProgress;
        }

        self.canChangeJobWithInProgressStatus = function () {
            return self.currentStatus === self.jobStatus.InProgress && self.jobDetails.allowChangeStatus;
        }

        self.IsPausedStatus = function () {
            return self.currentStatus === self.jobStatus.Paused;
        }

        self.canChangeJobWithPausedStatus = function () {
            return self.currentStatus === self.jobStatus.Paused && self.jobDetails.allowChangeStatus;
        }

        self.canChangeJobWithCompletedStatus = function () {
            return self.currentStatus === self.jobStatus.Completed && self.jobDetails.allowChangeStatus;
        }

        self.getEstimateTimeString = function () {
            return janitorService.getEstimateTimeString(self.jobDetails.date, self.jobDetails.estimate);
        }

        self.saveGuideComment = function () {
            var model = {
                jobId: $stateParams.id,
                dayAssignId: $stateParams.dayAssignId,
                memberId: self.currentUserData.memberModel.memberId,
                comment: self.guideCommentModel
            };

            jobProvider.SaveOrUpdateGuideComment(model).then(function () {
                getJobGuideComments($stateParams.id);
                self.guideCommentModel = '';
            });
        }

        self.switchToEditGuideComment = function (guideComment) {
            guideComment.isCommentEdit = !guideComment.isCommentEdit;
        }

        self.updateGuideComment = function (guideCommentModel) {
            var model = {
                id: guideCommentModel.id,
                jobId: guideCommentModel.jobId,
                dayAssignId: guideCommentModel.dayAssignId,
                memberId: guideCommentModel.memberId,
                comment: guideCommentModel.comment
            };

            jobProvider.SaveOrUpdateGuideComment(model).then(function () {
                self.switchToEditGuideComment(guideCommentModel);
            });
        }

        self.removeGuideComment = function (guideCommentModel) {
            var msg = $filter('translate')('Are you sure?');
            if (confirm(msg)) {
                jobProvider.RemoveGuideComment(guideCommentModel.id).then(function () {
                    getJobGuideComments($stateParams.id);
                });
            }
        }

        self.isOwnGuideComment = function (guideCommentModel) {
            return guideCommentModel.memberId === self.currentUserData.memberModel.memberId;
        }

        self.getFormattedGuideCommentDateAndTime = function (guideCommentModel) {
            return janitorService.getFormatedDateString(guideCommentModel.date) + ',' + ' ' + janitorService.getFormatedTimeString(guideCommentModel.date);
        }

        self.isFacility = function () {
            return self.jobDetails && self.jobDetails.jobType === JobType.Facility;
        }

        self.isTenant = function () {
            return self.jobDetails && self.jobDetails.jobType === JobType.Tenant;
        }

        self.getStatusTranslation = function (status) {
            return JobStatusPlatformKey.JanitorPlatform + self.jobStatusNameList[status];
        }

        self.goToTaskDetails = function (job) {
            var previousListPage = $stateParams.previousListPage ? $stateParams.previousListPage : $stateParams.previousState;
            $state.go(State.JanitorTaskDetails, { id: job.jobId, dayAssignId: job.dayAssignId, previousState: $state.current.name, jobType: null, previousListPage: previousListPage });
        }

        self.isOtherTypeTask = function() {
            if (self.jobDetails) {
                return self.jobDetails.jobType == JobType.Other;
            }
        }

        self.getTimeBeforeStartLabel = function (task) {
            return janitorService.getTimeBeforeStartLabel(task);
        }

        function isCurrentUserTeamLead() {
            if (self.currentUserData && self.jobDetails) {
                return self.currentUserData.memberModel.memberId === self.jobDetails.teamLeadId;
            }
        }

        function refreshMemberSpentTime(data) {
            data.forEach(function (memberSpentTime) {
                self.teamViewerModel.members.forEach(function (member) {
                    if (member.memberId == memberSpentTime.id) {
                        member.spentHours = memberSpentTime.value.approximateSpentHours;
                        member.spentMinutes = memberSpentTime.value.approximateSpentMinutes;
                        member.totalSpentHours = memberSpentTime.value.totalSpentHours;
                        member.totalSpentMinutes = memberSpentTime.value.totalSpentMinutes;
                        return;
                    }
                });
            });
        }

        function unassignJob(newJobStatus) {
            if (confirm($filter('translate')('Are you sure?'))) {
                var model = {
                    dayAssignId: $stateParams.dayAssignId,
                    changeStatusComment: self.changeStatusCommentModel,
                    members: self.teamViewerModel.members,
                    newJobStatus: newJobStatus,
                    selectedCancellingId: self.selectedCancellingTemplate ? self.selectedCancellingTemplate.id : '',
                    uploadedFileIds: self.uploadedFileIds
                };

                jobProvider.UnassignJob(model).then(function () {
                    goToMyTasks();
                });
            }
        }

        function completeJob() {
            if (!confirm($filter('translate')('Are you sure?'))) {
                return;
            }

            var model = {
                dayAssignId: $stateParams.dayAssignId,
                changeStatusComment: self.changeStatusCommentModel,
                members: self.teamViewerModel.members,
                uploadedFileIds: self.uploadedFileIds
            };

            jobProvider.CompleteJob(model).then(function (result) {
                self.completeJobErrorMessage = null;

                if (result.data.isSuccessful) {
                    goToMyTasks();
                } else {
                    var errorStringPattern = 'You will not able to complete job because current status is {currentStatus}. Please refresh page';
                    var replacedStringPattern = $filter('translate')(errorStringPattern.replace('{currentStatus}', result.data.currentStatus));
                    self.completeJobErrorMessage = replacedStringPattern;
                }
            });
        }

        self.getEstimateTimeInHoursString = function () {
            return janitorService.getEstimateTimeInHoursString(self.jobDetails.estimate);
        }

        self.isHaveEstimations = function () {
            return self.jobDetails && self.jobDetails.estimate > 0;
        }

        function goToMyTasks() {
            $state.go(State.JanitorMyTasks);
        }

        function uploadedFileSuccessHandler(fileresultJson) {
            var fileData = JSON.parse(fileresultJson);
            self.uploadedFileIds.push(fileData.fileId);
            jobProvider.GetUploadedFile(self.jobDetails.dayAssignId).then(function (result) {
                self.janitorUploadedImagesViewer.value = result.data.images;
                self.janitorUploadedVideosViewer.value = result.data.videos;
            });
        }

        function getQueryParams() {
            return { dayAssignId: self.jobDetails.dayAssignId };
        };

        function deleteUploadedFile(fileId) {
            self.uploadedFileIds.pop(fileId);
            uploadDataProvider.deleteUploadedFile(fileId).then(function () {
                loadJobDetails($stateParams.id, $stateParams.dayAssignId);
            });
        }

        function isTaskDurationNeeded() {
            return self.jobDetails.date && self.jobDetails.estimate && (self.jobDetails.jobType === JobType.Tenant || self.jobDetails.jobType === JobType.Other);
        }

        function loadCancellingTemplates(taskType) {
            cancellingTemplatesProvider.GetAllByTaskType(taskType).then(function (result) {
                self.cancellingTemplates = result.data;
            });
        }

        function refreshTeamPicker() {
            self.teamViewerModel.allowChangeStatus = self.jobDetails.allowChangeStatus;
            self.teamViewerModel.groupName = self.jobDetails.groupName;
            self.teamViewerModel.teamLeadId = self.jobDetails.teamLeadId;
            self.teamViewerModel.members = self.jobDetails.members ? self.jobDetails.members : [];
        }

        function refreshJanitorUploadedContentViewer() {
            if (self.jobDetails.janitorUploadList) {
                self.jobDetails.janitorUploadImageList.forEach(function (file) {
                    self.uploadedFileIds.push(file.fileId);
                });

                self.jobDetails.janitorUploadVideoList.forEach(function (file) {
                    self.uploadedFileIds.push(file.fileId);
                });
            }

            self.janitorUploadedImagesViewer.value = self.jobDetails.janitorUploadImageList;
            self.janitorUploadedVideosViewer.value = self.jobDetails.janitorUploadVideoList;
        }

        function appendResizeParamsToUrl() {
            var width = 400;
            var quality = 50;

            return '?width=' + width + '&quality=' + quality;
        }

        function getJobGuideComments(jobId) {
            jobProvider.GetJobGuideComments(jobId).then(function (result) {
                self.jobGuideComments = result.data;
            });
        }

        function initFileUploaderConfig() {
            self.fileUploaderConfig = new FileUploaderModel();

            self.fileUploaderConfig.ngFlowConfig.target = '/api/files/uploadFileWhenChangeJobStatus';
            self.fileUploaderConfig.onSuccess = uploadedFileSuccessHandler;
            self.fileUploaderConfig.onGetQueryParams = getQueryParams;
            self.fileUploaderConfig.contentTypes = [ContentType.Image, ContentType.Video];
        }

        function initCssHeaderClass(previousState) {
            var element = '';
            if (previousState === State.JanitorMyTasks) {
                element = document.querySelector('.js-janitor-my-tasks');
                element.classList.add('active-item');
            } else if (previousState === State.JanitorOpenedTasks) {
                element = document.querySelector('.js-janitor-open-tasks');
                element.classList.add('active-item');
            }
        }

        function clearScrollPosition() {
            var positionToScrolableArea = Number(sessionStorage.getItem('positionToScrolableArea'));
            if (positionToScrolableArea) {
                $('body,html').scrollTop(0);
            }
        }

        function loadJobDetails(jobId, dayAssignId) {
            jobProvider.GetJobDetailsById(dayAssignId).then(function (result) {
                var jobTypeNameList = janitorService.getJobTypeName();
                loadCancellingTemplates(result.data.jobType);

                self.jobDetails = result.data;
                hideWaitIcon();
                self.isCompletedJob = result.data.jobStatus == JobStatus.Completed;

                if (result.data.jobType == JobType.AdHoc ||
                    result.data.jobType == JobType.Tenant ||
                    result.data.jobType == JobType.Other) {
                    self.operationalTaskDescription = self.jobDetails.localDescription;
                    self.jobDetails.localDescription = '';
                }

                self.isOtherJob = self.jobDetails.jobType === JobType.Other;

                self.isTaskDurationRequired = isTaskDurationNeeded();
                self.currentStatus = self.jobDetails.jobStatus;
                self.jobDetails.jobTypeName = jobTypeNameList[self.jobDetails.jobType];
                refreshTeamPicker();
                refreshJanitorUploadedContentViewer();
                operationalTaskProvider.getTaskCreationInfo(jobId).then(function (result) {
                    self.taskCreationInfo = result.data;
                });
                loadJobsRelatedByAddressForTenantJob();
                $timeout(clearScrollPosition, 0);
            });
        }

        function fillJobStatusNameList() {
            for (var name in JobStatus) {
                if (JobStatus.hasOwnProperty(name)) {
                    self.jobStatusNameList.push(name);
                }
            }
        }

        function fillJobCancelHistory(dayAssignId) {
            historyProvider.getCancelingHistory(dayAssignId).then(function (result) {
                self.jobCancelingHistory = result.data;
            });
        }

        function getCurrentUserContext() {
            memberProvider.GetCurrentUserContext().then(function (result) {
                self.currentUserData = result.data;
            });
        }

        function loadJobsRelatedByAddressForTenantJob() {
            if (self.isTenant()) {
                jobProvider.GetTenantJobsRelatedByAddress($stateParams.id).then(function (result) {
                    self.relatedByAddressJobList = result.data;
                    self.hasJobsRelatedByAddress = self.relatedByAddressJobList.length > 0;
                });
            }
        }

        function hideWaitIcon() {
            self.isActiveWaitIcon = false;
        }

        function initControl() {
            self.teamViewerModel = new TeamViewerModel();
            getCurrentUserContext();
            fillJobStatusNameList();
            fillJobCancelHistory($stateParams.dayAssignId);
            loadJobDetails($stateParams.id, $stateParams.dayAssignId);
            initCssHeaderClass($stateParams.previousState);
            initFileUploaderConfig();
            getJobGuideComments($stateParams.id);

            self.janitorUploadedImagesViewer = new JanitorUploadedContentViewerModel();
            self.janitorUploadedImagesViewer.onFileDeletion = deleteUploadedFile;
            self.janitorUploadedImagesViewer.contentType = ContentType.Image;

            self.janitorUploadedVideosViewer = new JanitorUploadedContentViewerModel();
            self.janitorUploadedVideosViewer.onFileDeletion = deleteUploadedFile;
            self.janitorUploadedVideosViewer.contentType = ContentType.Video;
        }

        initControl();
    }

    angular.module('boligdrift').controller('taskDetailsController',
        [
            '$state',
            '$stateParams',
            '$filter',
            'jobProvider',
            'janitorService',
            'operationalTaskProvider',
            'managementProvider',
            'uploadDataProvider',
            'cancellingTemplatesProvider',
            'memberProvider',
            'historyProvider',
            'dateHelper',
            '$timeout',
            taskDetailsController
        ]);
})();