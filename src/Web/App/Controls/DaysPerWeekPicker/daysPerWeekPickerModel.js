function DaysPerWeekPickerModel(mode, date) {
    this.mode = mode ? mode : ControlMode.disable;
    this.isRequired = false;
    this.value = date ? [date] : [];
    this.allowedDaysCount = 1;
    this.onSave = function () { };
    this.onChange = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}