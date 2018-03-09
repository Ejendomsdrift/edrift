(function () {
    var perWeekJobSchedulePicker = function () {
        var controller = function () {
            var self = this;

            self.changeDay = function (day, index) {
                if (day.isChecked) {
                    self.checkedDaysPerWeekCount++;
                } else {
                    self.checkedDaysPerWeekCount--;
                }

                if (day.isChecked && self.checkedDaysPerWeekCount > self.config.repeatsPerWeek) {
                    self.lastSelectedIndex = self.lastSelectedIndex === null ? self.config.dayPerWeekList[0].weekDay - 1 : self.lastSelectedIndex;
                    self.weekDays[self.lastSelectedIndex].isChecked = false;
                    self.checkedDaysPerWeekCount--;
                }

                if (day.isChecked) {
                    self.lastSelectedIndex = index;  
                } 
            }

            self.changeRepeatPerWeek = function () {
                if (self.checkedDaysPerWeekCount > self.config.repeatsPerWeek) {
                    clearDays();
                    self.checkedDaysPerWeekCount = 0;
                }
            }

            self.config.triggerRefresh = function () {
                self.weekDays.forEach(function (day) {
                    day.isChecked = isChecked(day);
                });

                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerCopySavedData = function () {
                self.savedPerWeek = self.config.repeatsPerWeek;
                self.savedWeekDays = angular.copy(self.weekDays);
                self.checkedDaysPerWeekCount = self.config.dayPerWeekList.length;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedPerWeek !== self.config.repeatsPerWeek || isDayPerWeekListMatch();
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit;
            }

            self.isWeekend = function (day) {
                return day === WeekDays.Sat || day === WeekDays.Sun;
            }

            function isChecked(day) {
                var checkedDay = self.config.dayPerWeekList.find(function (assignedDay) {
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
                self.dayPerWeekList = self.weekDays
                                .filter(function (item) { return item.isChecked; })
                                .map(function (item) { return { id: '', weekDay: item.weekDay } });
            }

            function isDayPerWeekListMatch() {
                for (var i = 0; i < self.weekDays.length; i++) {
                    if (self.weekDays[i].isChecked !== self.savedWeekDays[i].isChecked) {
                        return true;
                    }
                }

                return false;
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    self.lastSelectedIndex = null;
                    updateSelectedDays();
                    self.config.onSave(self.dayPerWeekList, self.config.repeatsPerWeek);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.repeatsPerWeek = self.savedPerWeek;
                self.weekDays = angular.copy(self.savedWeekDays);
                self.checkedDaysPerWeekCount = self.savedPerWeek;
                self.lastSelectedIndex = null;
            }

            function initDaysPerWeekControl() {
                self.weekDays = [];
                self.lastSelectedIndex = null;
                self.checkedDaysPerWeekCount = 0;

                Object.keys(WeekDays).forEach(function (key) {
                    var day = {
                        weekDay: WeekDays[key],
                        alias: key,
                        isChecked: false
                    };

                    self.weekDays.push(day);
                });
            }

            function initEditControl() {
                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;
            }

            function init() {
                initDaysPerWeekControl();
                initEditControl();
            }

            init();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/PerWeekJobSchedulePicker/perWeekJobSchedulePicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'jobScheduleCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('perWeekJobSchedulePicker', [perWeekJobSchedulePicker]);
})();