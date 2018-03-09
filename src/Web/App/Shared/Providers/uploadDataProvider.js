var uploadDataProvider = function ($http, $q, toastService) {
    var self = {};
    var baseRoute = '/api/files/';

    var showToastAfter = function (promise) {
        var deferred = $q.defer();
        promise.then(function (result) {
            toastService.showToastSaveSuccessMessage();
            deferred.resolve(result);
        });
        return deferred.promise;
    };

    self.updateUploadList = function (model) {
        return showToastAfter($http.post(baseRoute + 'updateUploadList', model));
    }

    self.deleteUploadedFile = function(fileId) {
        return showToastAfter($http.post(baseRoute + 'deleteUploadedFile?fileId=' + fileId));
    }

    self.delete = function (fileId) {
        return showToastAfter($http.post(baseRoute + 'delete?fileId=' + fileId));
    }

    self.getDownloadLink = function(fileName, originalFileName) {
        return baseRoute + 'download?fileName=' + fileName + '&originalFileName=' + originalFileName;
    }

    return self;
}

angular.module('boligdrift').service('uploadDataProvider', ['$http', '$q', 'toastService', uploadDataProvider]);