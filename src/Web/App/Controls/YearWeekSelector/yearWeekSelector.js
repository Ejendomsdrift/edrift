(function () {
    var yearWeekSelector = function () {
        var controller = function ($scope, $document, calendarProvider) {
            var self = this;
            var startWeek = 1;
            var endWeek = 52;

            self.rightArrowClicked = function () {
                self.config.isShowWeek ? self.increaseWeek() : self.increaseYear();
            }

            self.leftArrowClicked = function () {
                self.config.isShowWeek ? self.decreaseWeek() : self.decreaseYear();
            }

            self.increaseYear = function (isSkipChangeEvent) {
                var previousYear = self.selectedYear;
                self.selectedYear++;
                if (self.years.indexOf(self.selectedYear) < 0) {
                    self.years.splice(self.years.indexOf(previousYear), 0, self.selectedYear);
                    self.years.sort(function (a, b) { return a > b; });
                }

                if (!isSkipChangeEvent) {
                    self.changeDate();
                }
            }

            self.decreaseYear = function (isSkipChangeEvent) {
                var previousYear = self.selectedYear;
                self.selectedYear--;
                if (self.years.indexOf(self.selectedYear) < 0) {
                    self.years.splice(self.years.indexOf(previousYear) - 1, 0, self.selectedYear);
                    self.years.sort(function (a, b) { return a > b; });
                }

                if (!isSkipChangeEvent) {
                    self.changeDate();
                }
            }

            self.increaseWeek = function () {
                self.selectedWeek++;
                if (self.selectedWeek > endWeek) {
                    self.increaseYear(true);
                    self.selectedWeek = startWeek;
                }
                self.changeDate();
            }

            self.decreaseWeek = function () {
                self.selectedWeek--;
                if (self.selectedWeek < startWeek) {
                    self.decreaseYear(true);
                    self.selectedWeek = endWeek;
                }
                self.changeDate();
            }

            self.changeDate = function (year, week) {
                self.selectedYear = year || self.selectedYear;
                self.selectedWeek = week || self.selectedWeek;
                self.config.onChange(self.selectedYear, self.selectedWeek);
            }

            var getYears = function () {
                self.currentYear = new Date().getFullYear();
                if (self.config.isShowYear) {
                    self.years = [0, 1, 2, 3, 4].map(function (v) { return self.currentYear + v; });

                    if (self.config.selectedYear) {
                        if (self.years.indexOf(self.config.selectedYear) < 0) {
                            self.years[0] > self.config.selectedYear ? self.years.unshift(self.config.selectedYear) : self.years.push(self.config.selectedYear);
                        }
                        self.selectedYear = self.config.selectedYear;
                    } else {
                        self.selectedYear = self.years[0];
                    }
                } else {
                    self.selectedYear = self.currentYear;
                }

                if (!self.config.isShowWeek) {
                    self.changeDate();
                } else {
                    loadWeeks();
                }
            }

            function loadWeeks() {
                calendarProvider.getTotalWeeks().then(getWeeksCallback);
            }

            function getWeeksCallback(result) {
                self.totalWeeks = result.totalWeeks;
                self.currentWeek = result.currentWeek;
                self.selectedWeek = self.config.selectedWeek ? self.config.selectedWeek : self.currentWeek;
                self.changeDate();
            }

            getYears();

            var documentOnKeyPress = function (evt) {
                $scope.$apply(function () {
                    if (evt.which === KeyCodes.RightArrow) {
                        self.rightArrowClicked();
                    }
                    else if (evt.which === KeyCodes.LeftArrow) {
                        self.leftArrowClicked();
                    }
                });
            };

            $document.on('keydown', documentOnKeyPress);

            $scope.$on('$destroy', function () {
                $document.off('keydown', documentOnKeyPress);
            });
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/YearWeekSelector/yearWeekSelector.html',
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'yearWeekSelectorCtrl',
            controller: ['$scope', '$document', 'calendarProvider', controller]
        };
    }

    angular.module('boligdrift').directive('yearWeekSelector', [yearWeekSelector]);
})();