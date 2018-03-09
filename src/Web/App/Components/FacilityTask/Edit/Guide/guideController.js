(function () {
    var guideController = function (
        $stateParams,
        $state,
        $filter,
        uploadDataProvider,
        jobProvider,
        memberProvider,
        yearPlanService,
        weekPlanService, 
        securityService,
        dateHelper,
        urlService) {

        var self = this;

        

        var loadJob = function () {
            self.jobId = $stateParams.id;
            jobProvider.GetJobAssigns(self.jobId, ContentType.Media).then(function (result) {
                self.globalAssign = result.data.globalAssign;
                self.assigns = result.data.assigns;
                self.assignedDepartments = result.data.assignedDepartments;
                self.isGroupedJob = result.data.isGroupedJob;
                self.addressList = result.data.addressList;
                self.globalImages = getCorrectUploadList(ContentType.Image, self.globalAssign.uploadList);
                self.globalVideos = getCorrectUploadList(ContentType.Video, self.globalAssign.uploadList);
                self.currentUser = result.data.currentUser;

                setGlobalDirectives();

                if (result.data.isChildJob) {
                    setGlobalBlockFlags();
                }

                if (self.isRedirectStateWeekPlan) {
                    initGuidesSectionShowFlags();
                } 

                if (self.localDepartment) {
                    self.changeLocalDepartment(self.localDepartment);
                } else if (self.isGroupedJob) {
                    self.localJobAddressOnChange();
                }
            });
        };

        self.changeLocalDepartment = function (department) {
            self.localAssign = department ? self.assigns.filter(function (d) { return d.housingDepartmentIdList.indexOf(department.id) >= 0 })[0] : null;
            localAssignChanged();
        }

        self.localJobAddressOnChange = function () {
            if (!self.localJobAddress) {
                self.localAssign = null;
                self.isAdressChoosen = false;
                return;
            }

            self.isAdressChoosen = true;
            self.localAssign = self.assigns.find(function (assign) {
                return assign.jobIdList.indexOf(self.localJobAddress.id) !== -1;
            });
            localAssignChanged();
        }

        self.showLocalBlock = function () {
            return showLocalDescriptionBlock() || showLocalImageBlock() || showLocalVideoBlock();
        }

        self.showLocalLabel = function () {
            if (self.isRedirectStateWeekPlan && (self.localDepartment || self.localJobAddress)) {
                return self.localAssign ? !self.showLocalBlock() : true;
            }
        }

        self.getFormattedGuideCommentDateAndTime = function (guideCommentModel) {
            return dateHelper.getLocalDateString(guideCommentModel.date) + ',' + ' ' + dateHelper.getTimeString(guideCommentModel.date);
        }

        self.removeGuideComment = function (guideCommentModel) {
            var msg = $filter('translate')('Are you sure?');
            if (confirm(msg)) {
                jobProvider.RemoveGuideComment(guideCommentModel.id).then(function() {
                    getGuideComments();
                });
            }
        }

        function setGlobalBlockFlags() {
            self.showGlobalDescriptionBlock = self.globalAssign.description !== null && self.globalAssign.description.length > 0;
            self.showGlobalImageBlock = self.globalImages.length > 0;
            self.showGlobalVideoBlock = self.globalVideos.length > 0;
            self.showGlobalSection = self.showGlobalDescriptionBlock || self.showGlobalImageBlock || self.showGlobalVideoBlock;
        }

        function showLocalDescriptionBlock() {
            self.showLocalDescription = self.localAssign && self.localAssign.description !== null && self.localAssign.description.length > 0;
            return self.showLocalDescription;
        }

        function showLocalImageBlock() {
            self.showLocalImage = self.localAssign && getCorrectUploadList(ContentType.Image, self.localAssign.uploadList).length > 0;
            return self.showLocalImage;
        }

        function showLocalVideoBlock() {
            self.showLocalVideo = self.localAssign && getCorrectUploadList(ContentType.Video, self.localAssign.uploadList).length > 0;
            return self.showLocalVideo;
        }

        function localAssignChanged() {
            self.localImages = !self.localAssign ? [] : getCorrectUploadList(ContentType.Image, self.localAssign.uploadList);
            self.localVideos = !self.localAssign ? [] : getCorrectUploadList(ContentType.Video, self.localAssign.uploadList);

            if (self.isRedirectStateWeekPlan) {
                showLocalDescriptionBlock();
                showLocalImageBlock();
                showLocalVideoBlock();
            }

            setLocalDirectives();
        }

        function hasSectionAnyData(assign) {
            return assign && (assign.uploadList && assign.uploadList.length > 0) || assign.description !== null && assign.description.length > 0;
        }

        function saveGlobalDescription(description) {
            var model = {
                assignId: self.globalAssign.id,
                jobId: self.jobId,
                departmentId: '',
                description: description
            };

            jobProvider.SaveDescription(model).then(loadJob);
        };

        function saveLocalDescription(description) {
            var model = {
                assignId: self.localAssign ? self.localAssign.id : '',
                jobId: !self.isGroupedJob ? self.jobId : self.localJobAddress.id,
                departmentId: getDepartmentId(),
                description: description
            };

            jobProvider.SaveDescription(model).then(loadJob);
        }

        function saveUploadedContentChanges(changedDescriptionFileList, markedForDeletionFileIdList) {
            var model = { changedDescriptionFileList: changedDescriptionFileList, markedForDeletionFileIdList: markedForDeletionFileIdList };
            uploadDataProvider.updateUploadList(model).then(loadJob);
        }

        function getGlobalQueryParams() {
            return {
                jobId: self.jobId,
                assignId: self.globalAssign.id,
                isLocalChanged: false,
                departmentId: null,
                uploaderId: self.currentUser.memberId
            };
        };

        function getLocalQueryParams() {
            return {
                jobId: self.isGroupedJob ? self.localJobAddress.id : self.jobId,
                assignId: self.localAssign ? self.localAssign.id : null,
                isLocalChanged: true,
                departmentId: getDepartmentId(),
                uploaderId: self.currentUser.memberId
            };
        };

        function getDepartmentId() {
            if (self.isGroupedJob) {
                return $stateParams.department ? $stateParams.department : self.filter.departmentid;
            }

            return self.localDepartment ? self.localDepartment.id : null;
        }

        function initGuidesSectionShowFlags() {
            self.showLocalSection = angular.isDefined(self.assigns.find(function (assign) { return hasSectionAnyData(assign) === true }));
            self.showGlobalSection = hasSectionAnyData(self.globalAssign) || !self.isGroupedJob;
            self.hideBothSection = self.showGlobalSection === false && self.showLocalSection === false;
        }

        function getCorrectUploadList(contentType, uploadList) {
            if (!uploadList) {
                return [];
            }

            if (contentType == ContentType.Image) {
                return uploadList.filter(function (upload) {
                    return upload.contentType == ContentType.Image;
                });
            } else if (contentType == ContentType.Video) {
                return uploadList.filter(function (upload) {
                    return upload.contentType == ContentType.Video;
                });
            }
        }


        function setGlobalDirectives() {
            setGlobalRichTextEditorDirective();
            setGlobalUploadedImagesViewerDirective();
            setGlobalUploadedVideousViewerDirective();
        }

        function setGlobalRichTextEditorDirective() {
            self.globalRichTextEditorConfig.value = self.globalAssign.description;
            self.globalRichTextEditorConfig.triggerRefresh();
        }

        function setGlobalUploadedImagesViewerDirective() {
            self.globalUploadedImagesViewer.value = self.globalImages;
            self.globalUploadedImagesViewer.triggerRefresh();
        }

        function setGlobalUploadedVideousViewerDirective() {
            self.globalUploadedVideosViewer.value = self.globalVideos;
            self.globalUploadedVideosViewer.triggerRefresh();
        }

        function setLocalDirectives() {
            setLocalRichTextEditorDirective();
            setLocalUploadedImagesViewerDirective();
            setLocalUploadedVideousViewerDirective();
        }

        function setLocalRichTextEditorDirective() {
            self.localRichTextEditorConfig.value = self.localAssign ? self.localAssign.description : null;
            self.localRichTextEditorConfig.triggerRefresh();
        }

        function setLocalUploadedImagesViewerDirective() {
            self.localUploadedImagesViewer.value = self.localImages;
            self.localUploadedImagesViewer.triggerRefresh();
        }

        function setLocalUploadedVideousViewerDirective() {
            self.localUploadedVideosViewer.value = self.localVideos;
            self.localUploadedVideosViewer.triggerRefresh();
        }

        function initGlobalUploadedContentViewerConfig() {
            self.globalUploadedImagesViewer = new UploadedContentViewerModel();
            self.globalUploadedImagesViewer.onSave = saveUploadedContentChanges;
            self.globalUploadedImagesViewer.contentType = ContentType.Image;
            self.globalUploadedImagesViewer.securityKey = ControlSecurityKey.GuideGlobalImages;

            self.globalUploadedVideosViewer = new UploadedContentViewerModel();
            self.globalUploadedVideosViewer.onSave = saveUploadedContentChanges;
            self.globalUploadedVideosViewer.contentType = ContentType.Video;
            self.globalUploadedVideosViewer.securityKey = ControlSecurityKey.GuideGlobalVideos;
        }

        function initLocalUploadedContentViewerConfig() {
            self.localUploadedImagesViewer = new UploadedContentViewerModel();
            self.localUploadedImagesViewer.onSave = saveUploadedContentChanges;
            self.localUploadedImagesViewer.contentType = ContentType.Image;
            self.localUploadedImagesViewer.securityKey = ControlSecurityKey.GuideLocalImages;

            self.localUploadedVideosViewer = new UploadedContentViewerModel();
            self.localUploadedVideosViewer.onSave = saveUploadedContentChanges;
            self.localUploadedVideosViewer.contentType = ContentType.Video;
            self.localUploadedVideosViewer.securityKey = ControlSecurityKey.GuideLocalVideos;
        }

        function manageGlobalSecurityPermission(securityResult) {
            var mode = securityResult[self.globalUploadedImagesViewer.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalUploadedImagesViewer.mode = mode;
            self.globalUploadedImagesViewer.triggerRefresh();

            mode = securityResult[self.globalUploadedVideosViewer.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalUploadedVideosViewer.mode = mode;
            self.globalUploadedVideosViewer.triggerRefresh();

            mode = securityResult[self.globalRichTextEditorConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.globalRichTextEditorConfig.mode = mode;
            self.globalRichTextEditorConfig.triggerRefresh();

            self.showGlobalFileUploader = securityResult[self.globalUploadedImagesViewer.securityKey];
        }

        function manageLocalSecurityPermission(securityResult) {
            var mode = securityResult[self.localUploadedImagesViewer.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localUploadedImagesViewer.mode = mode;
            self.localUploadedImagesViewer.triggerRefresh();

            mode = securityResult[self.localUploadedVideosViewer.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localUploadedVideosViewer.mode = mode;
            self.localUploadedVideosViewer.triggerRefresh();

            mode = securityResult[self.localRichTextEditorConfig.securityKey] ? ControlMode.view : ControlMode.disable;
            self.localRichTextEditorConfig.mode = mode;
            self.localRichTextEditorConfig.triggerRefresh();

            self.showLocalFileUploader = securityResult[self.localUploadedImagesViewer.securityKey];
        }

        function manageSecurityPermissions(securityResult) {
            self.securityResult = securityResult;
            manageGlobalSecurityPermission(securityResult);
            manageLocalSecurityPermission(securityResult);

            if (isAdministration()) {
                // we need provide all permissions to admin for editing local sections, for grouped tasks which were created by coordinator 
                setPermissionsForAdmin();
            }
        }

        function isAdministration() {
            return self.currentUser.currentRole == Role.Administrator || self.currentUser.currentRole == Role.SuperAdmin;
        }

        function setPermissionsForAdmin() {
            self.localUploadedImagesViewer.mode = ControlMode.view;
            self.localUploadedImagesViewer.triggerRefresh();

            self.localUploadedVideosViewer.mode = ControlMode.view;
            self.localUploadedVideosViewer.triggerRefresh();

            self.localRichTextEditorConfig.mode = ControlMode.view;
            self.localRichTextEditorConfig.triggerRefresh();

            self.showLocalFileUploader = true;
        }

        function initSecurityRules() {
            var page = $stateParams.redirectState ? getPreviousState() : null;

            var model = {
                groupName: TabSecurityKey.Guide,
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

        function initUploadedContentViewerConfig() {
            initGlobalUploadedContentViewerConfig();
            initLocalUploadedContentViewerConfig();
        }

        function initGlobalRichTextEditorConfig() {
            self.globalRichTextEditorConfig = new RichTextEditorModel();
            self.globalRichTextEditorConfig.onSave = saveGlobalDescription;
            self.globalRichTextEditorConfig.securityKey = ControlSecurityKey.GuideGlobalDescription;
        }

        function initLocalRichTextEditorConfig() {
            self.localRichTextEditorConfig = new RichTextEditorModel();
            self.localRichTextEditorConfig.onSave = saveLocalDescription;
            self.localRichTextEditorConfig.securityKey = ControlSecurityKey.GuideLocalDescription;
        }

        function initRichTextEditorConfig() {
            initGlobalRichTextEditorConfig();
            initLocalRichTextEditorConfig();
        }

        function initGlobalUploaderConfig() {
            self.globalImageFileUploaderConfig = new FileUploaderModel();
            self.globalImageFileUploaderConfig.contentTypes = [ContentType.Image];
            self.globalImageFileUploaderConfig.onSuccess = loadJob;
            self.globalImageFileUploaderConfig.onGetQueryParams = getGlobalQueryParams;

            self.globalVideoFileUploaderConfig = new FileUploaderModel();
            self.globalVideoFileUploaderConfig.contentTypes = [ContentType.Video];
            self.globalVideoFileUploaderConfig.onSuccess = loadJob;
            self.globalVideoFileUploaderConfig.onGetQueryParams = getGlobalQueryParams;
        }

        function initLocalUploaderConfig() {
            self.localImageFileUploaderConfig = new FileUploaderModel();
            self.localImageFileUploaderConfig.contentTypes = [ContentType.Image];
            self.localImageFileUploaderConfig.onSuccess = loadJob;
            self.localImageFileUploaderConfig.onGetQueryParams = getLocalQueryParams;

            self.localVideoFileUploaderConfig = new FileUploaderModel();
            self.localVideoFileUploaderConfig.contentTypes = [ContentType.Video];
            self.localVideoFileUploaderConfig.onSuccess = loadJob;
            self.localVideoFileUploaderConfig.onGetQueryParams = getLocalQueryParams;
        }

        function initFileUploaderConfig() {
            initGlobalUploaderConfig();
            initLocalUploaderConfig();
        }

        function initVariables() {
            self.isRedirectStateWeekPlan = $stateParams.redirectState.indexOf(Pages.WeekPlan) != -1;

            self.showGlobalFileUploader = true;
            self.showLocalFileUploader = true;

            self.showGlobalSection = true;
            self.showLocalSection = true;
            self.hideBothSection = false;

            self.showGlobalDescriptionBlock = true;
            self.showGlobalImageBlock = true;
            self.showGlobalVideoBlock = true;

            self.showLocalDescription = true;
            self.showLocalImage = true;
            self.showLocalVideo = true;
        }

        function init() {

            checkSecurityPermission();

            if (getPreviousState() == Pages.YearPlan) {
                self.filter = yearPlanService.getFilter();
            } else {
                self.filter = weekPlanService.getFilter();
            }

            initVariables();
            loadJob();

            initFileUploaderConfig();
            initRichTextEditorConfig();
            initUploadedContentViewerConfig();
            getGuideComments();

            memberProvider.GetCurrentUser().then(function (result) {
                self.currentUser = result.data;
                initSecurityRules();
            });
        }

        self.isRemovableGuideComment = function() {
            return self.securityResult && self.securityResult[ControlSecurityKey.GuideRemoveComment];
        }

        function getGuideComments() {
            jobProvider.GetJobGuideComments($stateParams.id).then(function(result) {
                self.guideComments = result.data;
            });            
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.FacilityTaskEditPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.FacilityTaskEditPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        init();
    };

    angular.module('boligdrift').controller('guideController',
        [
            '$stateParams',
            '$state',
            '$filter',
            'uploadDataProvider',
            'jobProvider',
            'memberProvider',
            'yearPlanService',
            'weekPlanService',
            'securityService',
            'dateHelper',
            'urlService',
            guideController
        ]);
})();
