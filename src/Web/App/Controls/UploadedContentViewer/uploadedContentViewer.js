(function () {
    var uploadedContentViewer = function () {
        var controller = function ($filter) {
            var self = this;
            
            self.videoCarouselConfig = {
                filter: { contentType: 'Video' },
                notFoundtitle: $filter('translate')('Videos are not uploaded'),
                getDescription: function (slide) { return slide.description; },
                getSrc: function (slide) { return slide.path; },
                getType: function (slide) { return 'video'; }
            };

            self.imageCarouselConfig = {
                filter: { contentType: 'Image' },
                notFoundtitle: $filter('translate')('Images are not uploaded'),
                getDescription: function (slide) { return slide.description; },
                getSrc: function (slide) { return slide.path; },
                getType: function (slide) { return 'image'; }
            };

            self.config.triggerRefresh = function () {
                self.markedForDeletionFileIdList = [];
                self.changedDescriptionFileList = [];
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = angular.copy(self.config.value);
            }

            self.config.triggerCheckControlChanged = function () {
                if (self.savedValue.length !== self.config.value.length) {
                    return true;
                } else {
                    for (var i = 0; i < self.savedValue.length; i++) {
                        if (self.savedValue[i].description !== self.config.value[i].description) {
                            return true;
                        }
                    }

                    return false;
                }
            }

            self.isDisableMode = function () {
                return self.editControlConfig.mode == ControlMode.disable;
            }

            self.isEditableMode = function() {
                return self.editControlConfig.mode == ControlMode.edit;
            }

            self.isContentTypeImage = function() {
                return self.config.contentType == ContentType.Image;
            }

            self.isContentTypeVideo = function () {
                return self.config.contentType == ContentType.Video;
            }

            self.changeDescription = function (uploadedContent) {
                var isFileFounded = self.changedDescriptionFileList.find(function(file) {
                    return file.fileId === uploadedContent.fileId;
                });

                if (!isFileFounded) {
                    self.changedDescriptionFileList.push(uploadedContent);
                }
            }

            self.deleteFile = function(fileId) {
                for (var i = 0; i < self.config.value.length; i++) {
                    if (self.config.value[i].fileId === fileId) {
                        self.markedForDeletionFileIdList.push(fileId);
                        self.config.value.splice(i, 1);
                        break;
                    }
                }
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    self.config.onSave(self.changedDescriptionFileList, self.markedForDeletionFileIdList);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.markedForDeletionFileIdList = [];
                self.changedDescriptionFileList = [];
                self.config.value = angular.copy(self.savedValue);
            }

            function initControl() {
                self.markedForDeletionFileIdList = [];
                self.changedDescriptionFileList = [];

                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.config.triggerCopySavedData();
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/UploadedContentViewer/uploadedContentViewer.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'uploadedContentCtrl',
            controller: ['$filter', controller]
        };
    }

    angular.module('boligdrift').directive('uploadedContentViewer', [uploadedContentViewer]);
})();