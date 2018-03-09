(function () {
    var statisticsController = function ($scope, $filter, statisticsProvider, securityService, urlService) {
        var self = this;
        self.isActiveLoadIcon = false;

        checkSecurityPermission();

        self.facilityTasksVsTenantTasks = getTaskRatioChart(false);
        self.unprocessedVsProcessedTasks = getTaskRatioChart(false);
        self.spentTimeVsTenantTasks = getTaskRatioChart(false);
        self.tenantTasksVsVisitsAmount = getTaskRatioChart(false);
        self.spentTimeVsFacilityTasks = getTaskRatioChart(true);
        self.cancelingTenantReason = getTaskRatioChart(false);

        var statisticFilters;

        self.viewType = ViewType;
        self.activeViewType = ViewType.Custom;
        self.viewNameCustomHours = 'hours';
        self.dateFormat = DatePickerFormatType.dayMonthYear;

        var chartMiddleware = new ChartMiddleware({
            viewSettings: {
                viewType: self.activeViewType,
                viewName: self.viewNameCustomHours,
                calcValueExpression: calcValueExpression
            },
            chartTotalExpression: getChartTotalLabelTranslation,
            datePickerSettings: {
                startDateElement: '#start-date-datepicker',
                endDateElement: '#end-date-datepicker'
            },
            messageClassName: 'statistic__empty-date',
            emptyDataMessage: $filter('translate')('No data')
        });

        self.setViewType = function (viewType, name) {
            var viewSettings = {
                viewType: viewType
            };

            self.activeViewType = viewType;

            switch (viewType) {
                case ViewType.Percentage:
                    chartMiddleware.setViewType(viewSettings);
                    break;
                case ViewType.Quantitative:
                    chartMiddleware.setViewType(viewSettings);
                    break;
                case ViewType.Custom:
                    viewSettings.viewName = name;
                    viewSettings.calcValueExpression = calcValueExpression;
                    chartMiddleware.setViewType(viewSettings);
                    break;
                default:
                    throw new Error('NotImplementException');
            }
        }

        self.getStatistics = function () {
            chartMiddleware.execute();

            var startDate = chartMiddleware.getStartDateWithTime();
            var endDate = chartMiddleware.getEndDateWithTime();

            var utcDatePeriod = {
                startDate: startDate,
                endDate: endDate
            };

            getDataForChart(statisticsProvider.getFacilityTasksVsTenantTasksData, utcDatePeriod, self.facilityTasksVsTenantTasks);
            getDataForChart(statisticsProvider.getUnprocessedVsProcessedTasksData, utcDatePeriod, self.unprocessedVsProcessedTasks);
            getDataForChart(statisticsProvider.getSpentTimeVsTenantTasksData, utcDatePeriod, self.spentTimeVsTenantTasks);
            getDataForChart(statisticsProvider.getSpentTimeVsFacilityTasksData, utcDatePeriod, self.spentTimeVsFacilityTasks);
            getDataForChart(statisticsProvider.getTenantTasksVsVisitsAmountData, utcDatePeriod, self.tenantTasksVsVisitsAmount);
            getDataForChart(statisticsProvider.getCancelingReasonData, utcDatePeriod, self.cancelingTenantReason);
        }

        function getDataForChart(getMethod, period, chartBlock) {
            getMethod(period).then(function (response) {
                chartBlock.isDataLoaded = true;
                _.defer(function () {
                    if (response.data.groups) {
                        chartBlock.groups = response.data.groups;
                    }
                    chartBlock.chart.setEntityData(response.data.data);
                });
            });
        }

        self.downloadCsv = function (chartBlock) {
            showLoadIcon();
            chartBlock.chart.download();
        }

        var departmentsDropDown;
        function initDepartmentsDropDown() {
            departmentsDropDown = chartMiddleware.addDropDownBlock({
                entityData: {
                    type: EntityDataType.Manual
                }
            });
        }

        var managementDepartmentsDropDown;
        function initManagementDepartmentsDropDown() {
            managementDepartmentsDropDown = chartMiddleware.addDropDownBlock({
                entityData: {
                    type: EntityDataType.Manual
                }
            });
        }

        var categoriesDropDown;
        function initCategoriesDropDown() {
            categoriesDropDown = chartMiddleware.addDropDownBlock({
                entityData: {
                    type: EntityDataType.Manual
                }
            });
        }

        var cancelingReasonsDropDown;
        function initCancelingReasonsDropDown() {
            cancelingReasonsDropDown = chartMiddleware.addDropDownBlock({
                entityData: {
                    type: EntityDataType.Manual
                }
            });
        }


        initDepartmentsDropDown();
        initManagementDepartmentsDropDown();
        initCategoriesDropDown();
        initCancelingReasonsDropDown();

        var housingDepartments = [];

        statisticsProvider.getStatisticFiltersModel().then(function (response) {
            statisticFilters = response.data;

            var managementDepartments = response.data.managementDepartments;
            self.spentTimeVsFacilityTasks.managementDepartments = managementDepartments;
            self.facilityTasksVsTenantTasks.managementDepartments = managementDepartments;
            self.spentTimeVsTenantTasks.managementDepartments = managementDepartments;
            self.tenantTasksVsVisitsAmount.managementDepartments = managementDepartments;
            self.cancelingTenantReason.managementDepartments = managementDepartments;
            self.unprocessedVsProcessedTasks.managementDepartments = managementDepartments;


            housingDepartments = getHousingDepartments(response.data.managementDepartments);
            self.facilityTasksVsTenantTasks.housingDepartments = housingDepartments;
            self.spentTimeVsTenantTasks.housingDepartments = housingDepartments;
            self.spentTimeVsFacilityTasks.housingDepartments = housingDepartments;
            self.tenantTasksVsVisitsAmount.housingDepartments = housingDepartments;
            self.cancelingTenantReason.housingDepartments = housingDepartments;
            self.unprocessedVsProcessedTasks.housingDepartments = housingDepartments;

            self.unprocessedVsProcessedTasks.cancelingReasonsPickerConfig.dataList = response.data.cancelingReasons;
        });

        function initUnprocessedVsProcessedTasksChart() {
            self.unprocessedVsProcessedTasks.chart = chartMiddleware.addChartBlock({
                entityGroupingExpression: function (data) {
                    var groupedData = {};

                    var groups = _.keys(self.unprocessedVsProcessedTasks.groups);
                    groups.forEach(function (group) {
                        groupedData[group] = data.filter(function (dataItem) {
                            return _.contains(self.unprocessedVsProcessedTasks.groups[group], dataItem.jobStatus);
                        });
                    });

                    return groupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.unprocessedVsProcessedTasks.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.unprocessedVsProcessedTasks.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: cancelingReasonsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return dataItem.cancelingReasons.some(function (reason) {
                                    return reason == filterKey.value;
                                });
                            });

                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.unprocessedVsProcessedTasks.cancelingReasonsPickerConfig.selected;
                        }
                    }
                ],
                chartHolder: '#unprocessedVsProcessedTasks-chart__holder',
                chartTotalHolder: '#unprocessedVsProcessedTasks-chart__total',
                chartType: ChartTypeEnum.Pie,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return $filter('translate')('statistics-page__' + key);
                },
                chartItemColorExpression: getAvailableColors,
                chartSettings: {
                    options: {
                        tooltips: getDefaultChartTooltipSettings()
                    }
                },
                fileSettings: entityDataExpression('/api/statistics/getUnprocessedVsProcessedTasksCsv', function () {
                    hideLoadIcon();
                }),
                emptyDataCallback: function () {
                    setDataEmpty(self.unprocessedVsProcessedTasks);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.unprocessedVsProcessedTasks);
                }
            });
        }

        function getTaskRatioChart(isCategoryPickerRequired) {
            var chartBlock = {
                chart: {},
                isDataLoaded: false,
                isDataEmpty: false,
                groups: [],
                housingDepartments: [],
                simpleDepartmentPickerConfig: {
                    onSelect: function (department) {
                        chartBlock.chart.updateChart();
                    },
                    onRemove: function (department) {
                        chartBlock.chart.updateChart();
                    }
                },
                managementDepartmentPickerConfig: {
                    onSelect: function (department) {

                        if (chartBlock.managementDepartmentPickerConfig.selectedDepartments.length === 1) {
                            chartBlock.housingDepartments = _.clone(department.housingDepartments);
                        } else {
                            department.housingDepartments.forEach(function (housingDepartment) {
                                chartBlock.housingDepartments.push(housingDepartment);
                            });
                        }
                        _.defer(function () {
                            chartBlock.chart.updateChart();
                        });
                    },
                    onRemove: function (department) {
                        if (chartBlock.managementDepartmentPickerConfig.selectedDepartments.length === 0) {
                            chartBlock.housingDepartments = housingDepartments;
                        } else {
                            chartBlock.housingDepartments =
                                _.difference(chartBlock.housingDepartments, department.housingDepartments);
                        }

                        _.defer(function () {
                            chartBlock.chart.updateChart();
                        });
                    }
                },
                cancelingReasonsPickerConfig: {
                    onUpdate: function () {
                        chartBlock.chart.updateChart();
                    },
                    placeholder: $filter('translate')('Choose canceling reason')
                }
            };
            if (isCategoryPickerRequired)
                chartBlock.categoryTree = {
                    config: {
                        dropdownView: true,
                        isIncludeGroupedTasks: false,
                        isIncludeHiddenTasks: false
                    },
                    onSelected: function (category) {
                        self.spentTimeVsFacilityTasks.chart.updateChart();
                        return true;
                    }
                };
            return chartBlock;
        }

        function initFacilityTasksVsTenantTasksChart() {
            self.facilityTasksVsTenantTasks.chart = chartMiddleware.addChartBlock({
                entityGroupingExpression: function (data) {
                    var facilityGroupedData = {};

                    var groups = _.keys(self.facilityTasksVsTenantTasks.groups);
                    groups.forEach(function (group) {
                        facilityGroupedData[group] = data.filter(function (dataItem) {
                            return _.contains(self.facilityTasksVsTenantTasks.groups[group], dataItem.jobType);
                        });
                    });

                    return facilityGroupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.facilityTasksVsTenantTasks.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.facilityTasksVsTenantTasks.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    }
                ],
                chartHolder: '#facilityTasksVsTenantTasks-chart__holder',
                chartTotalHolder: '#facilityTasksVsTenantTasks-chart__total',
                chartType: ChartTypeEnum.Pie,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return $filter('translate')('statistics-page__' + key);
                },
                chartItemColorExpression: getAvailableColors,
                chartSettings: {
                    options: {
                        tooltips: getDefaultChartTooltipSettings()
                    }
                },
                fileSettings: entityDataExpression('/api/statistics/getFacilityTasksVsTenantTasksCsv', function () {
                    hideLoadIcon();
                }),
                emptyDataCallback: function () {
                    setDataEmpty(self.facilityTasksVsTenantTasks);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.facilityTasksVsTenantTasks);
                }
            });
        }

        function initTenantTasksVsVisitsAmount() {
            self.tenantTasksVsVisitsAmount.chart = chartMiddleware.addChartBlock({
                entitySortingExpression: function () {
                    return EntitySortDirection.DESC;
                },
                entityGroupingExpression: function (data) {
                    var groupedData = _.groupBy(data, function (dataItem) {
                        return dataItem.amount;
                    });

                    return groupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.tenantTasksVsVisitsAmount.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.tenantTasksVsVisitsAmount.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    }
                ],
                chartHolder: '#tenantTasksVsVisitsAmount-chart__holder',
                chartTotalHolder: '#tenantTasksVsVisitsAmount-chart__total',
                chartType: ChartTypeEnum.HorizontalBar,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return key;
                },
                chartItemColorExpression: getAvailableColors,
                chartSettings: getDefaultBarChartSettings(),
                fileSettings: entityAddressDataExpression('/api/statistics/getTenantTasksVsVisitsAmountCsv',
                    function () {
                        hideLoadIcon();
                    }),
                emptyDataCallback: function () {
                    setDataEmpty(self.tenantTasksVsVisitsAmount);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.tenantTasksVsVisitsAmount);
                }
            });
        }

        function initSpentTimeVsTenantTasksChart() {
            self.spentTimeVsTenantTasks.chart = chartMiddleware.addChartBlock({
                entityGroupingExpression: function (data) {
                    var groupedData = _.groupBy(data, function (dataItem) {
                        return dataItem.tenantType;
                    });

                    return groupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.spentTimeVsTenantTasks.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.spentTimeVsTenantTasks.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    }],
                chartHolder: '#spentTimeVsTenantTasks-chart__holder',
                chartTotalHolder: '#spentTimeVsTenantTasks-chart__total',
                chartType: ChartTypeEnum.HorizontalBar,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return $filter('translate')('tenantType-' + key);
                },
                chartItemColorExpression: getAvailableColors,
                chartSettings: getDefaultBarChartSettings(),
                fileSettings: entityDataExpression('/api/statistics/getSpentTimeVsTenantTasksCsv', function () {
                    hideLoadIcon();
                }),
                emptyDataCallback: function () {
                    setDataEmpty(self.spentTimeVsTenantTasks);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.spentTimeVsTenantTasks);
                }
            });
        }

        function initSpentTimeVsFacilityTasksChart() {
            self.spentTimeVsFacilityTasks.chart = chartMiddleware.addChartBlock({
                entityGroupingExpression: function (data) {
                    var groupedData = _.groupBy(data,
                        function (dataItem) {
                            return dataItem.categorySortPriority;
                        });
                    return groupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.spentTimeVsFacilityTasks.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.spentTimeVsFacilityTasks.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: categoriesDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.categoryId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.spentTimeVsFacilityTasks.categoryTree.config.getSelectedCategories();
                        }
                    }],
                chartHolder: '#spentTimeVsFacilityTasks-chart__holder',
                chartTotalHolder: '#spentTimeVsFacilityTasks-chart__total',
                chartType: ChartTypeEnum.HorizontalBar,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return statisticFilters.categoriesIdsToNamesRelation[groupedData[0].categoryId];
                },
                chartItemColorExpression: getAvailableColors,
                chartSettings: getDefaultBarChartSettings(),
                fileSettings: entityDataExpression('/api/statistics/getSpentTimeVsFacilityTasksCsv', function () {
                    hideLoadIcon();
                }),
                emptyDataCallback: function () {
                    setDataEmpty(self.spentTimeVsFacilityTasks);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.spentTimeVsFacilityTasks);
                }
            });
        }        

        function initCancelingTenantReasonChart() {
            self.cancelingTenantReason.chart = chartMiddleware.addChartBlock({
                entityGroupingExpression: function (data) {
                    var groupedData = _.groupBy(data, function (dataItem) {
                        return dataItem.reasonId;
                    });

                    return groupedData;
                },
                entityData: {
                    type: EntityDataType.Manual
                },
                dropDownCollection: [
                    {
                        id: managementDepartmentsDropDown.id,
                        entityFilterExpression: function (filterKey, data) {
                            var result = data.filter(function (dataItem) {
                                return filterKey.some(function (filterKeyitem) {
                                    return filterKeyitem.id === dataItem.managementDepartmentId.toString();
                                });
                            });
                            return result;
                        },
                        filterKeyExpression: function () {
                            return self.cancelingTenantReason.managementDepartmentPickerConfig.selectedDepartments;
                        }
                    },
                    {
                        id: departmentsDropDown.id,
                        entityFilterExpression: departmentsDropDownExpression,
                        filterKeyExpression: function () {
                            return self.cancelingTenantReason.simpleDepartmentPickerConfig.selectedDepartments;
                        }
                    }],
                chartHolder: '#cancelingTenantReason-chart__holder',
                chartTotalHolder: '#cancelingTenantReason-chart__total',
                chartType: ChartTypeEnum.HorizontalBar,
                chartItemLabelExpression: function (groupedData, key, viewType, viewName) {
                    return groupedData[0].reason;
                },
                chartItemColorExpression: getAvailableColors,
                fileSettings: cancelingReasonsDataExpressions('/api/statistics/getTenantTaskSeparatedByCanceledReasonDataCsv', function () {
                    hideLoadIcon();
                }),
                chartSettings: getDefaultBarChartSettings(),
                emptyDataCallback: function () {
                    setDataEmpty(self.cancelingTenantReason);
                },
                notEmptyDataCallback: function () {
                    setDataNotEmpty(self.cancelingTenantReason);
                },
                chartTotalExpression: getCancelingTenantReasonChartLabel
            });
        }

        function getCancelingTenantReasonChartLabel(totalValue) {
            if (isNaN(totalValue)) {
                return "";
            }

            var totalPercentLabel = $filter('translate')('Total: {0}%');
            var totalQuantityLabel = $filter('translate')('Total: {0} items');
            var totalHoursLabel = $filter('translate')('Timer is not displayed for this graph');

            switch (self.activeViewType) {
            case ViewType.Percentage:
                return totalPercentLabel.replace('{0}', totalValue);
            case ViewType.Quantitative:
                return totalQuantityLabel.replace('{0}', totalValue);
            case ViewType.Custom:
                return totalHoursLabel;
            default:
                throw new Error('NotImplementException');
            }
        }

        function getChartTotalLabelTranslation(totalValue, viewType, viewName) {
            if (isNaN(totalValue)) {
                return "";
            }

            var totalPercentLabel = $filter('translate')('Total: {0}%');
            var totalQuantityLabel = $filter('translate')('Total: {0} items');
            var totalHoursLabel = $filter('translate')('Total: {0} hours');

            switch (viewType) {
                case ViewType.Percentage:
                    return totalPercentLabel.replace('{0}', totalValue);
                case ViewType.Quantitative:
                    return totalQuantityLabel.replace('{0}', totalValue);
                case ViewType.Custom:
                    return totalHoursLabel.replace('{0}', totalValue);
                default:
                    throw new Error('NotImplementException');
            }
        }

        var departmentsDropDownExpression = function (filterKey, data) {
            var result = data.filter(function (dataItem) {
                return filterKey.some(function (filterKeyitem) {
                    return filterKeyitem.id == dataItem.housingDepartmentId;
                });
            });
            return result;
        };

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.StatisticsPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.StatisticsPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        _.defer(function () {
            initFacilityTasksVsTenantTasksChart();
            initSpentTimeVsTenantTasksChart();
            initSpentTimeVsFacilityTasksChart();
            initTenantTasksVsVisitsAmount();
            initCancelingTenantReasonChart();
            initUnprocessedVsProcessedTasksChart();
            self.getStatistics();
        });

        self.scrollToTop = function () {
            $("body, html").animate({
                scrollTop: 0
            }, 400);
        }

        angular.element(window).on("scroll", function (e) {
            $scope.$apply(function () {
                self.isVisibleButton = window.pageYOffset > 300;
            });
        });

        function getHousingDepartments(managementDepartments) {
            var result = [];

            for (var index = 0, lenth = managementDepartments.length; index < lenth; index++) {
                var managementDepartment = managementDepartments[index];

                managementDepartment.housingDepartments.forEach(function (housingDepartment) {
                    result.push({ id: housingDepartment.id, name: housingDepartment.name });
                });
            }

            return result;
        }

        function getDefaultChartTooltipSettings() {
            function isRound(num) {
                return num % 1 === 0;
            }

            return {
                callbacks: {
                    title: function () {
                        return '';
                    },
                    label: function (tooltipItem, data) {
                        var label = data.labels[tooltipItem.index];
                        var amount = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        var datasetLabel = label + ': ';
                        if (isRound(amount)) {
                            datasetLabel += amount;
                        } else {
                            datasetLabel += amount.toFixed(1);
                        }

                        return datasetLabel;
                    }
                }
            };
        }

        function getDefaultBarChartSettings() {
            var chartSettings = {
                options: {
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes: [
                            {
                                ticks: {
                                    beginAtZero: true
                                }
                            }
                        ]
                    },
                    tooltips: getDefaultChartTooltipSettings()
                }
            };

            return chartSettings;
        }

        function entityAddressDataExpression(url, fileDownloadCallback) {
            var result = {
                buildType: FileBuildType.ServerModel,
                url: url,
                entityDataExpression: function (data, startDate, endDate) {
                    return {
                        rangeDateString: getRangeDateString(startDate, endDate),
                        startDate: startDate,
                        endDate: endDate,
                        addressStatisticInfos: getAddresses(data)
                    }
                },
                fileDownloadCallback: fileDownloadCallback
            };

            return result;
        }

        function entityDataExpression(url, fileDownloadCallback) {
            var result = {
                buildType: FileBuildType.ServerModel,
                url: url,
                entityDataExpression: function (data, startDate, endDate) {
                    _.each(data, function (dataItem, key, collection) {
                        collection[key] = mapGroupedTasks(dataItem);
                    });
                    var request = {
                        rangeDateString: getRangeDateString(startDate, endDate),
                        startDate: startDate,
                        endDate: endDate,
                        groupedTasksIds: data
                    };
                    return request;
                },
                fileDownloadCallback: fileDownloadCallback
            };

            return result;
        }

        //function absenceReasonsDataExpression(url, fileDownloadCallback) { // TODO Don't delete it! Chart hiden.
        //    var result = {
        //        buildType: FileBuildType.ServerModel,
        //        url: url,
        //        entityDataExpression: function (data, startDate, endDate) {
        //            return {
        //                rangeDateString: getRangeDateString(startDate, endDate),
        //                startDate: startDate,
        //                endDate: endDate,
        //                absencesIdList: getAbsencesIdList(data)
        //            }
        //        },
        //        fileDownloadCallback: fileDownloadCallback
        //    };

        //    return result;
        //}

        function cancelingReasonsDataExpressions(url, fileDownloadCallback) {
            var result = {
                buildType: FileBuildType.ServerModel,
                url: url,
                entityDataExpression: function (data, startDate, endDate) {
                    return {
                        rangeDateString: getRangeDateString(startDate, endDate),
                        startDate: startDate,
                        endDate: endDate,
                        dayAssignIdList: getCancelingReasonDayAssignIdList(data)
                    }
                },
                fileDownloadCallback: fileDownloadCallback
            };

            return result;
        }

        function getRangeDateString(startDate, endDate) {
            var startDateString = startDate.toLocaleDateString();
            var endDateString = endDate.toLocaleDateString();
            var rangeDateString = startDateString + '_' + endDateString;
            return rangeDateString;
        }

        function mapGroupedTasks(data) {

            var result = [];
            data.map(function (dataItem) {
                result = result.concat(dataItem.taskId);
            });

            return result;
        }

        //function getAbsencesIdList(data) { // TODO Don't delete it! Chart hiden.
        //    var result = [];
        //    _.each(data, function (groupedAbsencesByReasonId) {
        //        _.each(groupedAbsencesByReasonId, function (absence) {
        //            result.push(absence.absenceId);
        //        });
        //    });

        //    return result;
        //}

        function getAddresses(data) {
            var addresses = [];

            _.each(data, function (dataItem, key, collection) {
                addresses = addresses.concat(collection[key]);
            });

            return addresses;
        }

        function getCancelingReasonDayAssignIdList(data) {
            var result = [];
            _.each(data, function (groupedByCancelingReasonId) {
                _.each(groupedByCancelingReasonId, function (cancelingReason) {
                    result.push(cancelingReason.dayAssignId);
                });
            });

            return result;
        }

        function calcValueExpression(groupedData) {
            var result = groupedData.reduce(function (memo, value, index, array) {
                return memo + array[index].spentTime;
            }, 0);
            return result;
        }

        function createColors() {
            var startRgbColorValue = 255;
            var endRgbColorValue = 60;

            var colors = [];
            for (var colorValue = startRgbColorValue; colorValue > endRgbColorValue; colorValue--) {
                colors.push('rgba(' + 0 + ', ' + colorValue + ', ' + 0 + ', 0.9)');
            }

            return colors;
        }

        var availableColors = createColors();

        function getAvailableColors(chartItemIndex, chartItemsCount) {
            var colorIndex = Math.round((chartItemIndex + 1) * (availableColors.length / chartItemsCount)) - 1;
            var result = availableColors[colorIndex];
            return result;
        }

        function setDataEmpty(context) {
            context.isDataEmpty = true;
            $scope.$applyAsync();
        }

        function setDataNotEmpty(context) {
            context.isDataEmpty = false;
            $scope.$applyAsync();
        }

        function showLoadIcon() {
            self.isActiveLoadIcon = true;
        }

        function hideLoadIcon() {
            self.isActiveLoadIcon = false;
            $scope.$applyAsync();
        }
    };

    angular.module('boligdrift').controller('statisticsController', ['$scope', '$filter', 'statisticsProvider', 'securityService', 'urlService', statisticsController]);
})();