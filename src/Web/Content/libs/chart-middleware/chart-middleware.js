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
var ChartMiddleware = (function () {
    function ChartMiddleware(settings) {
        var _this = this;
        this.settings = {};
        this.allBlocksReady = function () {
            _this.stackWorker.run("allBlocksReady");
        };
        _.extendOwn(this.settings, settings);
        var dataProvider = new DataProvider();
        this.dateProvider = DatePickerFactory.getDatePickerProvider(this.settings.datePickerSettings);
        var downloadFileProvider = DownloadFileFactory.getDownloadFileFactory(dataProvider, this.dateProvider);
        this.fileBlockService = new FileBlockService(downloadFileProvider);
        this.stackWorker = StackWorkerFactory.StackWorker;
        this.pipeline = new ChartMiddlewarePipeline();
        this.entityDataProvider = new EntityDataProvider(dataProvider);
        this.dropDownService = new DropDownService(this.entityDataProvider, this.stackWorker, this.pipeline, this.dateProvider);
        this.chartProvider = ChartFactory.getChartProvider();
        this.validateChartProvider();
        this.chartBlockService = new ChartBlockService(this.chartProvider, this.entityDataProvider, this.dropDownService, this.pipeline, this.dateProvider, this.settings);
        this.internalInit();
    }
    ChartMiddleware.prototype.addChartBlock = function (chartBlockSettings) {
        var groupedChartBlock = this.chartBlockService.createGroupedChartBlock(chartBlockSettings);
        var chartBlock = this.chartBlockService.addChartBlock(groupedChartBlock, chartBlockSettings);
        this.chartBlockService.addGroupedChartBlock(groupedChartBlock);
        var result = this.createChartMiddlewareObject(chartBlock, groupedChartBlock);
        return result;
    };
    ChartMiddleware.prototype.addCollectionOfChartBlocks = function (chartBlockSettingsCollection) {
        var _this = this;
        var groupedChartBlock = this.chartBlockService.createGroupedChartBlock(chartBlockSettingsCollection);
        chartBlockSettingsCollection.charBlocks.forEach(function (chartBlockSettings) {
            _this.chartBlockService.addChartBlock(groupedChartBlock, chartBlockSettings);
        });
        this.chartBlockService.addGroupedChartBlock(groupedChartBlock);
        var result = {
            setEntityData: function (data) {
                _this.stackWorker.add("allBlocksReady", function () {
                    groupedChartBlock.data = data;
                    _this.chartBlockService.createOrUpdate(groupedChartBlock);
                });
            }
        };
        return result;
    };
    ChartMiddleware.prototype.addDropDownBlock = function (dropDownSettings) {
        var _this = this;
        var dropDownBlock = this.dropDownService.createBlock(dropDownSettings);
        this.dropDownService.addBlock(dropDownBlock);
        var result = {
            id: dropDownBlock.id,
            setData: function (data, isRunFilters) {
                _this.stackWorker.add("allBlocksReady", function () {
                    dropDownBlock.data = data;
                    _this.dropDownService.runBuild(dropDownBlock.id);
                });
            }
        };
        return result;
    };
    ChartMiddleware.prototype.execute = function () {
        this.chartBlockService.execute();
    };
    ChartMiddleware.prototype.setViewType = function (viewSettings) {
        this.chartBlockService.setViewSettings(viewSettings);
    };
    ChartMiddleware.prototype.internalInit = function () {
        var _this = this;
        this.dropDownService.init();
        this.chartBlockService.init();
        this.pipeline.initChartMiddlewarePipelineItem(this.allBlocksReady);
        if (this.settings.sendButton) {
            this.settings.sendButton = DomHelper.getElement(this.settings.sendButton);
            this.settings.sendButton.addEventListener("click", function () {
                var startDate = _this.dateProvider.getStartDateWithTime();
                var endDate = _this.dateProvider.getEndDateWithTime();
                if (_this.settings.sendButtonClickCallback) {
                    _this.settings.sendButtonClickCallback(startDate, endDate);
                }
            });
        }
    };
    ChartMiddleware.prototype.createChartMiddlewareObject = function (chartBlock, groupedChartBlock) {
        var _this = this;
        var result = {
            updateChart: function (dropDownId, filterKey) {
                _this.stackWorker.add("allBlocksReady", function () {
                    var filters = _this.getFilters(dropDownId, filterKey);
                    _this.chartBlockService.createOrUpdate(groupedChartBlock, filters);
                });
            },
            setEntityData: function (data) {
                _this.stackWorker.add("allBlocksReady", function () {
                    groupedChartBlock.data = data;
                    _this.chartBlockService.createOrUpdate(groupedChartBlock);
                });
            },
            getData: function () {
                return chartBlock.chartData;
            },
            download: function () {
                _this.fileBlockService.buildAndDownloadFile(chartBlock);
            }
        };
        return result;
    };
    ChartMiddleware.prototype.getFilters = function (dropDownId, filterKey) {
        var result = [];
        if (_.isEmpty(dropDownId)) {
            return result;
        }
        var filterModel = {
            dropDownId: dropDownId,
            dropDownKey: filterKey
        };
        result.push(filterModel);
        return result;
    };
    ChartMiddleware.prototype.validateChartProvider = function () {
        if (this.chartProvider) {
            return;
        }
        throw new Error("provider is null");
    };
    ChartMiddleware.prototype.setStartDateElement = function (dateElement) {
        this.dateProvider.setStartDateElement(dateElement);
    };
    ChartMiddleware.prototype.setEndDateElement = function (dateElement) {
        this.dateProvider.setEndDateElement(dateElement);
    };
    ChartMiddleware.prototype.getStartDateWithTime = function () {
        return this.dateProvider.getStartDateWithTime();
    };
    ChartMiddleware.prototype.getEndDateWithTime = function () {
        return this.dateProvider.getEndDateWithTime();
    };
    ChartMiddleware.prototype.setMinStartDate = function (date) {
        this.dateProvider.setMinStartDate(date);
    };
    ChartMiddleware.prototype.setMaxStartDate = function (date) {
        this.dateProvider.setMaxStartDate(date);
    };
    ChartMiddleware.prototype.setMinEndDate = function (date) {
        this.dateProvider.setMinEndDate(date);
    };
    ChartMiddleware.prototype.setMaxEndDate = function (date) {
        this.dateProvider.setMaxEndDate(date);
    };
    return ChartMiddleware;
}());
var EntityDataType;
(function (EntityDataType) {
    EntityDataType[EntityDataType["WebRequest"] = 0] = "WebRequest";
    EntityDataType[EntityDataType["Inline"] = 1] = "Inline";
    EntityDataType[EntityDataType["Manual"] = 2] = "Manual";
})(EntityDataType || (EntityDataType = {}));
var DataProvider = (function () {
    function DataProvider() {
        this.processErrorResponse = function (errorResponse) {
            console.log(errorResponse);
        };
    }
    DataProvider.prototype.get = function (url, success, error, isParseResponse) {
        var request = new XMLHttpRequest();
        request.open("GET", url);
        this.sendRequest(request, undefined, success, error, isParseResponse);
    };
    DataProvider.prototype.post = function (url, data, success, error, isParseResponse) {
        var request = new XMLHttpRequest();
        request.open("POST", url);
        request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        this.sendRequest(request, data, success, error, isParseResponse);
    };
    DataProvider.prototype.sendRequest = function (request, data, success, error, isParseResponse) {
        if (_.isUndefined(isParseResponse)) {
            isParseResponse = true;
        }
        request.addEventListener("load", function () {
            var response = isParseResponse ? JSON.parse(request.response) : request.response;
            if (request.status >= 200 && request.status < 400) {
                (success || (function () { }))(response, request);
            }
            else {
                (error || (function () { }))(response);
            }
        });
        request.addEventListener("error", function () {
            (error || (function () { }))(request.response);
        });
        request.send(JSON.stringify(data));
    };
    return DataProvider;
}());
var EntityDataProvider = (function () {
    function EntityDataProvider(dataProvider) {
        this.dataProvider = dataProvider;
    }
    EntityDataProvider.prototype.execute = function (entityData, startDate, endDate, webRequestCallback, inlineCallback, manualCallback) {
        switch (entityData.type) {
            case EntityDataType.WebRequest:
                this.executeWebRequest(entityData, startDate, endDate, webRequestCallback);
                break;
            case EntityDataType.Inline:
                inlineCallback(entityData.data);
                break;
            case EntityDataType.Manual:
                var emptyData = [];
                manualCallback(emptyData);
                break;
            default:
                throw new RangeError("entity data type should be 'WebRequest(0)' or 'Inline(1)', or 'Manual(2)'");
        }
    };
    EntityDataProvider.prototype.executeWebRequest = function (entityData, startDate, endDate, fetchDataCallback) {
        this.prepareEntityDataForWebRequest(entityData, startDate, endDate);
        this.validateEntityDataForWebRequest(entityData);
        switch (entityData.httpMethod) {
            case RequestHttpMethod.Get:
                this.dataProvider.get(entityData.url, function (response) {
                    fetchDataCallback(response);
                }, this.dataProvider.processErrorResponse);
                break;
            case RequestHttpMethod.Post:
                this.dataProvider.post(entityData.url, entityData.requestBody, function (response) {
                    fetchDataCallback(response);
                }, this.dataProvider.processErrorResponse);
                break;
            default:
                throw new RangeError("http method should be 'GET = 0' or 'POST = 1'");
        }
    };
    EntityDataProvider.prototype.prepareEntityDataForWebRequest = function (entityData, startDate, endDate) {
        if (!entityData.url && entityData.getRequestSettingsCallback) {
            var requestSettings = entityData.getRequestSettingsCallback(startDate, endDate);
            entityData.url = requestSettings.url;
            if (!entityData.requestBody) {
                entityData.requestBody = requestSettings.requestBody;
            }
        }
    };
    EntityDataProvider.prototype.validateEntityDataForWebRequest = function (entityData) {
        if (!entityData.url) {
            throw new Error("The url must be (for web request)");
        }
    };
    return EntityDataProvider;
}());
var RequestHttpMethod;
(function (RequestHttpMethod) {
    RequestHttpMethod[RequestHttpMethod["Get"] = 0] = "Get";
    RequestHttpMethod[RequestHttpMethod["Post"] = 1] = "Post";
})(RequestHttpMethod || (RequestHttpMethod = {}));
var ChartMiddlewarePipeline = (function (_super) {
    __extends(ChartMiddlewarePipeline, _super);
    function ChartMiddlewarePipeline() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    ChartMiddlewarePipeline.prototype.getNextPipelineItem = function (currentStep) {
        switch (currentStep) {
            case PipelineEnum.ChartBlock:
                return PipelineEnum.DropDownBlock;
            case PipelineEnum.DropDownBlock:
                return PipelineEnum.ChartMiddleware;
            case PipelineEnum.ChartMiddleware:
                return PipelineEnum.Last;
            default:
                throw new RangeError("unrecognized pipeline type");
        }
    };
    ChartMiddlewarePipeline.prototype.initChartBlockPipelineItem = function (getTotalCount) {
        this.add(this.getChartBlockPipelineItem(getTotalCount));
    };
    ChartMiddlewarePipeline.prototype.getChartBlockPipelineItem = function (getTotalCount) {
        var chartBlocksCounterRule = new PipelineCounterRule(getTotalCount);
        var result = {
            name: "allChartBlocksReady",
            type: PipelineEnum.ChartBlock,
            rules: [chartBlocksCounterRule]
        };
        return result;
    };
    ChartMiddlewarePipeline.prototype.initDropDownBlockPipelineItem = function (getTotalCountFunction, runFunction) {
        this.add(this.getDropDownBlockPipelineItem(getTotalCountFunction, runFunction));
    };
    ChartMiddlewarePipeline.prototype.getDropDownBlockPipelineItem = function (getTotalCountFunction, runFunction) {
        var dropDownBlockCounterRule = new PipelineCounterRule(getTotalCountFunction);
        var result = {
            name: "allDropDownBlocksReady",
            type: PipelineEnum.DropDownBlock,
            rules: [dropDownBlockCounterRule],
            run: runFunction
        };
        return result;
    };
    ChartMiddlewarePipeline.prototype.initChartMiddlewarePipelineItem = function (runFunction) {
        this.add(this.getChartMiddlewarePipelineItem(runFunction));
    };
    ChartMiddlewarePipeline.prototype.getChartMiddlewarePipelineItem = function (runFunction) {
        var result = {
            name: "allBlocksReady",
            type: PipelineEnum.ChartMiddleware,
            run: runFunction
        };
        return result;
    };
    return ChartMiddlewarePipeline;
}(BasePipeline));
var PipelineEnum;
(function (PipelineEnum) {
    PipelineEnum[PipelineEnum["ChartBlock"] = 0] = "ChartBlock";
    PipelineEnum[PipelineEnum["DropDownBlock"] = 1] = "DropDownBlock";
    PipelineEnum[PipelineEnum["ChartMiddleware"] = 2] = "ChartMiddleware";
    PipelineEnum[PipelineEnum["Last"] = Number.MAX_VALUE] = "Last";
})(PipelineEnum || (PipelineEnum = {}));
var FileBlockService = (function () {
    function FileBlockService(downloadFileProvider) {
        this.downloadFileProvider = downloadFileProvider;
    }
    FileBlockService.prototype.buildAndDownloadFile = function (chartBlock) {
        switch (chartBlock.settings.fileSettings.buildType) {
            case FileBuildType.Server:
                this.downloadFileProvider.downloadFile(chartBlock, false);
                break;
            case FileBuildType.ServerModel:
                this.downloadFileProvider.downloadFile(chartBlock);
                break;
            case FileBuildType.Client:
                throw new Error("Not Implemenet Exception");
            case FileBuildType.ClientWithDataFromServer:
                throw new Error("Not Implemenet Exception");
            default:
                throw new RangeError("FileBuildType should be 'Server(0)' or 'ServerModel(1)' or 'Client(2)', or 'ClientWithDataFromServer(3)'");
        }
    };
    return FileBlockService;
}());
var DropDownService = (function () {
    function DropDownService(entityDataProvider, stackWorker, pipeline, dateProvider) {
        var _this = this;
        this.entityDataProvider = entityDataProvider;
        this.stackWorker = stackWorker;
        this.pipeline = pipeline;
        this.dateProvider = dateProvider;
        this.dropDownBlocks = [];
        this.execute = function () {
            var startDate = _this.dateProvider.getStartDateWithTime();
            var endDate = _this.dateProvider.getEndDateWithTime();
            if (_this.dropDownBlocks.length) {
                _this.dropDownBlocks.forEach(function (dropDownBlock) {
                    _this.entityDataProvider.execute(dropDownBlock.settings.entityData, startDate, endDate, function (fetchData) { return _this.webRequestCallbackHandler(fetchData, dropDownBlock); }, function (fetchData) { return _this.inlineCallbackHandler(fetchData, dropDownBlock); }, function (fetchData) { return _this.manualCallbackHandler(fetchData, dropDownBlock); });
                });
            }
            else {
                _this.pipeline.tryNext(PipelineEnum.DropDownBlock);
            }
        };
    }
    DropDownService.prototype.init = function () {
        var _this = this;
        this.pipeline.initDropDownBlockPipelineItem(function () {
            return _this.dropDownBlocks.length;
        }, this.execute);
    };
    DropDownService.prototype.createBlock = function (originalSettings) {
        var dropDownBlock = {
            id: _.uniqueId("ddb_"),
            settings: originalSettings
        };
        return dropDownBlock;
    };
    DropDownService.prototype.addBlock = function (dropDownBlock) {
        this.dropDownBlocks.push(dropDownBlock);
    };
    DropDownService.prototype.build = function (dropDownCollectionItems, updateChartBlockEventHandler) {
        var _this = this;
        dropDownCollectionItems.forEach(function (dropDownCollectionItem) {
            _this.stackWorker.add("buildDropDownGroup-" + dropDownCollectionItem.id, function () {
                dropDownCollectionItem.dropDownHolder = DomHelper.getElement(dropDownCollectionItem.dropDownHolder);
                _this.buildChartBlockDropDown(dropDownCollectionItem, updateChartBlockEventHandler);
            });
        });
    };
    DropDownService.prototype.runBuild = function (dropDownBlockId) {
        this.stackWorker.run("buildDropDownGroup-" + dropDownBlockId);
    };
    DropDownService.prototype.webRequestCallbackHandler = function (fetchData, dropDownBlock) {
        dropDownBlock.data = fetchData;
        this.runBuild(dropDownBlock.id);
        this.pipeline.tryNext(PipelineEnum.DropDownBlock);
    };
    DropDownService.prototype.inlineCallbackHandler = function (fetchData, dropDownBlock) {
        dropDownBlock.data = fetchData;
        this.runBuild(dropDownBlock.id);
        this.pipeline.tryNext(PipelineEnum.DropDownBlock);
    };
    DropDownService.prototype.manualCallbackHandler = function (fetchData, dropDownBlock) {
        this.pipeline.tryNext(PipelineEnum.DropDownBlock);
    };
    DropDownService.prototype.find = function (id) {
        var result = _.find(this.dropDownBlocks, function (dropDownBlock) {
            return dropDownBlock.id === id;
        });
        return result;
    };
    DropDownService.prototype.buildChartBlockDropDown = function (dropDownCollectionItem, updateCallback) {
        var dropDownBlock = this.find(dropDownCollectionItem.id);
        if (!dropDownCollectionItem.element) {
            dropDownCollectionItem.element = this.createDropDownElement(dropDownBlock);
        }
        dropDownCollectionItem.element.addEventListener("change", function () {
            updateCallback();
        });
        dropDownCollectionItem.dropDownHolder.appendChild(dropDownCollectionItem.element);
        if (dropDownCollectionItem.afterCreation) {
            dropDownCollectionItem.afterCreation(dropDownCollectionItem.element);
        }
    };
    DropDownService.prototype.createDropDownElement = function (dropDownBlock) {
        var selectElement = document.createElement("select");
        selectElement.multiple = dropDownBlock.settings.isMultiple;
        dropDownBlock.data.forEach(function (dropDownItem) {
            var optionElement = document.createElement("option");
            optionElement.value = dropDownItem.value;
            optionElement.text = dropDownItem.text;
            selectElement.appendChild(optionElement);
        });
        return selectElement;
    };
    return DropDownService;
}());
var ChartBlockService = (function () {
    function ChartBlockService(chartProvider, entityDataProvider, dropDownService, pipeline, dateProvider, settings) {
        this.chartProvider = chartProvider;
        this.entityDataProvider = entityDataProvider;
        this.dropDownService = dropDownService;
        this.pipeline = pipeline;
        this.dateProvider = dateProvider;
        this.settings = settings;
        this.groupedChartBlocks = [];
    }
    ChartBlockService.prototype.init = function () {
        var _this = this;
        this.pipeline.initChartBlockPipelineItem(function () {
            return _this.groupedChartBlocks.length;
        });
    };
    ChartBlockService.prototype.createGroupedChartBlock = function (entitySettings) {
        var groupedChartBlock = {
            id: this.groupedChartBlocks.length,
            entityData: entitySettings.entityData,
            charts: []
        };
        return groupedChartBlock;
    };
    ChartBlockService.prototype.addGroupedChartBlock = function (chartBlock) {
        this.groupedChartBlocks.push(chartBlock);
    };
    ChartBlockService.prototype.addChartBlock = function (groupedChartBlock, originalSettings) {
        var chartBlock = {
            settings: originalSettings
        };
        chartBlock.settings.rgbaColors = this.getSettingsValue(chartBlock.settings, "rgbaColors", undefined);
        chartBlock.settings.viewSettings = this.getSettingsValue(chartBlock.settings, "viewSettings", { viewType: ViewType.Percentage });
        chartBlock.settings.emptyDataMessage = this.getSettingsValue(chartBlock.settings, "emptyDataMessage", undefined);
        chartBlock.settings.messageTagName = this.getSettingsValue(chartBlock.settings, "messageTagName", "span");
        chartBlock.settings.messageClassName = this.getSettingsValue(chartBlock.settings, "messageClassName", undefined);
        chartBlock.settings.isShowEmptyChart = this.getSettingsValue(chartBlock.settings, "isShowEmptyChart", false);
        chartBlock.settings.chartClassName = this.getSettingsValue(chartBlock.settings, "chartClassName", undefined);
        chartBlock.settings.chartTotalExpression = this.getSettingsValue(chartBlock.settings, "chartTotalExpression", undefined);
        groupedChartBlock.charts.push(chartBlock);
        return chartBlock;
    };
    ChartBlockService.prototype.setViewSettings = function (viewSettings) {
        var _this = this;
        this.groupedChartBlocks.forEach(function (groupedChartBlock) {
            groupedChartBlock.charts.forEach(function (chartBlock) {
                chartBlock.settings.viewSettings = viewSettings;
                if (chartBlock.chart) {
                    _this.createOrUpdate(groupedChartBlock);
                }
            });
        });
    };
    ChartBlockService.prototype.getSettingsValue = function (originChartBlockSettings, propertyName, defaultValue) {
        var chartBlockProperty = originChartBlockSettings[propertyName];
        if (!_.isUndefined(chartBlockProperty)) {
            return chartBlockProperty;
        }
        var commonProperty = this.settings[propertyName];
        if (!_.isUndefined(commonProperty)) {
            return commonProperty;
        }
        return defaultValue;
    };
    ChartBlockService.prototype.execute = function () {
        var _this = this;
        _.once(function () { return _this.prepareChartBlocks(); })();
        _.once(function () { return _this.validate(); })();
        var startDate = this.dateProvider.getStartDateWithTime();
        var endDate = this.dateProvider.getEndDateWithTime();
        this.groupedChartBlocks.forEach(function (groupedChartBlock) {
            _this.entityDataProvider.execute(groupedChartBlock.entityData, startDate, endDate, function (fetchData) { return _this.webRequestCallbackHandler(fetchData, groupedChartBlock); }, function (fetchData) { return _this.inlineCallbackHandler(fetchData, groupedChartBlock); }, function (fetchData) { return _this.manualCallbackHandler(fetchData, groupedChartBlock); });
        });
    };
    ChartBlockService.prototype.webRequestCallbackHandler = function (fetchData, groupedChartBlock) {
        groupedChartBlock.data = fetchData;
        this.createOrUpdate(groupedChartBlock);
        this.pipeline.tryNext(PipelineEnum.ChartBlock);
    };
    ChartBlockService.prototype.inlineCallbackHandler = function (fetchData, groupedChartBlock) {
        groupedChartBlock.data = fetchData;
        this.createOrUpdate(groupedChartBlock);
        this.pipeline.tryNext(PipelineEnum.ChartBlock);
    };
    ChartBlockService.prototype.manualCallbackHandler = function (fetchData, groupedChartBlock) {
        groupedChartBlock.data = fetchData;
        this.pipeline.tryNext(PipelineEnum.ChartBlock);
    };
    ChartBlockService.prototype.prepareChartBlocks = function () {
        this.groupedChartBlocks.forEach(function (groupedChartBlock) {
            groupedChartBlock.charts.forEach(function (chartBlock) {
                chartBlock.settings.chartHolder = DomHelper.getElement(chartBlock.settings.chartHolder);
                if (chartBlock.settings.chartTotalHolder) {
                    chartBlock.settings.chartTotalHolder = DomHelper.getElement(chartBlock.settings.chartTotalHolder);
                }
            });
        });
    };
    ChartBlockService.prototype.createOrUpdate = function (groupedChartBlock, filters) {
        var _this = this;
        groupedChartBlock.charts.forEach(function (chartBlock) {
            var filteredData = _this.filterData(chartBlock.settings.dropDownCollection, groupedChartBlock.data, filters);
            if (chartBlock.chart) {
                _this.updateChart(chartBlock, filteredData);
            }
            else {
                chartBlock.chart = _this.createChart(chartBlock, filteredData);
                _this.buildDropDownBlocks(groupedChartBlock, chartBlock);
            }
        });
    };
    ChartBlockService.prototype.buildDropDownBlocks = function (groupedChartBlock, chartBlock) {
        var _this = this;
        if (chartBlock.settings.dropDownCollection && chartBlock.settings.dropDownCollection.length) {
            this.dropDownService.build(chartBlock.settings.dropDownCollection, function () {
                var filteredData = _this.filterData(chartBlock.settings.dropDownCollection, groupedChartBlock.data);
                _this.updateChart(chartBlock, filteredData);
            });
        }
    };
    ChartBlockService.prototype.filterData = function (dropDownCollectionItems, data, filters) {
        var _this = this;
        var filteredData = data;
        if (!dropDownCollectionItems) {
            return filteredData;
        }
        dropDownCollectionItems.forEach(function (dropDownCollectionItem) {
            var filterKey = _this.getFilterKey(dropDownCollectionItem, filters);
            var isRunFilterExpression = Array.isArray(filterKey) ?
                filterKey.some(function (filterKeyItem) { return !_.isEmpty(filterKeyItem); }) :
                !_.isEmpty(filterKey);
            if (isRunFilterExpression) {
                filteredData = dropDownCollectionItem.entityFilterExpression(filterKey, filteredData);
            }
        });
        return filteredData;
    };
    ChartBlockService.prototype.getFilterKey = function (dropDownCollectionItem, filters) {
        if (dropDownCollectionItem.filterKeyExpression) {
            return dropDownCollectionItem.filterKeyExpression(dropDownCollectionItem.element);
        }
        var filterModel = _.find(filters, function (filter) {
            return filter.dropDownId === dropDownCollectionItem.id;
        });
        if (filterModel) {
            return filterModel.dropDownKey;
        }
        if (!dropDownCollectionItem.element) {
            return null;
        }
        if (dropDownCollectionItem.element.multiple) {
            var selectedValues = _.map(dropDownCollectionItem.element.selectedOptions, function (selectedOption) {
                return selectedOption.value;
            });
            return selectedValues;
        }
        var result = dropDownCollectionItem.element.value;
        return result;
    };
    ChartBlockService.prototype.createChart = function (chartBlock, data) {
        var totalCount = this.getChartTotalDataItems(data);
        chartBlock.chartData = this.getChartGroupedData(chartBlock, data);
        chartBlock.sortedData = this.getChartSortedData(chartBlock, chartBlock.chartData);
        return this.chartProvider.createChart(chartBlock.settings, chartBlock.chartData, totalCount, chartBlock.sortedData);
    };
    ChartBlockService.prototype.updateChart = function (chartBlock, data) {
        var totalCount = this.getChartTotalDataItems(data);
        chartBlock.chartData = this.getChartGroupedData(chartBlock, data);
        chartBlock.sortedData = this.getChartSortedData(chartBlock, chartBlock.chartData);
        this.chartProvider.updateChart(chartBlock.settings, chartBlock.chart, chartBlock.chartData, totalCount, chartBlock.sortedData);
    };
    ChartBlockService.prototype.getChartTotalDataItems = function (data) {
        return data.length;
    };
    ChartBlockService.prototype.getChartGroupedData = function (chartBlock, data) {
        return chartBlock.settings.entityGroupingExpression(data);
    };
    ChartBlockService.prototype.getChartSortedData = function (chartBlock, data) {
        if (chartBlock.settings.entitySortingExpression) {
            var dataInArray = this.getEntityItems(data);
            return chartBlock.settings.entitySortingExpression(dataInArray);
        }
        else {
            return null;
        }
    };
    ChartBlockService.prototype.validate = function () {
        if (this.groupedChartBlocks.length === 0) {
            throw new Error("There is no any grouped chart block");
        }
        this.groupedChartBlocks.forEach(function (groupedChartBlock) {
            groupedChartBlock.charts.forEach(function (chartBlock) {
                if (!chartBlock.settings.chartHolder) {
                    throw new Error("Chart holder (holder for canvas) must be");
                }
            });
        });
    };
    ChartBlockService.prototype.getEntityItems = function (data) {
        var result = Object.keys(data).map(function (itemIndex) {
            var item = {
                key: itemIndex,
                value: data[itemIndex]
            };
            return item;
        });
        return result;
    };
    return ChartBlockService;
}());
//# sourceMappingURL=chart-middleware.js.map