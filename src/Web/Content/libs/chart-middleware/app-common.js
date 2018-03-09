var ViewType;
(function (ViewType) {
    ViewType[ViewType["Percentage"] = 0] = "Percentage";
    ViewType[ViewType["Quantitative"] = 1] = "Quantitative";
    ViewType[ViewType["Custom"] = 2] = "Custom";
})(ViewType || (ViewType = {}));
var BaseDownloadFileProvider = (function () {
    function BaseDownloadFileProvider(dateProvider) {
        this.dateProvider = dateProvider;
    }
    BaseDownloadFileProvider.prototype.getFileChartModel = function (chartBlock) {
        var result;
        var startDate = this.dateProvider.getStartDateWithTime();
        var endDate = this.dateProvider.getEndDateWithTime();
        var clonedChartData = _.clone(chartBlock.chartData);
        if (chartBlock.settings.fileSettings.entityDataExpression) {
            result = chartBlock.settings.fileSettings.entityDataExpression(clonedChartData, startDate, endDate);
        }
        else {
            result = {
                startDate: startDate,
                endDate: endDate,
                items: this.getFileChartBlockItems(chartBlock, clonedChartData)
            };
        }
        return result;
    };
    BaseDownloadFileProvider.prototype.getFileChartBlockItems = function (chartBlock, clonedChartData) {
        _.each(clonedChartData, function (chartBlockItemData, key, collection) {
            if (chartBlock.settings.fileSettings.entityDataItemExpression) {
                collection[key] = chartBlock.settings.fileSettings.entityDataItemExpression(chartBlockItemData);
            }
        });
        return clonedChartData;
    };
    return BaseDownloadFileProvider;
}());
var FileBuildType;
(function (FileBuildType) {
    FileBuildType[FileBuildType["Server"] = 0] = "Server";
    FileBuildType[FileBuildType["ServerModel"] = 1] = "ServerModel";
    FileBuildType[FileBuildType["Client"] = 2] = "Client";
    FileBuildType[FileBuildType["ClientWithDataFromServer"] = 3] = "ClientWithDataFromServer";
})(FileBuildType || (FileBuildType = {}));
var DomHelper = (function () {
    function DomHelper() {
    }
    DomHelper.getElement = function (element) {
        var resultElement;
        if (element instanceof HTMLElement) {
            resultElement = element;
        }
        else if (typeof element === "string") {
            resultElement = document.querySelector(element);
        }
        else if (element instanceof Function) {
            resultElement = element();
        }
        else {
            throw new Error("unknown element type, should be: selector, element or function that return element");
        }
        this.validateResultElement(resultElement);
        return resultElement;
    };
    DomHelper.validateResultElement = function (resultElement) {
        if (!resultElement) {
            throw new Error("Element (holder) is missing, check selector, element or callback (callback must return element)");
        }
    };
    DomHelper.show = function (element) {
        element.style.display = "block";
    };
    DomHelper.hide = function (element) {
        element.style.display = "none";
    };
    return DomHelper;
}());
var DateHelper = (function () {
    function DateHelper(originalSettings) {
        this.originalSettings = originalSettings;
        this.settings = {
            isSetDefaultStartDate: true,
            isSetDefaultEndDate: true,
            isUseLastMonthByDefault: true
        };
        this.settings = _.extendOwn(this.settings, this.originalSettings);
    }
    DateHelper.prototype.getStartDateElement = function () {
        if (!this.settings.startDateElement) {
            return null;
        }
        if (!this.startDateElement) {
            this.startDateElement = DomHelper.getElement(this.settings.startDateElement);
        }
        return this.startDateElement;
    };
    DateHelper.prototype.getEndDateElement = function () {
        if (!this.settings.endDateElement) {
            return null;
        }
        if (!this.endDateElement) {
            this.endDateElement = DomHelper.getElement(this.settings.endDateElement);
        }
        return this.endDateElement;
    };
    DateHelper.prototype.setStartDateElement = function (dateElement) {
        this.settings.startDateElement = dateElement;
        this.startDateElement = DomHelper.getElement(this.settings.startDateElement);
    };
    DateHelper.prototype.setEndDateElement = function (dateElement) {
        this.settings.endDateElement = dateElement;
        this.endDateElement = DomHelper.getElement(this.settings.endDateElement);
    };
    DateHelper.prototype.getDefaultStartDate = function () {
        var defaultDate = this.getDefaultDate(this.settings.isSetDefaultStartDate, this.settings.defaultStartDate, this.getPrevMonthFirstDate);
        defaultDate = this.getMinAvailableDate(defaultDate, this.settings.minStartDate);
        defaultDate = this.getMaxAvailableDate(defaultDate, this.settings.maxStartDate);
        return defaultDate;
    };
    DateHelper.prototype.getDefaultEndDate = function () {
        var defaultDate = this.getDefaultDate(this.settings.isSetDefaultEndDate, this.settings.defaultEndDate, this.getPrevMonthLastDate);
        defaultDate = this.getMinAvailableDate(defaultDate, this.settings.minEndDate);
        defaultDate = this.getMaxAvailableDate(defaultDate, this.settings.maxEndDate);
        return defaultDate;
    };
    DateHelper.prototype.getDefaultDate = function (isSetDefaultDate, defaultDate, getDateFunction) {
        if (!isSetDefaultDate) {
            return null;
        }
        if (defaultDate) {
            return defaultDate;
        }
        if (!this.settings.isUseLastMonthByDefault) {
            return new Date();
        }
        return getDateFunction();
    };
    DateHelper.prototype.getPrevMonthFirstDate = function () {
        var currentDate = new Date();
        var result = new Date(currentDate.getFullYear(), currentDate.getMonth() - 1, 1);
        return result;
    };
    DateHelper.prototype.getPrevMonthLastDate = function () {
        var currentDate = new Date();
        var result = new Date(currentDate.getFullYear(), currentDate.getMonth(), 0);
        return result;
    };
    DateHelper.prototype.getMinAvailableDate = function (defaultDate, minDate) {
        var result = (minDate && minDate > defaultDate) ? minDate : defaultDate;
        return result;
    };
    DateHelper.prototype.getMaxAvailableDate = function (defaultDate, maxDate) {
        var result = (maxDate && maxDate < defaultDate) ? maxDate : defaultDate;
        return result;
    };
    return DateHelper;
}());
var ChartTypeEnum;
(function (ChartTypeEnum) {
    ChartTypeEnum[ChartTypeEnum["Pie"] = 0] = "Pie";
    ChartTypeEnum[ChartTypeEnum["Doughnut"] = 1] = "Doughnut";
    ChartTypeEnum[ChartTypeEnum["Bar"] = 2] = "Bar";
    ChartTypeEnum[ChartTypeEnum["HorizontalBar"] = 3] = "HorizontalBar";
    ChartTypeEnum[ChartTypeEnum["Line"] = 4] = "Line";
    ChartTypeEnum[ChartTypeEnum["Scatter"] = 5] = "Scatter";
    ChartTypeEnum[ChartTypeEnum["PolarArea"] = 6] = "PolarArea";
    ChartTypeEnum[ChartTypeEnum["Bubble"] = 7] = "Bubble";
})(ChartTypeEnum || (ChartTypeEnum = {}));
var ColorHelper = (function () {
    function ColorHelper() {
        this.colors = [];
    }
    ColorHelper.prototype.setColors = function (colors) {
        this.colors = colors;
    };
    ColorHelper.prototype.getColor = function (chartItemIndex) {
        if (!this.colors) {
            return undefined;
        }
        var colorIndex = chartItemIndex % this.colors.length;
        if (_.isNaN(colorIndex)) {
            return undefined;
        }
        var result = this.colors[colorIndex];
        return result;
    };
    return ColorHelper;
}());
var EntitySortDirection;
(function (EntitySortDirection) {
    EntitySortDirection[EntitySortDirection["ASC"] = 0] = "ASC";
    EntitySortDirection[EntitySortDirection["DESC"] = 1] = "DESC";
})(EntitySortDirection || (EntitySortDirection = {}));
//# sourceMappingURL=app-common.js.map