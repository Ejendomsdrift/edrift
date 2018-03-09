(function () {
    var securityProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/security/';

        self.hasAccessByKeyList = function (model) {
            return $http.post(baseUrl + 'hasAccessByKeyList', model);
        }

        self.hasAccessByGroupName = function(model) {
            return $http.post(baseUrl + 'hasAccessByGroupName', model);
        }

        self.addKey = function(model) {
            return $http.post(baseUrl + 'addKey', model);
        }

        self.addDefaultKeys = function() {
            return $http.get(baseUrl + 'addDefaultKeys');
        }
    };

    angular.module('boligdrift').service('securityProvider', ['$http', securityProvider]);

})();