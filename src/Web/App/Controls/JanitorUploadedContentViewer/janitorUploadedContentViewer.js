(function () {
    var janitorUploadedContentViewer = function () {
        var controller = function () {
            var self = this;

            self.deleteFile = function (fileId) {
                self.config.onFileDeletion(fileId); 
            }

            self.isImage = function() {
                if (self.config) {
                    return self.config.contentType == ContentType.Image;
                }
            }

            self.isVideo = function() {
                if (self.config) {
                    return self.config.contentType == ContentType.Video;
                }
            }
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/janitorUploadedContentViewer/janitorUploadedContentViewer.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'janitorUploadedContentCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('janitorUploadedContentViewer', [janitorUploadedContentViewer]);
})();