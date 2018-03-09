function MemberPickerModel(isMultiple, mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.selectedIds = [];
    this.selectedMembers = [];
    this.selectedId = null;
    this.selectedMember = angular.noop;
    this.members = [];
    this.defaultTextKey = null;
    this.isMultiple = isMultiple ? isMultiple : false;
    this.onSelect = function (memberId) { };
    this.onRemove = function (memberId) { };
    this.onSave = function () { };
    this.triggerClear = function () { };
    this.triggerRefresh = function () { };
    this.showTimeView = false;
}