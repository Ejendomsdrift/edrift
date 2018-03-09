var absenceTemplatesProvider = function($http) {
    var self = this;
    var baseUrl = "/api/absenceTemplates";

    self.GetAll = function() {
        return $http.get(baseUrl + "/GetAll");
    }

    self.Delete = function(templateId) {
        return $http.post(baseUrl + '/Delete?templateId=' + templateId);
    }

    self.Create = function (templateText) {
        var encodedTemplateText = encodeURI(templateText);
        return $http.post(baseUrl + '/Create?templateText=' + encodedTemplateText);
    }
}
angular.module("boligdrift").service("absenceTemplatesProvider", ["$http", absenceTemplatesProvider]);