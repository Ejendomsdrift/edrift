(function () {
    var groupOverviewController = function ($scope, $state, $filter, groupProvider, securityService, urlService) {
        var self = this;
        self.state = State;

        function init() {
            checkSecurityPermission();
            groupProvider.GetExistingGroups().then(getGroupsCallback);
        };

        function getGroupsCallback(result) {
            self.groups = result.data;
        }

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

        self.editGroup = function (groupId) {
            $state.go(State.GroupEditGeneral, { id: groupId });
        }

        self.deleteGroup = function (group) {
            if (!group.isCanBeDeleted) {
                alert($filter('translate')('Group cant be deleted'));
            }
            else {
                if (confirm($filter('translate')('Are you sure?'))) {
                    groupProvider.DeleteGroup(group.id).then(function () {
                        self.groups.splice(self.groups.indexOf(group), 1);
                    });
                }
            }
        }

        var managementListener = $scope.$on('managementDepartmentChanged', init);
        $scope.$on('$destroy', function () {
            managementListener();
        });

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.GroupsPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.GroupsPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        init();
    }

    angular.module("boligdrift").controller('groupOverviewController', ['$scope', '$state', '$filter', 'groupProvider', 'securityService', 'urlService', groupOverviewController]);
})();