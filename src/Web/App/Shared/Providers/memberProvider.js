(function () {
    var memberProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/member/';

        self.GetMembers = function () {
            return $http.get(baseUrl + 'getMembers');
        }

        self.GetAllowedMembersForJob = function () {
            return $http.get(baseUrl + 'getAllowedMembersForJob');
        }

        self.GetCurrentUser = function () {
            return $http.get(baseUrl + 'getCurrentUser');
        }

        self.GetCurrentUserContext = function() {
            return $http.get(baseUrl + 'getCurrentUserContext');
        }

        self.GetCurrentEmployee = function () {
            return $http.get(baseUrl + 'getCurrentEmployee');
        }

        self.UpdateEmployeeSettings = function (model) {
            return $http.post(baseUrl + 'updateEmployeeSettings', model);
        }

        self.SwitchMemberToNextAvailableRole = function (memberId) {
            return $http.post(baseUrl + 'switchMemberToNextAvailableRole?memberId=' + memberId);
        }

        self.SaveManagementDepartment = function(managementDepartmentId) {
            return $http.post(baseUrl + 'saveManagementDepartment?managementDepartmentId=' + managementDepartmentId);
        }
    }

    angular.module('boligdrift').service('memberProvider', ['$http', memberProvider]);
})();