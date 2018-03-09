(function () {
    var urlService = function ($state, $window, memberProvider) {
        var self = this;

        self.defaultRedirect = function () {
            memberProvider.GetCurrentUser().then(function (result) {
                if (result.data) {
                    self.redirectUserToDefaultPage(result.data.currentRole);
                }
                else {
                    $window.localStorage.clear();
                    $window.location.reload();
                }
            });
        }

        self.redirectUserToDefaultPage = function (userRole) {
            switch (userRole) {
                case Role.Administrator:
                    $state.go(State.YearPlanTaskOverview);
                    break;
                case Role.Coordinator:
                    $state.go(State.WeekPlanGridView, { isEditJobPopupClosed: true });
                    break;
                case Role.SuperAdmin:
                    $state.go(State.SetupTranslation);
                    break;
                case Role.Janitor:
                    $state.go(State.JanitorMyTasks);
                    break;
                default:
                    $state.go(State.Home);
                    break;
            }
        }
    };

    angular.module('boligdrift').service('urlService', ['$state', '$window', 'memberProvider', urlService]);

})();