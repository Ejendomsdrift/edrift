(function () {
    var datePicker = function () {
        var controller = function (dateHelper, moment) {
            var self = this;

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerValidate = function () {
                self.includeValidation = true;
                self.isValidValue = angular.isDefined(self.config.date) && self.config.date !== null && self.config.date !== '' && isValidDate();
                return self.isValidValue;
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.date;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.date;
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit || self.editControlConfig.mode == ControlMode.create;
            }

            self.isViewMode = function () {
                return self.editControlConfig.mode == ControlMode.view;
            }

            self.changeDate = function() {
                if (self.editControlConfig.mode == ControlMode.create) {
                    var date = dateHelper.parseDateFromDatePickerFormat(self.config.date);
                    self.config.onChange(date, self.config.memberId);
                }
            }

            self.config.getDate = function() {
                return dateHelper.parseDateFromDatePickerFormat(self.config.date);
            }

            self.isDateValid = function() {
                return !self.includeValidation || self.isValidValue;
            }

            self.config.triggerGetDate = function() {
                return dateHelper.parseDateFromDatePickerFormat(self.config.date);
            }

            function isValidDate() {
                return self.config.view == DatePickerViewType.years ? validateYear() : validateDate();
            }

            function validateYear() {
                var selectedYear = dateHelper.parseYearFromDatePickerFormat(self.config.date);
                var currentYear = moment.utc().year();
                var result = selectedYear.year() >= currentYear;

                return result;
            }

            function validateDate() {
                var currentDate = moment.utc();
                var selectedDate = dateHelper.parseDateFromDatePickerFormat(self.config.date);

                var result = !selectedDate.isBefore(currentDate, 'day');
                return result;
            }

            function saveHandler() {
                if (!self.config.triggerValidate()) {
                    return false;
                }

                if (self.config.triggerCheckControlChanged()) {
                    var date = self.config.view == DatePickerViewType.years
                        ? dateHelper.parseYearFromDatePickerFormat(self.config.date)
                        : dateHelper.parseDateFromDatePickerFormat(self.config.date);

                    self.config.onSave(date);

                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.date = self.savedValue;
                $('.datepicker').datepicker('update', self.savedValue.toString());
                self.includeValidation = false;
                self.isValidValue = true;
            }

            function initControl() {
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
            templateUrl: '/App/Controls/DatePicker/datePicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'datePickerCtrl',
            controller: ['dateHelper', 'moment', controller]
        };
    }

    angular.module('boligdrift').directive('datePicker', [datePicker]);
})();