var BootstrapDatePickerProvider = (function () {
    function BootstrapDatePickerProvider(settings) {
        this.dateHelper = new DateHelper(settings);
        this.setAvailableDates();
        this.setDefaultDates();
    }
    BootstrapDatePickerProvider.prototype.setStartDateElement = function (dateElement) {
        this.dateHelper.setStartDateElement(dateElement);
    };
    BootstrapDatePickerProvider.prototype.setEndDateElement = function (dateElement) {
        this.dateHelper.setEndDateElement(dateElement);
    };
    BootstrapDatePickerProvider.prototype.getStartDateWithTime = function () {
        var startDateElement = this.dateHelper.getStartDateElement();
        if (_.isNull(startDateElement)) {
            return null;
        }
        var startDate = this.getDate(startDateElement);
        return startDate;
    };
    BootstrapDatePickerProvider.prototype.getEndDateWithTime = function () {
        var endDateElement = this.dateHelper.getEndDateElement();
        if (_.isNull(endDateElement)) {
            return null;
        }
        var endDate = this.getDate(endDateElement);
        if (endDate) {
            endDate.setHours(23, 59, 59);
        }
        return endDate;
    };
    BootstrapDatePickerProvider.prototype.getDate = function (dateElement) {
        return $(dateElement).datepicker("getDate");
    };
    BootstrapDatePickerProvider.prototype.setMinStartDate = function (date) {
        var startDateElement = this.dateHelper.getStartDateElement();
        if (_.isNull(startDateElement)) {
            return null;
        }
        this.setMinDate(date, startDateElement);
    };
    BootstrapDatePickerProvider.prototype.setMaxStartDate = function (date) {
        var startDateElement = this.dateHelper.getStartDateElement();
        if (_.isNull(startDateElement)) {
            return null;
        }
        this.setMaxDate(date, startDateElement);
    };
    BootstrapDatePickerProvider.prototype.setMinEndDate = function (date) {
        var endDateElement = this.dateHelper.getEndDateElement();
        if (_.isNull(endDateElement)) {
            return null;
        }
        this.setMinDate(date, endDateElement);
    };
    BootstrapDatePickerProvider.prototype.setMaxEndDate = function (date) {
        var endDateElement = this.dateHelper.getEndDateElement();
        if (_.isNull(endDateElement)) {
            return null;
        }
        this.setMaxDate(date, endDateElement);
    };
    BootstrapDatePickerProvider.prototype.setDefaultDates = function () {
        var _this = this;
        _.defer(function () {
            var defaultStartDate = _this.dateHelper.getDefaultStartDate();
            if (!_.isNull(defaultStartDate)) {
                var startDateElement = _this.dateHelper.getStartDateElement();
                if (!_.isNull(startDateElement)) {
                    $(startDateElement).datepicker("setDate", defaultStartDate);
                }
            }
            var defaultEndDate = _this.dateHelper.getDefaultEndDate();
            if (!_.isNull(defaultEndDate)) {
                var endDateElement = _this.dateHelper.getEndDateElement();
                if (!_.isNull(endDateElement)) {
                    $(endDateElement).datepicker("setDate", defaultEndDate);
                }
            }
        });
    };
    BootstrapDatePickerProvider.prototype.setAvailableDates = function () {
        var _this = this;
        _.defer(function () {
            var startDateElement = _this.dateHelper.getStartDateElement();
            if (!_.isNull(startDateElement)) {
                _this.setMinDate(_this.dateHelper.settings.minStartDate, startDateElement);
                _this.setMaxDate(_this.dateHelper.settings.maxStartDate, startDateElement);
            }
            var endDateElement = _this.dateHelper.getEndDateElement();
            if (!_.isNull(endDateElement)) {
                _this.setMinDate(_this.dateHelper.settings.minEndDate, endDateElement);
                _this.setMaxDate(_this.dateHelper.settings.maxEndDate, endDateElement);
            }
        });
    };
    BootstrapDatePickerProvider.prototype.setMinDate = function (minDate, dateElement) {
        $(dateElement).datepicker("setStartDate", minDate);
    };
    BootstrapDatePickerProvider.prototype.setMaxDate = function (maxDate, dateElement) {
        $(dateElement).datepicker("setEndDate", maxDate);
    };
    return BootstrapDatePickerProvider;
}());
var DatePickerFactory = (function () {
    function DatePickerFactory() {
    }
    DatePickerFactory.getDatePickerProvider = function (settings) {
        return new BootstrapDatePickerProvider(settings);
    };
    return DatePickerFactory;
}());
var datePickerFactoryTypeChecker = DatePickerFactory;
//# sourceMappingURL=app-bootstrap-date-picker-provider.js.map