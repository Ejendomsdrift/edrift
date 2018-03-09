function SelectControlModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.selected = null;
    this.dataList = [];
    this.placeholder = '';
    this.onUpdate = function () { };
    this.onRemove = function () { };
    this.triggerSetValue = function () { };
    this.triggerUpdateDataList = function () { };
    this.showAll = false;
    this.showAllTranslationKey = '';
}