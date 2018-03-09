(function () {
    var multiSelectChecker = function ($compile) {
        return {
            restrict: 'A',
            terminal: true,
            priority: 50000,
            compile: function compile(element, attrs) {
                element.removeAttr("multi-select-checker");
                element.removeAttr("data-multi-select-checker");

                return {
                    pre: function preLink(scope, iElement, iAttrs, controller) { },
                    post: function postLink(scope, iElement, iAttrs, controller) {
                        if (scope.isMultiple) {
                            iElement[0].setAttribute('multiple', '');
                        }
                        $compile(iElement)(scope);
                    }
                };
            }
        };
    }

    angular.module('boligdrift')
        .directive('multiSelectChecker', [
            '$compile',
            multiSelectChecker
        ]);
})();