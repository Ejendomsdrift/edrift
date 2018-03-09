(function () {
    var statisticsProvider = function($http) {
        var self = this;
        var baseUrl = '/api/statistics/';
        
        self.getUnprocessedVsProcessedTasksData = function (period) {
            return $http.post(baseUrl + 'getUnprocessedVsProcessedTasksData', period);
        }
        self.getFacilityTasksVsTenantTasksData = function(period) {
            return $http.post(baseUrl + 'getFacilityTasksVsTenantTasksData', period);
        }

        self.getSpentTimeVsTenantTasksData = function(period) {
            return $http.post(baseUrl + 'getSpentTimeVsTenantTasksData', period);
        }

        self.getSpentTimeVsFacilityTasksData = function(period) {
            return $http.post(baseUrl + 'getSpentTimeVsFacilityTasksData', period);
        }

        self.getCompletedVsOverdueTasksData = function(period) {
            return $http.post(baseUrl + 'getCompletedVsOverdueTasksData', period);
        }

        self.getStatisticFiltersModel = function() {
            return $http.get(baseUrl + 'getStatisticFiltersModel');
        }

        self.getAbsencesReasonData = function(period) {
            return $http.post(baseUrl + 'getAbsencesReasonData', period);
        }

        self.getTenantTasksVsVisitsAmountData = function(period) {
            return $http.post(baseUrl + 'getTenantTasksVsVisitsAmountData', period);
        }

        self.getCancelingReasonData = function(period) {
            return $http.post(baseUrl + 'getTenantTaskSeparatedByCanceledReasonData', period);
        }
    };

    angular.module('boligdrift').service('statisticsProvider', ['$http', statisticsProvider]);
})();