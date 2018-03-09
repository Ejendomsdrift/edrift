(function () {
    var confirmation = function ($filter) {
        var self = {
            priority: 100,
            restrict: 'A',
            scope: {
                confirm: '=',
                cancel: '='
            }
        };
         
        self.link = function (scope, element, attrs) {
            var self = this;
            self.scope = scope;

            var msg = $filter('translate')(attrs.confirmation || 'Are you sure?');

            element.bind('click', function (event) {
                if (confirm(msg)) {
                    if (self.scope.confirm) {
                        self.scope.confirm();
                    }
                } else {
                    if (self.scope.cancel) {
                        self.scope.cancel();
                    } else {
                        event.stopImmediatePropagation();
                        event.preventDefault();
                    }
                }
            });
        };

        return self;
    };

    angular.module('boligdrift').directive('confirmation', ['$filter', confirmation]);
})();