(function () {
    var security = function (securityService) {
        var self = {
            restrict: 'A',
            scope: {
                page: '='
            }
        };

        self.link = function (scope, element, attrs) {
            var page = scope.page || null;
            var elementsList = Array.prototype.slice.call(element[0].querySelectorAll('[data-access-key]'));
            var accessKeyList = [];
            var mappedElementsList = elementsList.map(function (item) {
                var angularElement = angular.element(item);
                angularElement.hide();
                var key = item.getAttribute('data-access-key');
                accessKeyList.push(key);
                return { element: angularElement, key: key };
            });

            var model = { keyList: accessKeyList, page: page };
            securityService.hasAccessByKeyList(model).then(function (result) {
                Object.keys(result.data).forEach(function (key) {
                    var accessElement = mappedElementsList.find(function (item) { return item.key.toLowerCase() === key.toLowerCase() }).element;
                    if (!result.data[key]) {
                        accessElement.remove();
                    }
                    else {
                        accessElement.show();
                    }
                });
            });
        };

        return self;
    }

    angular.module('boligdrift').directive('security', ['securityService', security]);
})();
