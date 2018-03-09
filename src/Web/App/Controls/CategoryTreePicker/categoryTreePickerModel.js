function CategoryTreePickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.value = '';
    this.categoryTreeConfig = {};
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}