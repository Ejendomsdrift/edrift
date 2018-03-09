function PhoneInputControlModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.mask = PhoneMask.Danish;
    this.value = '';
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}