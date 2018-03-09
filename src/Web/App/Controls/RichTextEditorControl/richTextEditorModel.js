function RichTextEditorModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.value = '';
    this.maxLength = 4000;
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerValidate = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}