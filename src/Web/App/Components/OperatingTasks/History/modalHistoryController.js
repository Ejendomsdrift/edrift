(function() {
    var modalHistoryController = function ($stateParams) {
        var self = this;

        function init() {
            self.historyControlConfig = new HistoryModel();
            self.historyControlConfig.disableViewJobPopup = true;
            self.historyControlConfig.dayAssignId = $stateParams.dayAssignId;
            self.historyControlConfig.isOpenedFromPopup = true;

            self.historyControlConfig.allowedColumns = [
                HistoryControlColumns.changeStatusDate,
                HistoryControlColumns.whoChangedStatus,
                HistoryControlColumns.status,
                HistoryControlColumns.changeStatusComment,
                HistoryControlColumns.cancellationReason,
                HistoryControlColumns.fileName,
                HistoryControlColumns.reportedTime
            ];

        }

        init();
    };

    angular.module('boligdrift').controller('modalHistoryController', ['$stateParams', modalHistoryController]);
})();