(function () {
    var daysPerWeekPicker = function () {
        var controller = function () {
            var self = this;

            self.changeDay = function (day, index) {
                if (self.config.isRequired && !day.isChecked) {
                    day.isChecked = true;
                    return;
                }
                
                if (day.isChecked && self.config.value.length >= self.config.allowedDaysCount) {
                    self.lastSelectedIndex = self.lastSelectedIndex === null ? self.config.value[0].weekDay - 1 : self.lastSelectedIndex;
                    if (self.lastSelectedIndex > -1) {
                        self.weekDays[self.lastSelectedIndex].isChecked = false;
                    }
                }

                if (day.isChecked) {
                    self.lastSelectedIndex = index;
                }

                self.config.value = self.weekDays.filter(function (item) { return item.isChecked; });

                if (self.editControlConfig.mode == ControlMode.create) {
                    updateSelectedDays();
                    self.config.onSave(self.config.value);
                    self.config.triggerCopySavedData();
                }
                else {
                    self.config.onChange(self.config.value);
                }
            }

            self.isWeekend = function (day) {
                return day === WeekDays.Sat || day === WeekDays.Sun;
            }

            self.config.triggerRefresh = function () {
                if (self.config.value.length > self.config.allowedDaysCount) {
                    clearDays();
                }
                applyModelToControl();
                self.editControlConfig.mode = self.config.mode;
                self.includeValidation = false;
                self.config.triggerCopySavedData();
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.value;
            }

            self.config.triggerCheckControlChanged = function () {
                if (self.savedValue && self.savedValue.lenght) {
                    return self.savedValue.weekDay !== self.config.value[0].weekDay;
                } else {
                    return true;
                }
            }

            self.config.triggerValidate = function() {
                self.includeValidation = true;
                self.isValidValue = angular.isDefined(self.config.value) && self.config.value !== null && self.config.value.length > 0;
                return self.isValidValue;
            }

            self.isWeekend = function(day) {
                return day === WeekDays.Sat || day === WeekDays.Sun;
            }

            self.isDayValid = function() {
                return !self.includeValidation || self.isValidValue;
            }

            function applyModelToControl() {
                self.weekDays.forEach(function (day) {
                    day.isChecked = isChecked(day);
                });
            }

            function isChecked(day) {
                var checkedDay = self.config.value.find(function (assignedDay) {
                    return assignedDay.weekDay === day.weekDay;
                });

                return checkedDay ? true : false;
            }

            function clearDays() {
                self.weekDays.forEach(function (item) {
                    item.isChecked = false;
                });
            }

            function updateSelectedDays() {
                self.config.value = self.weekDays
                                .filter(function (item) { return item.isChecked; })
                                .map(function (item) { return {id: '', weekDay: item.weekDay} });
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    updateSelectedDays();
                    self.config.onSave(self.config.value);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.includeValidation = false;
                self.isValidValue = true;
                self.config.value = self.savedValue;
                clearDays();
                self.config.triggerRefresh();
            }

            function initControl() {
                self.weekDays = [];
                self.lastSelectedIndex = null;

                Object.keys(WeekDays).forEach(function (key) {
                    var day = {
                        weekDay: WeekDays[key],
                        alias: key,
                        isChecked: false
                    };

                    self.weekDays.push(day);
                });
                applyModelToControl();
                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;
            }

            initControl();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/DaysPerWeekPicker/daysPerWeekPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'daysPerWeekPickerCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('daysPerWeekPicker', [daysPerWeekPicker]);
})();