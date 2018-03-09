(function () {
    var nativeCarousel = function ($filter, $window) {
        var self = {
            restrict: 'E',
            templateUrl: '/App/Controls/NativeCarousel/nativeCarousel.html',
            scope: {
                slides: '=',
                config: '=',
                defaultSlideWidth: '@'
            }
        };

        self.link = function (scope, element) {
            scope.showCarousel = true;
            scope.defaultSlideWidth = scope.defaultSlideWidth ? scope.defaultSlideWidth : 793;
            scope.$window = $window;
            initSlides(scope.slides);

            scope.showNext = function () {
                scope.currentSlide++;
                if (scope.currentSlide > scope.filteredSlides.length - 1)
                    scope.currentSlide = 0;
                scope.slideOffset = scope.slideWidth * scope.currentSlide;
            };

            scope.showPrev = function () {
                scope.currentSlide--;
                if (scope.currentSlide < 0)
                    scope.currentSlide = scope.filteredSlides.length - 1;
                scope.slideOffset = scope.slideWidth * scope.currentSlide;
            };

            scope.getElementDimensions = function () {
                return { 'h': element.height(), 'w': element.width() };
            };

            var resizeListener = scope.$watch(scope.getElementDimensions, function (newValue, oldValue) {
                updateWidth(newValue);
            }, true);

            function updateWidth(size, window) {
                var width = getWidth();
                scope.sliderWidth = scope.filteredSlides ? width * scope.filteredSlides.length : 0;
                scope.slideWidth = width;
                scope.slideOffset = scope.slideWidth * scope.currentSlide;
                scope.showCarousel = true;
            };

            var modelListener = scope.$watch('slides', function (v) {
                initSlides(v);
            });

            scope.$on('$destroy', function () {
                resizeListener();
                modelListener();
            });

            angular.element(window).on('resize', function () {
                scope.$apply(function () {
                    scope.showCarousel = false;
                    updateWidth(scope.getElementDimensions(), $window);
                });
            });

            function getWidth() {
                return scope.$window.innerWidth > scope.defaultSlideWidth ?
                       scope.defaultSlideWidth :
                       scope.$window.innerWidth - 54;
            }

            function initSlides(slides) {
                scope.filteredSlides = $filter('filter')(slides, scope.config.filter);
                scope.currentSlide = 0;
                scope.slideOffset = 0;
            };
        };

        return self;
    }

    angular.module('boligdrift').directive('nativeCarousel', ['$filter', '$window', nativeCarousel]);
})();