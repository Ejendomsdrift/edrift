(function() {
    var historyProvider = function ($http) {
        var self = this;

        var baseUrl = '/api/history/';

        self.getHistoryByAddress = function (address) {
            return $http.get(baseUrl + 'getHistoryByAddress?address=' + encodeURIComponent(address));
        }

        self.getHistoryByDayAssignId = function(dayAssignId) {
            return $http.get(baseUrl + 'getHistoryByDayAssignId?dayAssignId=' + dayAssignId);
        }

        self.getCancelingHistory = function(dayAssignId) {
            return $http.get(baseUrl + 'getCanceledHistory?dayAssignId=' + dayAssignId);
        }

        self.getAddressesForManagementDepartment = function () {
            return $http.get(baseUrl + 'getAddressesForManagementDepartment');
        }
    };

    angular.module('boligdrift').service('historyProvider', ['$http', historyProvider]);
})();