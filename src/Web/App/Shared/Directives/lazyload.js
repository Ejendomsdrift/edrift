(function () {
    var lazyLoad = function ($rootScope, $window) {
        var self = {
            restrict: 'A'
        };

        self.link = function (scope, elem, attrs) {
            var checkWhenEnabled, handler, scrollDistance, isStickHeader;
            $window = angular.element($window);
            scrollDistance = 0;
            if (attrs.lazyLoadDistance != null) {
                scope.$watch(attrs.lazyLoadDistance, function (value) {
                    return scrollDistance = parseInt(value, 10);
                });
            }
            isStickHeader = false;
            if (attrs.lazyLoadStickHeader != null) {
                scope.$watch(attrs.lazyLoadStickHeader, function (value) {
                    return scrollDistance = isStickHeader = value;
                });
            }

            handler = function () {
                if (isStickHeader) {
                    stickHeader();
                }
                var elementBottom, remaining, shouldScroll, windowBottom;
                windowBottom = $window.height() + $window.scrollTop();
                elementBottom = elem.offset().top + elem.height();
                remaining = elementBottom - windowBottom;
                shouldScroll = remaining <= $window.height() * scrollDistance;
                if (shouldScroll) {
                    if ($rootScope.$$phase) {
                        return scope.$eval(attrs.lazyLoad);
                    } else {
                        return scope.$apply(attrs.lazyLoad);
                    }
                }
            };

            stickHeader = function () {
                var tableStickyHeader = $(".js-sticky-thead");
                var tableStickyHeaderHeight = tableStickyHeader.outerHeight();
                var stickyCategoryTree = $(".js-category-filter");
                var headerHeight = $(".js-header").outerHeight();

                if (tableStickyHeader.length && stickyCategoryTree.length) {
                    if (window.scrollY >= tableStickyHeader.offset().top - tableStickyHeaderHeight) {
                        tableStickyHeader.addClass("_sticky");
                        stickyCategoryTree.addClass("_sticky");
                        tableStickyHeader.css('top', headerHeight);
                    }
                    else if (window.scrollY <= 386 - tableStickyHeaderHeight) {
                        tableStickyHeader.removeClass("_sticky");
                        stickyCategoryTree.removeClass("_sticky");
                    }
                }
            };

            $window.on('scroll', handler);
            scope.$on('$destroy', function () {
                return $window.off('scroll', handler);
            });
        };

        return self;
    };

    angular.module('boligdrift').directive('lazyLoad', ['$rootScope', '$window', lazyLoad]);
})();