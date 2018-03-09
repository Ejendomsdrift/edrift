function WeeksPickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.value = [];
    this.currentRole = 0;
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerClearWeeks = function () { };
    this.triggerLockControl = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}