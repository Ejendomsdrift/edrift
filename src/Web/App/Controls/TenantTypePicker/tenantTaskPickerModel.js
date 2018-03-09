function TenantTaskPickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.value = null;
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}