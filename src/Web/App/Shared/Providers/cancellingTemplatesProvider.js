var cancellingTemplatesProvider = function ($http) {
    var self = this;
    var baseUrl = "/api/cancelingTemplates/";

    self.GetByFilter = function(isCoordinatorReason) {
        return $http.get(baseUrl + 'getByFilter?isCoordinatorReason=' + isCoordinatorReason);
    }

    self.GetAllByTaskType = function(taskType) {
        return $http.get(baseUrl + 'getAllByTaskType?taskType=' + taskType);
    }

    self.GetCoordinatorByTaskType = function(taskType) {
        return $http.get(baseUrl + 'getCoordinatorByTaskType?taskType=' + taskType);
    }

    self.Delete = function(templateId) {
        return $http.post(baseUrl + 'delete?templateId=' + templateId);
    }

    self.Create = function (model) {
        return $http.post(baseUrl + 'create', model);
    }
}
angular.module('boligdrift').service('cancellingTemplatesProvider', ['$http', cancellingTemplatesProvider]);