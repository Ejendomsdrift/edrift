var departmentProvider = function ($http) {
    var self = this;
    var baseUrl = "/api/department/";

    self.getDepartments = function () {
        return $http.get(baseUrl + 'getHousingDepartments');
    }

    self.getDepartmentInfoById = function(housingDepartmentId) {
        return $http.get(baseUrl + 'getDepartmentInfoById?housingDepartmentId=' + housingDepartmentId);
    }
}

angular.module('boligdrift').service('departmentProvider', ['$http', departmentProvider]);