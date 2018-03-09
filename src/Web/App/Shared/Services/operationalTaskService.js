(function () {
    var operationalTaskService = function () {
        var self = this;

        self.getMode = function (isAccessAllowed, isTaskSaved) {
            if (isTaskSaved && isAccessAllowed) {
                return ControlMode.view;
            } else if (!isTaskSaved && isAccessAllowed) {
                return ControlMode.create;
            } else {
                return ControlMode.disable;
            }
        }

        self.getPreviousPage = function(redirectState) {
            if (redirectState.indexOf(Pages.MyTasks) != -1) {
                return Pages.MyTasks;
            } else if (redirectState.indexOf(Pages.WeekPlan) != -1) {
                return Pages.WeekPlan;
            } else if (redirectState.indexOf(Pages.History) != -1) {
                return Pages.History;
            }
        }
    };

    angular.module('boligdrift').service('operationalTaskService', [operationalTaskService]);

})();