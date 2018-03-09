var yearPlanProvider = function ($http) {
    var self = this;
    var baseUrl = '/api/yearPlan/'; 

    self.getAllDepartmentsYearPlan = function (model) {
        return $http.post(baseUrl + 'getAllDepartmentsYearPlan', model);
    }

    self.getDepartmentYearPlan = function () {
        return $http.get(baseUrl + 'getDepartmentYearPlan');
    }

    self.getYearPlanWeekData = function(departmentId, year) {
        return $http.get(baseUrl + 'getYearPlanWeekData?departmentId=' + departmentId + '&year=' + year);
    }

    self.getAllData = function(departmentId, year) {
        return $http.get(baseUrl + 'getAllData?departmentId=' + departmentId + '&year=' + year);
    }
};

angular.module('boligdrift').service('yearPlanProvider', ['$http', yearPlanProvider]);