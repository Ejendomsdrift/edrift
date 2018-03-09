(function () {
    var teamViewer = function () {
        var controller = function (dateHelper) {
            var self = this;
            self.showTooltipInfoBlock = false;

            self.isTeamLead = function (memberId) {
                return memberId === self.config.teamLeadId;
            }

            self.getTotalTimeSpent = function (member) {
                return dateHelper.formatSpentTimeFromHoursAndMinutes(member.totalSpentHours, member.totalSpentMinutes);
            }

            self.getApproximateSpentTime = function (member) {
                return dateHelper.formatSpentTimeFromHoursAndMinutes(member.spentHours, member.spentMinutes);
            }

            self.toogleTooltipInfoBlock = function () {
                self.showTooltipInfoBlock = !self.showTooltipInfoBlock;
            }
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/TeamViewer/teamViewer.html',
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'teamViewerCtrl',
            controller: ['dateHelper', controller]
        };
    }

    angular.module('boligdrift').directive('teamViewer', [teamViewer]);
})();