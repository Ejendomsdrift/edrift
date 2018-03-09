var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var DownloadFileFactory = (function () {
    function DownloadFileFactory() {
    }
    DownloadFileFactory.getDownloadFileFactory = function (dataProvider, dateProvider, originContentSettings) {
        return new XhrBlobDownloadFileProvider(dataProvider, dateProvider, originContentSettings);
    };
    return DownloadFileFactory;
}());
var downloadFileFactoryTypeChecker = DownloadFileFactory;
var XhrBlobDownloadFileProvider = (function (_super) {
    __extends(XhrBlobDownloadFileProvider, _super);
    function XhrBlobDownloadFileProvider(dataProvider, dateProvider, originContentSettings) {
        var _this = _super.call(this, dateProvider) || this;
        _this.dataProvider = dataProvider;
        _this.contentSettings = {
            columnDelimiter: "	",
            lineDelimiter: "\n",
            blobPropertyBagType: "application/octet-stream"
        };
        if (originContentSettings) {
            _this.contentSettings = _.extendOwn(_this.contentSettings, originContentSettings);
        }
        return _this;
    }
    XhrBlobDownloadFileProvider.prototype.downloadFile = function (chartBlock, isParseResponse) {
        var _this = this;
        var fileChartModel = this.getFileChartModel(chartBlock);
        this.dataProvider.post(chartBlock.settings.fileSettings.url, fileChartModel, function (response, xhr) {
            if (_.isString(response)) {
                var fileName = _this.getFileName(xhr);
                _this.downloadFileFromString(response, fileName, chartBlock);
            }
            else if (_.isObject(response)) {
                var fileResultModel = response;
                if (_.isString(fileResultModel.content)) {
                    _this.downloadFileFromString(fileResultModel.content, fileResultModel.fileName, chartBlock);
                }
                else {
                    _this.downloadFileFromArray(fileResultModel.content, fileResultModel.fileName, chartBlock);
                }
            }
        }, this.dataProvider.processErrorResponse, isParseResponse);
    };
    XhrBlobDownloadFileProvider.prototype.downloadFileFromString = function (content, filename, chartBlock) {
        var data = new Blob([this.stringToArrayBuffer(content)], { type: this.contentSettings.blobPropertyBagType });
        var disableAutoBOM = true;
        if (chartBlock.settings.fileSettings.fileDownloadCallback) {
            chartBlock.settings.fileSettings.fileDownloadCallback(filename);
        }
        saveAs(data, filename, disableAutoBOM);
    };
    XhrBlobDownloadFileProvider.prototype.downloadFileFromArray = function (dataArray, filename, chartBlock) {
        var csvString = this.convertArrayOfObjectsToCSV(dataArray);
        if (!csvString) {
            return;
        }
        this.downloadFileFromString(csvString, filename, chartBlock);
    };
    XhrBlobDownloadFileProvider.prototype.convertArrayOfObjectsToCSV = function (dataArray) {
        var _this = this;
        if (dataArray == null || !dataArray.length) {
            return;
        }
        var result = "";
        dataArray.forEach(function (data) {
            var columns = _.values(data);
            result += columns.join(_this.contentSettings.columnDelimiter);
            result += _this.contentSettings.lineDelimiter;
        });
        return result;
    };
    XhrBlobDownloadFileProvider.prototype.stringToArrayBuffer = function (str) {
        var buf = new ArrayBuffer(str.length * 2);
        var bufView = new Uint16Array(buf);
        for (var i = 0, strLen = str.length; i < strLen; i++) {
            bufView[i] = str.charCodeAt(i);
        }
        return buf;
    };
    XhrBlobDownloadFileProvider.prototype.getFileName = function (xhr) {
        var disposition = xhr.getResponseHeader('Content-Disposition');
        if (!disposition || disposition.indexOf('attachment') === -1) {
            return "";
        }
        var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        var matches = filenameRegex.exec(disposition);
        if (matches != null && matches[1]) {
            return matches[1].replace(/['"]/g, '');
        }
        else {
            return "";
        }
    };
    return XhrBlobDownloadFileProvider;
}(BaseDownloadFileProvider));
//# sourceMappingURL=app-xhr-blob-download-file-provider.js.map