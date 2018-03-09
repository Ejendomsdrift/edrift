(function () {
    var authorizationController = function (authorizationProvider, $state, $window, $timeout, urlService, memberProvider, settingProvider) {
        var self = this;

        self.signin = function () {
            authorizationProvider.LogIn(self.login, self.password).then(function (result) {
                var data = result.data;
                self.isLoginNotExist = !data.isLoginExist;
                self.isPasswordNotExist = !data.isPasswordExist;
                self.userHasNotRole = !data.userHasRole;
                self.isUserDisabled = data.isUserDisabled;

                if (data.isLoginExist && data.isPasswordExist && data.userHasRole && !data.isUserDisabled) {
                    urlService.redirectUserToDefaultPage(data.userRole);

                    $timeout(function () {
                        $window.location.reload();
                    }, 500);
                }
            });
        };

        function clearScrollPosition() {
            var positionToScrolableArea = Number(sessionStorage.getItem('positionToScrolableArea'));
            if (positionToScrolableArea) {
                $('body').scrollTop(0);
            }
        }

        function loadMember() {
            memberProvider.GetCurrentUser().then(function (result) {
                if (result.data) {
                    urlService.redirectUserToDefaultPage(result.data.userRole);
                    $timeout(clearScrollPosition, 0);
                }
            });
        }

        function initControl() {
            settingProvider.isADFSMode().then(function (result) {
                if (result.data) {
                    $state.go(State.Home);
                    $window.location.reload();
                }
                else {
                    loadMember();
                }
            });
        }

        initControl();
    };

    angular.module("boligdrift").controller('authorizationController', ['authorizationProvider',
        '$state',
        '$window',
        '$timeout',
        'urlService',
        'memberProvider',
        'settingProvider',
        authorizationController]);
})();