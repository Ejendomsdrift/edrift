var weekPlanProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/weekPlan/';

        self.GetWeekTasks = function(model) {
            return $http.post(baseUrl + 'getWeekTasks', model);
        }

        self.GetJobsForWeek = function (model) {
            return $http.post(baseUrl + 'getJobsForWeek', model);
        }

        self.GetDayAssignIdsForMembers = function (model) {
            return $http.post(baseUrl + 'getDayAssignIdsForMembers', model);
        }
    }

angular.module('boligdrift').service('weekPlanProvider', ['$http', weekPlanProvider]);
