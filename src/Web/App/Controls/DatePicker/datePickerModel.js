function DatePickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.date = new Date().toLocaleDateString();
    this.view = DatePickerViewType.month;
    this.format = DatePickerFormatType.dayMonthYear;
    this.haveStartDateRestriction = true;
    this.onSave = function () { };
    this.onChange = function () { };
    this.getDate = function () { };
    this.triggerRefresh = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.triggerValidate = function () { };
    this.triggerGetDate = function () { };
    this.securityKey = '';
    this.memberId = null;
}