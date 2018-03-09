(function () {
    var fileUploader = function () {
        var controller = function (memberProvider, dateHelper, settingProvider, $filter) {
            var self = this;
            self.isActiveLoadIcon = false;

            self.initControl = function ($flow) {
                self.flow = $flow;
            }

            self.onSubmitted = function () {
                if (self.config.autoSave && !self.isNotAllowedExtension) {
                    uploadFile();
                }
            }

            self.onAdded = function ($file) {
                self.config.onFileAdded();
                var ext = '.' + $file.getExtension().toLowerCase();
                self.isNotAllowedExtension = self.extensions.indexOf(ext) === -1;
                if (self.isNotAllowedExtension) {
                    var alertMsgForNotAllowedExtension = $filter('translate')('This file is not allowed here');
                    alert(alertMsgForNotAllowedExtension);
                }

                return self.extensions.indexOf(ext) > -1;
            };

            self.onSuccess = function (message) {
                self.config.onSuccess(message);
                hideLoadIcon();
            }

            self.config.triggerUploadFile = function () {
                uploadFile();
            }

            self.getFormatedCreationDate = function () {
                return dateHelper.getLocalDateString(new Date());
            }

            function hideLoadIcon () {
                self.isActiveLoadIcon = false;
            }

            function showLoadIcon () {
                self.isActiveLoadIcon = true;
            }

            function loadFileExtensions() {
                self.extensions = [];
                settingProvider.getFileExtensions().then(function (result) {
                    self.config.contentTypes.forEach(function (contentType) {
                        self.extensions = self.extensions.concat(result.data[contentType.toLowerCase()]);
                    });                    
                });
            }

            function loadCurrentUser() {
                memberProvider.GetCurrentUser().then(function (result) {
                    self.currentUser = result.data;
                });
            }

            function uploadFile() {
                if (self.flow && self.flow.files.length) {
                    showLoadIcon();
                    angular.extend(self.flow.opts.query, self.config.onGetQueryParams());
                    self.flow.upload();
                } 
            }

            function initControl() {
                loadFileExtensions();
                loadCurrentUser();
            }

            initControl();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/FileUploader/fileUploader.html',
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'uploaderCtrl',
            controller: ['memberProvider', 'dateHelper', 'settingProvider', '$filter', controller]
        };
    }

    angular.module('boligdrift').directive('fileUploader', [fileUploader]);
})();