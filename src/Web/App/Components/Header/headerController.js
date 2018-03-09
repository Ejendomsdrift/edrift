(function () {
    var headerController = function ($scope, $rootScope, memberProvider, $state) {
        var self = this;
        self.role = Role;
        
        self.onManagementChanged = function (managementDepartment) {
            memberProvider.SaveManagementDepartment(managementDepartment.id).then(function() {
                $rootScope.$broadcast('managementDepartmentChanged');
            }); 
        }

        self.isStatisticsPage = function() {
            return $state.$current.name === State.Statistics;
        }

        function loadManagments() {
            memberProvider.GetCurrentUserContext().then(function (result) {
                self.currentUserContext = result.data;
            });
        }

        function initControl() {
            loadManagments();
        }

        initControl();
    };

    angular.module('boligdrift').controller('headerController', ['$scope', '$rootScope', 'memberProvider', '$state', headerController]);
})();
