(function () {
    var weeksPicker = function () {
        var controller = function () {
            var self = this;
            var WEEKS_IN_YEAR = 52;
            var WeekChangedBy = Object.freeze({ Administrator: 1, Coordinator: 2 });
            self.changedByAdministratorAndCoordinator |= WeekChangedBy.Administrator | WeekChangedBy.Coordinator;
            self.allWeeksSelected = false;
            self.allWeeksDeselected = false;

            self.config.triggerCopySavedData = function () {
                self.savedValue = angular.copy(self.weeks);
            }

            self.config.triggerCheckControlChanged = function () {
                for (var i = 0; i < self.weeks.length; i++) {
                    if (self.weeks[i].isChecked !== self.savedValue[i].isChecked ||
                        self.weeks[i].changedBy !== self.savedValue[i].changedBy) {
                        return true;
                    }
                }

                return false;
            }

            self.weekOnClick = function (week) {
                if (self.editControlConfig.mode == ControlMode.disable) {
                    return;
                }

                if (!week) {
                    return;
                }

                if (week.changedBy === 0 ||
                   (week.changedBy & self.config.currentRole) === self.config.currentRole) {
                    week.isChecked = !week.isChecked;
                }

                week.changedBy = getChangedBy(week.changedBy);
                week.cssClass = getWeekCssClass(week.changedBy);

                setButtonsFlag();
            }

            self.selectAllWeeks = function () {
                self.weeks.forEach(function (week) {
                    if (!week.isChecked || (week.changedBy & self.config.currentRole) !== self.config.currentRole) {
                        week.isChecked = true;
                        week.changedBy = getChangedBy(week.changedBy);
                        week.cssClass = getWeekCssClass(week.changedBy);
                    }
                });
                manageButtonsFlags(true);
            }

            self.deselectAllWeeks = function () {
                self.weeks.forEach(function (week) {
                    if (week.isChecked && (week.changedBy & self.config.currentRole) === self.config.currentRole) {
                        week.isChecked = false;
                        week.changedBy = getChangedBy(week.changedBy);
                        week.cssClass = getWeekCssClass(week.changedBy);
                    }
                });
                manageButtonsFlags(false);
            }

            self.config.triggerRefresh = function () {
                self.config.value.forEach(function (week) {
                    self.weeks[week.number - 1].isChecked = true;
                    self.weeks[week.number - 1].changedBy = week.changedBy;
                    self.weeks[week.number - 1].cssClass = getWeekCssClass(week.changedBy);
                });

                setButtonsFlag();
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerLockControl = function (value) {
                self.editControlConfig.mode = value ? ControlMode.disable : ControlMode.view;
            }

            self.config.triggerClearWeeks = function () {
                initWeeks();
            }

            function getChangedBy(changedBy) {
                if ((changedBy & self.changedByAdministratorAndCoordinator) === self.changedByAdministratorAndCoordinator ||
                    (changedBy & self.config.currentRole) === self.config.currentRole) {
                    changedBy ^= self.config.currentRole;
                    return changedBy;
                } else {
                    changedBy |= self.config.currentRole;
                    return changedBy;
                }
            }

            function getWeekCssClass(changedBy) {
                switch (changedBy) {
                    case self.changedByAdministratorAndCoordinator:
                        return '_grey-box _border';
                    case WeekChangedBy.Administrator:
                        return '_grey-box';
                    case WeekChangedBy.Coordinator:
                        return '_border';
                    default:
                        return '';
                }
            }

            function checkIfAllWeekSelected() {
                for (var i = 0; i < self.weeks.length; i++) {
                    if (self.weeks[i].changedBy !== self.config.currentRole &&
                        self.weeks[i].changedBy !== self.changedByAdministratorAndCoordinator) {
                        return false;
                    }
                }
                return true;
            }

            function checkIfAllWeekDeselected() {
                for (var i = 0; i < self.weeks.length; i++) {
                    if (self.weeks[i].changedBy === self.config.currentRole ||
                        self.weeks[i].changedBy === self.changedByAdministratorAndCoordinator) {
                        return false;
                    }
                }
                return true;
            }

            function updateSelectedWeeks() {
                self.config.value = self.weeks
                                .filter(function (item) { return item.changedBy !== 0; })
                                .map(function (item) { return { number: item.number, changedBy: item.changedBy } });
            }


            function setButtonsFlag() {
                self.allWeeksSelected = checkIfAllWeekSelected();
                self.allWeeksDeselected = checkIfAllWeekDeselected();
            }

            function manageButtonsFlags(value) {
                self.allWeeksSelected = value;
                self.allWeeksDeselected = !value;
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    updateSelectedWeeks();
                    self.config.onSave(self.config.value);
                    self.config.triggerCopySavedData();
                    setButtonsFlag();
                }

                return true;
            }

            function cancelHandler() {
                self.weeks = angular.copy(self.savedValue);
                setButtonsFlag();
            }

            function initWeeks() {
                self.weeks = [];
                for (var i = 0; i < WEEKS_IN_YEAR; i++) {
                    self.weeks.push({
                        number: i + 1,
                        changedBy: 0,
                        isChecked: false,
                        cssClass: ''
                    });
                }
            }

            function init() {
                initWeeks();

                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.config.triggerCopySavedData();
            }

            init();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/WeeksPicker/weeksPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'weeksPickerCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('weeksPicker', [weeksPicker]);
})();