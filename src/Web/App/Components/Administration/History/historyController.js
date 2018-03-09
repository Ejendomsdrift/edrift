(function() {
    var historyController = function($scope, securityService, urlService) {
        var self = this;

        self.scrollToTop = function() {
            $("body, html").animate({
                    scrollTop: 0
                },
                400);
        }

        angular.element(window).on("scroll",
            function(e) {
                $scope.$apply(function() {
                    self.isVisibleButton = window.pageYOffset > 300;
                });
            });

        function init() {
            checkSecurityPermission();

            self.historyControlConfig = new HistoryModel();
            self.historyControlConfig.isOpenedFromPopup = false;
            self.historyControlConfig.allowedColumns = [
                HistoryControlColumns.residentName,
                HistoryControlColumns.changeStatusDate,
                HistoryControlColumns.title,
                HistoryControlColumns.jobComment,
                HistoryControlColumns.status,
                HistoryControlColumns.changeStatusComment,
                HistoryControlColumns.cancellationReason,
                HistoryControlColumns.fileName
            ];
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.HistoryPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.HistoryPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        init();
    };

    angular.module("boligdrift").controller('historyController', ['$scope', 'securityService', 'urlService', historyController]);
})();