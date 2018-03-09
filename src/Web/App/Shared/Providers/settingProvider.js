(function () {
    var settingProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/setting/';

        self.isADFSMode = function () {
            return $http.get(baseUrl + 'isADFSMode');
        }

        self.getFileExtensions = function () {
            return $http.get(baseUrl + 'getFileExtensions');
        }
    };

    angular.module('boligdrift').service('settingProvider', ['$http', settingProvider]);

})();