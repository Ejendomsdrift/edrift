(function () {
    var userAvatar = function () {
        var self = {
            restrict: 'E',
            templateUrl: '/App/Controls/UserAvatar/userAvatar.html',
            replace: true,
            scope: {
                config: '='
            }
        };

        self.link = function (scope, element, attr) {
            var defUrl = '/Content/images/default_avatar.png';

            scope.$watch(function () { return scope.config.url }, function (newUrl) {
                scope.avatarUrl = getUrl(newUrl);
            });

            element.bind('error', function () {
                angular.element(this).attr("src", defUrl);
            });

            scope.config.triggerChangeUrl = function (url) {
                scope.config.url = url;
                scope.avatarUrl = getUrl(url);
            }

            function getUrl(url) {
                return url ? url : defUrl;
            }

            function initClass(url) {
                switch (scope.config.size) {
                    case "medium":
                        scope.avatarClass = "medium-avatar";
                        break;
                    case "large":
                        scope.avatarClass = "large-avatar";
                        break;
                    default:
                        scope.avatarClass = "small-avatar";
                        break;
                }
            }

            function initControl() {
                scope.avatarUrl = getUrl(scope.config.url);
                initClass();
            }

            initControl();
        }

        return self;
    }

    angular.module('boligdrift').directive('userAvatar', [userAvatar]);
})();