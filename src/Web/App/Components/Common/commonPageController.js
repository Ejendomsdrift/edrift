(function () {
    var commonPageController = function (
        authorizationProvider,
        memberProvider,
        settingProvider,
        jobProvider,
        $document,
        $stateParams,
        $state,
        $window,
        securityService,
        weekPlanService,
        urlService,
        $scope) {

        var self = this;
        self.roleEnum = Role;
        self.isAuthorizedUser = false;
        self.isMobileMenuOpen = false;
        self.state = State;

        self.isAdministration = function () {
            if (self.currentUser) {
                return self.currentUser.currentRole == Role.Administrator;
            }
        }

        self.isManagement = function () {
            if (self.currentUser) {
                return self.currentUser.currentRole == Role.Coordinator;
            }
        }

        self.isJanitor = function () {
            if (self.currentUser) {
                return self.currentUser.currentRole == Role.Janitor;
            }
        }

        self.toggleMobileMenu = function () {
            self.isMobileMenuOpen = !self.isMobileMenuOpen;
        }

        self.openMobilePage = function (state) {
            self.isMobileMenuOpen = false;
            $state.go(state);
        }

        self.Logout = function () {
            authorizationProvider.LogOut().then(function () {
                $window.localStorage.clear();
                $window.location.reload();
            });
        }

        self.switchPlatform = function () {
            memberProvider.SwitchMemberToNextAvailableRole(self.currentUser.memberId).then(function () {
                $window.location.href = "/";
            });
        }

        self.redirectToDefaultPage = function () {
            urlService.redirectUserToDefaultPage(self.currentUser.currentRole);
        }

        self.createOperationalTasks = function() {
            $state.go(State.OperationalTaskCreateAdHoc, { redirectState: self.redirectState });
        }

        function initSignalRConnection() {
            if (self.isJanitor()) {
                var janitorHubProxy = $.connection.janitorHub;
                janitorHubProxy.client.refreshJanitorTasks = function() {
                    $scope.$broadcast('refreshJanitorTasks', {});
                };
                janitorHubProxy.client.refreshOpenTasks = function() {
                    $scope.$broadcast('refreshOpenTasks', {});
                };
                janitorHubProxy.client.refreshCompletedTasks = function() {
                    $scope.$broadcast('refreshCompletedTasks', {});
                };
                janitorHubProxy.client.refreshPage = function () {
                    loadCurrentMember(true);
                };
                $.connection.hub.start();
            }

            if (self.isManagement()) {
                var managementHubProxy = $.connection.managementHub;
                managementHubProxy.client.refreshWeekPlanGridTasks = function() {
                    $scope.$broadcast('refreshWeekPlanGridTasks', {});
                };
                managementHubProxy.client.refreshWeekPlanListTasks = function() {
                    $scope.$broadcast('refreshWeekPlanListTasks', {});
                };
                managementHubProxy.client.refreshWeekPlanTimeTasks = function() {
                    $scope.$broadcast('refreshWeekPlanTimeTasks', {});
                };
                managementHubProxy.client.refreshPage = function () {
                    loadCurrentMember(true);
                };
                $.connection.hub.start();
            }
        }

        function redirect(name) {
            switch (name) {
                case State.GroupCreate:
                case State.GroupEditGeneral:
                    $state.go(State.Groups, { showDisabled: $stateParams.showDisabled });
                    break;
                case State.FacilityTaskDayAssign:
                case State.OperationalTaskCreateAdHoc:
                case State.OperationalTaskEditAdHoc:
                case State.OperationalTaskEditTenant:
                case State.OperationalTaskEditOther:
                    weekPlanService.clearStoredDayAssign();
                    var redirectState = $stateParams.redirectState ? $stateParams.redirectState : State.WeekPlanGridView; 
                    self.currentUser.currentRole === Role.Janitor ? $state.go(State.JanitorMyTasks) : $state.go(redirectState, { isEditJobPopupClosed: true, listViewCurrentTab: $stateParams.listViewCurrentTab });
                    break;
                default:
                    $state.go(State.YearPlanTaskOverview, { showDisabled: $stateParams.showDisabled, isShowCategoryPanel: true, showAll: $stateParams.showAll });
                    break;
            }
        }

        function keydownListener(evt) {
            if (evt.which === KeyCodes.Esc) {
                redirect($state.$current.name);
            }
        }

        function loadCurrentMember(redirectToHome) {
            
            memberProvider.GetCurrentUserContext().then(function (result) {
                
                self.currentUserContext = result.data;
                self.currentUser = result.data.memberModel;
                self.redirectState = self.currentUser.currentRole === Role.Janitor ? State.JanitorMyTasks : State.WeekPlanGridView;
                if (self.currentUser) {
                    initSignalRConnection();
                    self.isAuthorizedUser = true;

                    if (redirectToHome) {
                        $window.location.href = "/";
                    } else if ($window.location.hash === "#/") {
                        urlService.redirectUserToDefaultPage(self.currentUser.currentRole);
                    }
                }
            });
        }

        function initControl() {
            $document.on('keydown', keydownListener);
            loadCurrentMember();

            settingProvider.isADFSMode().then(function (result) {
                self.isADFSMode = result.data;
            });            
        }

        var janitorTaskCountChanged = $scope.$on('janitorTaskCountChanged', function (event, data) {
            jobProvider.GetJobCounters().then(function (result) {
                self.myTaskCount = result.data.janitorTasksCount;
                self.openedTaskCount = result.data.openedTasksCount;
            });
        });

        $scope.$on('$destroy', janitorTaskCountChanged);

        initControl();
    };

    angular.module('boligdrift').controller('commonPageController',
        [
            'authorizationProvider',
            'memberProvider',
            'settingProvider',
            'jobProvider',
            '$document',
            '$stateParams',
            '$state',
            '$window',
            'securityService',
            'weekPlanService',
            'urlService',
            '$scope',
            commonPageController]);
})();