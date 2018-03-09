var ChartFactory = (function () {
    function ChartFactory() {
    }
    ChartFactory.getChartProvider = function () {
        return new ChartJsProvider();
    };
    return ChartFactory;
}());
var chartFactoryTypeChecker = ChartFactory;
var ChartJsProvider = (function () {
    function ChartJsProvider() {
        this.colorHelper = new ColorHelper();
    }
    ChartJsProvider.prototype.updateChart = function (settings, chart, groupedData, totalCount, sortedData) {
        this.clearChartData(chart.data);
        var chartTotalValue = this.addChartData(settings, chart.data, groupedData, totalCount, sortedData);
        this.buildChartTotalLabel(chartTotalValue, settings);
        chart.update();
        this.processMessage(settings, groupedData);
    };
    ChartJsProvider.prototype.createChart = function (settings, groupedData, totalCount, sortedData) {
        var _this = this;
        settings.canvasElement = this.getCanvasElementByHolder(settings);
        var canvasContext = settings.canvasElement.getContext("2d");
        var chartSettings = this.getChartSettings(settings);
        var chartTotalValue = this.addChartData(settings, chartSettings.data, groupedData, totalCount, sortedData);
        this.buildChartTotalLabel(chartTotalValue, settings);
        var chart = new Chart(canvasContext, chartSettings);
        this.prepareMessageElement(settings, function () {
            _this.processMessage(settings, groupedData);
        });
        return chart;
    };
    ChartJsProvider.prototype.addChartData = function (settings, chartData, groupedData, totalCount, sortedData) {
        var _this = this;
        var chartItemIndex = 0;
        var chartItemsCount = _.size(groupedData);
        this.colorHelper.setColors(settings.rgbaColors);
        var totalItemsValue = 0;
        var data = this.sortData(groupedData, sortedData);
        _.each(data, function (groupedItem) {
            var dataItems = groupedItem.value;
            var key = groupedItem.key;
            var chartItemValue = _this.getChartItemValue(dataItems, totalCount, settings.viewSettings);
            totalItemsValue += chartItemValue;
            var chartItemLabel = _this.getChartItemLabel(settings, dataItems, key, chartItemValue);
            chartData.labels.push(chartItemLabel);
            var dataset = chartData.datasets[0];
            dataset.data.push(chartItemValue);
            var color = _this.getChartItemColor(settings, chartItemIndex, chartItemsCount);
            if (color) {
                dataset.backgroundColor.push(color);
            }
            chartItemIndex++;
        });
        return totalItemsValue;
    };
    ChartJsProvider.prototype.clearChartData = function (chartData) {
        chartData.labels = [];
        chartData.datasets.forEach(function (dataset) {
            dataset.data = [];
            if (true) {
                dataset.backgroundColor = [];
            }
        });
    };
    ChartJsProvider.prototype.getChartItemValue = function (dataItems, totalCount, viewSettings) {
        if (viewSettings.calcValueExpression) {
            return viewSettings.calcValueExpression(dataItems, totalCount, viewSettings.viewType, viewSettings.viewName);
        }
        var result;
        switch (viewSettings.viewType) {
            case ViewType.Percentage:
                result = this.calcPercentages(dataItems.length, totalCount);
                break;
            case ViewType.Quantitative:
                result = dataItems.length;
                break;
            case ViewType.Custom:
                throw new Error("If view type is Custom calcValueExpression must be");
            default:
                throw new RangeError("View type should be Percentage (0) or Quantitative(1) or Custom(2)");
        }
        return result;
    };
    ChartJsProvider.prototype.getChartItemLabel = function (settings, dataItems, key, chartItemValue) {
        if (settings.chartItemLabelExpression) {
            return settings.chartItemLabelExpression(dataItems, key, settings.viewSettings.viewType, settings.viewSettings.viewName);
        }
        var result;
        switch (settings.viewSettings.viewType) {
            case ViewType.Percentage:
                result = key + " " + chartItemValue + "%";
                break;
            case ViewType.Quantitative:
                result = key + " " + chartItemValue;
                break;
            case ViewType.Custom:
                result = key + " " + settings.viewSettings.viewName + " " + chartItemValue;
                break;
            default:
                throw new RangeError("View type should be Percentage (0) or Quantitative(1) or Custom(2)");
        }
        return result;
    };
    ChartJsProvider.prototype.buildChartTotalLabel = function (totalValue, settings) {
        if (!settings.chartTotalExpression) {
            return;
        }
        if (totalValue % 10 !== 0) {
            totalValue = parseFloat(totalValue.toFixed(1));
        }
        var chartTotalLabel = settings.chartTotalExpression(totalValue, settings.viewSettings.viewType, settings.viewSettings.viewName);
        if (settings.chartTotalHolder) {
            settings.chartTotalHolder.textContent = chartTotalLabel;
        }
    };
    ChartJsProvider.prototype.calcPercentages = function (dataItemsLength, totalCount) {
        var result = (dataItemsLength * 100 / totalCount) * 10;
        result = Math.round(result) / 10;
        return result;
    };
    ChartJsProvider.prototype.getCanvasElementByHolder = function (settings) {
        var chartHolder = settings.chartHolder;
        var canvasElement = chartHolder.getElementsByTagName("canvas")[0];
        if (!canvasElement) {
            canvasElement = document.createElement("canvas");
            chartHolder.innerHTML = "";
            chartHolder.appendChild(canvasElement);
        }
        if (!_.isUndefined(settings.chartClassName)) {
            canvasElement.className = settings.chartClassName;
        }
        return canvasElement;
    };
    ChartJsProvider.prototype.getChartSettings = function (settings) {
        var defaultSettings = {
            type: this.getChartTypeByEnum(settings.chartType),
            data: {
                datasets: [{
                        data: [],
                        backgroundColor: []
                    }],
                labels: []
            }
        };
        var chartSettings = _.extendOwn(defaultSettings, settings.chartSettings);
        return chartSettings;
    };
    ChartJsProvider.prototype.getChartTypeByEnum = function (chartTypeEnum) {
        switch (chartTypeEnum) {
            case ChartTypeEnum.Pie:
                return "pie";
            case ChartTypeEnum.Doughnut:
                return "doughnut";
            case ChartTypeEnum.Bar:
                return "bar";
            case ChartTypeEnum.HorizontalBar:
                return "horizontalBar";
            case ChartTypeEnum.Line:
            case ChartTypeEnum.Scatter:
                return "line";
            case ChartTypeEnum.PolarArea:
                return "polarArea";
            case ChartTypeEnum.Bubble:
                return "bubble";
            default:
                throw new RangeError("Chart Type is incorrect");
        }
    };
    ChartJsProvider.prototype.getChartItemColor = function (settings, chartItemIndex, chartItemsCount) {
        var color = "";
        if (settings.chartItemColorExpression) {
            color = settings.chartItemColorExpression(chartItemIndex, chartItemsCount);
        }
        else {
            color = this.colorHelper.getColor(chartItemIndex);
        }
        return color;
    };
    ChartJsProvider.prototype.processMessage = function (settings, groupedData) {
        if (_.isUndefined(settings.emptyDataMessage) || settings.isShowEmptyChart) {
            return;
        }
        var isChartItemsCountEmpty = _.size(groupedData) === 0;
        if (isChartItemsCountEmpty || this.isChartItemsDataEmpty(groupedData)) {
            DomHelper.hide(settings.canvasElement);
            settings.messageElement.textContent = settings.emptyDataMessage;
            DomHelper.show(settings.messageElement);
            if (settings.emptyDataCallback) {
                settings.emptyDataCallback();
            }
        }
        else {
            settings.messageElement.textContent = "";
            DomHelper.hide(settings.messageElement);
            DomHelper.show(settings.canvasElement);
            if (settings.notEmptyDataCallback) {
                settings.notEmptyDataCallback();
            }
        }
    };
    ChartJsProvider.prototype.isChartItemsDataEmpty = function (groupedData) {
        var result = _.all(groupedData, function (dataItems) {
            return dataItems.length === 0;
        });
        return result;
    };
    ChartJsProvider.prototype.prepareMessageElement = function (settings, afterCreateMessageElementCallback) {
        settings.messageElement = this.createMessageElement(settings);
        afterCreateMessageElementCallback();
        settings.chartHolder.appendChild(settings.messageElement);
    };
    ChartJsProvider.prototype.createMessageElement = function (settings) {
        var messageElement = document.createElement(settings.messageTagName);
        if (!_.isUndefined(settings.messageClassName)) {
            messageElement.className = settings.messageClassName;
        }
        return messageElement;
    };
    ChartJsProvider.prototype.sortData = function (groupedData, sortedData) {
        if (sortedData && typeof sortedData === "object") {
            return sortedData;
        }
        var result = this.getEntityItems(groupedData);
        if (sortedData === EntitySortDirection.ASC) {
            result = _.sortBy(result, function (item) { return item.key; });
        }
        else if (sortedData === EntitySortDirection.DESC) {
            result = _.sortBy(result, function (item) { return item.key; }).reverse();
        }
        return result;
    };
    ChartJsProvider.prototype.getEntityItems = function (groupedData) {
        var result = Object.keys(groupedData).map(function (itemIndex) {
            var item = {
                key: itemIndex,
                value: groupedData[itemIndex]
            };
            return item;
        });
        return result;
    };
    return ChartJsProvider;
}());
//# sourceMappingURL=app-chartjs-provider.js.map