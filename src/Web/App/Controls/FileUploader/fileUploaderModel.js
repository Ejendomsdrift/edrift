function FileUploaderModel() {
    this.contentTypes = [ContentType.Document, ContentType.Image];
    this.onSuccess = function () { };
    this.onFileAdded = function () { };
    this.onGetQueryParams = function () { };
    this.triggerUploadFile = {};
    this.autoSave = true;
    this.contentUrl = '';
    this.ngFlowConfig = {
        target: '/api/files/upload',
        permanentErrors: [404, 500, 501],
        chunkSize: 9007199254740992,
        maxChunkRetries: 1,
        chunkRetryInterval: 5000,
        singleFile: true,
        testChunks: false
    };
}