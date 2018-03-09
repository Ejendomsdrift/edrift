function TimePickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.time = new Date();
    this.hoursStep = 1;
    this.minutesStep = 5;
    this.pastTimeAlowed = false;
    this.showMeridian = false;
    this.onSave = function () { };
    this.getTime = function () { };
    this.triggerRefresh = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.triggerValidate = function () { };
    this.securityKey = '';
}