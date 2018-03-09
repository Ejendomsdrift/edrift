(function () {
    var focus = function ($timeout) {
        var self = {
            restrict: 'A'
        };

        self.link = function (scope, element) {
            $timeout(function () {
                element[0].focus();
            }, 700);
        };

        return self;
    };

    angular.module('boligdrift').directive('focus', ['$timeout', focus]);
})();