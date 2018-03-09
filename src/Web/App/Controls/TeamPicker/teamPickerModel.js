function TeamPickerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.timeViewDayScope = null;
    this.userIdList = [];
    this.groupId = null;
    this.teamLeadId = null;
    this.isAssignedToAllUsers = true;
    this.isRequired = true;
    this.onChange = function () { };
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
    this.selectedGroup = null;
    this.isOpenedState = false;
    this.isUnassignAll = false;
}