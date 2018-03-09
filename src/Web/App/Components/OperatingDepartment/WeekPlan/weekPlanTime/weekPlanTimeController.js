(function () {
    var weekPlanTimeController = function ($scope, $interval, managementProvider, weekPlanService, dateHelper, memberProvider) {
        var self = this;

        self.init = function (selectedWeek, selectedYear) {
            self.selectedWeek = selectedWeek;
            self.selectedYear = selectedYear;
            self.selectedMemberIds = weekPlanService.getMemberFilter();

            memberProvider.GetCurrentUserContext().then(function (result) {
                self.currentUserContext = result.data;
                if (self.selectedWeek && self.selectedYear) {
                    getTimeView();
                }
            });
        };

        self.minutesToViewFormat = function(minutes, forceMinutesDisplay) {
            return dateHelper.minutesToViewFormat(minutes, forceMinutesDisplay);
        }

        function getTimeView() {
            var model = {
                managementDepartmentId: self.currentUserContext.selectedManagementDepartment.id,
                year: self.selectedYear,
                week: self.selectedWeek,
                memberIds: self.selectedMemberIds
            };

            managementProvider.getManagementDepartmentTimeView(model).then(function (result) {
                self.timeView = result.data;
                weekPlanService.getWeekDays(model.year, model.week).then(function (result) {
                    self.weekDays = result;
                });
            });
        }

        self.getDayFromDate = function (dateString) {
            if (dateString) {
                var date = new Date(dateString);
                return date.getDate();
            }
        }

        var weekPlanFilterChanged = $scope.$on('weekPlanFilterChanged', function (event, data) {
            self.selectedWeek = data.selectedWeek;
            self.selectedYear = data.selectedYear;
            getTimeView();
        });

        var weekPlanMemberFilterChanged = $scope.$on('weekPlanMemberFilterChanged', function (event, data) {
            self.selectedMemberIds = data.selectedMemberIds;
            getTimeView();
        });

        var refreshWeekPlanTimeTasks = $scope.$on('refreshWeekPlanTimeTasks', function () {
            getTimeView();
        });

        $scope.$on('$destroy', weekPlanFilterChanged);
        $scope.$on('$destroy', weekPlanMemberFilterChanged);
        $scope.$on('$destroy', refreshWeekPlanTimeTasks);
    };

    angular.module("boligdrift").controller('weekPlanTimeController', ['$scope', '$interval', 'managementProvider', 'weekPlanService', 'dateHelper', 'memberProvider', weekPlanTimeController]);
})();