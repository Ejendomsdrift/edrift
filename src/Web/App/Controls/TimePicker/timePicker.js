(function () {
    var timePicker = function () {
        var controller = function (dateHelper) {
            var self = this;

            self.config.triggerRefresh = function () {
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerValidate = function (dateInMilliseconds) {
                self.includeValidation = true;
                var currentDateInMilliseconds = new Date().getTime();
                self.isValidValue = dateInMilliseconds > currentDateInMilliseconds;
                return self.isValidValue;
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.time;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.time;
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit || self.editControlConfig.mode == ControlMode.create;
            }

            self.changeTime = function () {
                if (self.editControlConfig.mode == ControlMode.create) {
                    var date = dateHelper.parseDateFromDatePickerFormat(self.config.time);
                    self.config.onSave(date);
                }
            }

            self.isTimeValid = function () {
                return !self.includeValidation || self.isValidValue;
            }

            self.config.getTime = function() {
                return dateHelper.parseDateFromDatePickerFormat(self.config.time);
            }

            function correctMinutes() {
                var minutes = self.config.time.getMinutes();
                if (minutes % 10 !== 0 && minutes % 5 !== 0) {
                    var difference = Math.ceil(minutes - (minutes - minutes % 10));
                    self.config.time.setMinutes(minutes - difference);
                }
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    var date = dateHelper.parseDateFromDatePickerFormat(self.config.time);
                    self.config.onSave(date);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.time = self.savedValue;
                self.includeValidation = false;
                self.isValidValue = true;
            }

            function initControl() {
                self.includeValidation = false;
                correctMinutes();

                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.config.triggerCopySavedData();
            }

            initControl();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/TimePicker/timePicker.html',
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'timePickerCtrl',
            controller: ['dateHelper', controller]
        }
    }

    angular.module('boligdrift').directive('timePicker', [timePicker]);
})();