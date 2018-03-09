(function () {
    var enter = function () {
        var self = {
            restrict: 'A'
        };

        self.link = function (scope, element, attrs) {
            element.bind("keydown keypress", function (event) {
                if (event.which === 13) {
                    scope.$apply(function () {
                        scope.$eval(attrs.enter);
                    });

                    event.preventDefault();
                }
            });
        };

        return self;
    }

    angular.module('boligdrift').directive('enter', [enter]);
})();
