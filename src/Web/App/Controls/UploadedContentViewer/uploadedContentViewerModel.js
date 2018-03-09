function UploadedContentViewerModel(mode) {
    this.mode = mode ? mode : ControlMode.disable;
    this.value = [];
    this.maxLength = 400;
    this.contentType = ContentType.ImagesAndVideos;
    this.onSave = function () { };
    this.triggerRefresh = function () { };
    this.triggerCopySavedData = function () { };
    this.triggerCheckControlChanged = function () { };
    this.securityKey = '';
}