﻿function AddressControlModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.address = "";
    this.isRequired = false;
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
    this.departmentId = '';
}