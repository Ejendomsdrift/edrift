(function () {
    var managementProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/management/';

        self.getManagementDepartmentTimeView = function (model) {
            return $http.post(baseUrl + "getManagementDepartmentTimeView", model);
        }
    };

    angular.module('boligdrift').service('managementProvider', ['$http', managementProvider]);
})();