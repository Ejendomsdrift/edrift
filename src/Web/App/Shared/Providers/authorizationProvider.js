(function() {
    var authorizationProvider = function ($http) {
        var self = this;
        var baseUrl = '/api/authorization';

        self.LogIn = function (login, password) {
            var model = { login: login, password: password };
            return $http.post(baseUrl + '/login', model);
        }

        self.LogOut = function () {
            return $http.get(baseUrl + '/logout');
        }
    };

    angular.module('boligdrift').service('authorizationProvider', ['$http', authorizationProvider]);

})();