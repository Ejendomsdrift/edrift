(function () {
    var httpInterceptorService = function ($window, $q) {
        var self = this;
        var unauthorizedStatus = 401;

        self.responseError = function (response) {
            if (response && response.status === unauthorizedStatus) {
                var headers = response.headers();
                $window.location.href = headers.loginredirecturl;
            }
            return $q.reject(response);
        };
    };

    angular.module('boligdrift').service('httpInterceptorService', ['$window', '$q', httpInterceptorService]);
})();
