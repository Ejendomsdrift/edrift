var employeesManagementProvider = function ($http, $q, toastService) {
    var self = this;
    var baseUrl = "/api/employeesManagement/";

    var showSaveToastAfter = function (promise) {
        var deferred = $q.defer();
        promise.then(function (result) {
            if (result.data.isSucceeded) {
                toastService.showToastSaveSuccessMessage();
            }
            deferred.resolve(result);
        });
        return deferred.promise;
    };

    var showDeleteToastAfter = function (promise) {
        var deferred = $q.defer();
        promise.then(function (result) {
            toastService.showToastSaveSuccessMessage('Delete success');
            deferred.resolve(result);
        });
        return deferred.promise;
    };

    self.getAbsencesForCurrentManagementDepartment = function () {
        return $http.get(baseUrl + 'GetAbsencesForCurrentManagementDepartment');
    }

    self.addAbsenceForMember = function (model) {
        return showSaveToastAfter($http.post(baseUrl + 'AddAbsenceForMember', model));
    }

    self.deleteAbsence = function (absenceId) {
        return showDeleteToastAfter($http.post(baseUrl + 'DeleteAbsence?absenceId=' + absenceId));
    }

    self.DeleteAllAbsencesForMember = function (memberId) {
        return showDeleteToastAfter($http.post(baseUrl + 'DeleteAllAbsencesForMember?memberId=' + memberId));
    }
}

angular.module("boligdrift").service("employeesManagementProvider", ["$http", '$q', 'toastService', employeesManagementProvider]);