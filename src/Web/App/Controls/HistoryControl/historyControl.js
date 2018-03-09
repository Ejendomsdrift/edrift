(function () {
    var historyControl = function () {
        var controller = function (historyProvider, dateHelper, $state, $scope, weekPlanService, memberProvider) {
            var self = this;
            self.column = HistoryControlColumns;
            self.jobStatusNameList = [];
            self.historyList = [];

            self.getFormattedDateString = function (date) {
                return dateHelper.getLocalDateString(date);
            }

            self.getFormattedTimeString = function (date) {
                return dateHelper.getTimeString(date);
            }

            self.viewJob = function (tenantJobHistoryLine) {
                if (!self.config.disableViewJobPopup) {
                    $state.go(State.OperationalTaskEditTenant,
                    {
                        id: tenantJobHistoryLine.jobId,
                        dayAssignId: tenantJobHistoryLine.dayAssignId,
                        departmentId: tenantJobHistoryLine.jobHousingDepartmentId,
                        jobType: JobType.Tenant,
                        redirectState: $state.current.name
                    });
                }
            }

            self.showColumn = function (column) {
                return self.config.allowedColumns.indexOf(column) > -1;
            }

            self.getSpentTime = function (historyLine) {
                return dateHelper.formatSpentTimeFromHoursAndMinutes(historyLine.reportedHours, historyLine.reportedMinutes);
            }

            self.onChangeAddress = function (address) {
                if (address) {
                    self.isActiveWaitIcon = true;
                    historyProvider.getHistoryByAddress(address).then(function (result) {
                        weekPlanService.storeHistoryAddress(address);
                        fillHistory(result);
                        self.isActiveWaitIcon = false;
                    });
                } else {
                    self.historyList = [];
                    self.showHistoryTable = false;
                }
            }

            self.getStatusTranslation = function (status) {
                return JobStatusPlatformKey.CoordinatorPlatform + self.jobStatusNameList[status];
            }

            self.showNoDataLabel = function() {
                if (self.config.disableViewJobPopup) {
                    return self.historyList.length == 0 && !self.isActiveWaitIcon;
                } else {
                    return self.selectedAddress && !self.showHistoryTable && !self.isActiveWaitIcon;
                }
            }

            function checkFilteredByAddressHistoryItems(address) {
                if (self.historyList.length == 0) {
                    self.showHistoryTable = false;
                    return;
                }

                var foundedResult = self.historyList.filter(function (item) {
                    return item.address === address;
                });

                self.showHistoryTable = foundedResult.length > 0;
            }

            function getAddressesForManagementDepartment() {
                historyProvider.getAddressesForManagementDepartment().then(function (result) {
                    self.addressList = result.data;
                });
            }

            function fillHistory(result) {
                self.historyList = result.data;
                if (!self.config.disableViewJobPopup) {
                    fillStoredHistoryAddress();
                }
            }

            function fillJobStatusNameList() {
                for (var name in JobStatus) {
                    if (JobStatus.hasOwnProperty(name)) {
                        self.jobStatusNameList.push(name);
                    }
                }
            }

            function fillStoredHistoryAddress() {
                var address = weekPlanService.getHistoryAddress();

                if (!address) {
                    return;
                }

                self.selectedAddress = address.historyAddress;
                checkFilteredByAddressHistoryItems(self.selectedAddress);
            }

            function manageHistoryTableVisibility() {
                if (self.config.isOpenedFromPopup && self.config.dayAssignId) {
                    fillHistoryForDayAssign();
                } 
            }

            function fillHistoryForDayAssign() {
                self.isActiveWaitIcon = true;
                historyProvider.getHistoryByDayAssignId(self.config.dayAssignId).then(function (result) {
                    fillHistory(result);
                    self.isActiveWaitIcon = false;
                });
            }

            function init() {
                fillJobStatusNameList();
                memberProvider.GetCurrentUserContext().then(function (result) {
                    self.currentUserContext = result.data;
                    getAddressesForManagementDepartment();
                    manageHistoryTableVisibility();
                });
            }

            var managementListener = $scope.$on('managementDepartmentChanged', init);
            $scope.$on('$destroy', function () {
                managementListener();
            });

            init();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/HistoryControl/historyControl.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'historyControlCtrl',
            controller: ['historyProvider', 'dateHelper', '$state', '$scope', 'weekPlanService', 'memberProvider', controller]
        };

    };

    angular.module('boligdrift').directive('historyControl', [historyControl]);

})();