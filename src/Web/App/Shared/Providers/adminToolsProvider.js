(function () {
    var adminToolsProvider = function ($http, $q, toastService) {
        var self = this;
        var baseUrl = '/api/admintools';

        var showToastAfter = function (promise) {
            var deferred = $q.defer();
            promise.then(function (result) {
                toastService.showToastSaveSuccessMessage();
                deferred.resolve(result);
            });
            return deferred.promise;
        };

        self.CreateDefaultCategories = function () {
            return showToastAfter($http.post(baseUrl + '/createDefaultCategories'));
        }

        self.ClearDataBase = function (collections) {
            return showToastAfter($http.post(baseUrl + '/clearDataBase?collections='+collections));
        }

        self.GetDataBase = function () {
            return $http.get(baseUrl + '/getDataBase');
        }
    };

    angular.module('boligdrift').service('adminToolsProvider', ['$http', '$q', 'toastService', adminToolsProvider]);
})();