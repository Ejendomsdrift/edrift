function PerWeekJobSchedulePickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.repeatsPerWeek = 1;
    this.dayPerWeekList = [];
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}