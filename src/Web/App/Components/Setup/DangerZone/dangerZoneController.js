(function () {
    var dangerZoneController = function (adminToolsProvider, securityService, urlService, employeesManagementProvider, $http, dateHelper, cancellingTemplatesProvider) {
        var self = this;

        self.init = function () {

            checkSecurityPermission();

            adminToolsProvider.GetDataBase().then(function (result) {
                self.collections = result.data.map(function (c) { return { name: c, checked: false } });
            });
        }

        self.dropDatabase = function () {
            var checkedCollections = self.collections.filter(function (col) { return col.checked; });
            var collections = checkedCollections.map(function (col) { return col.name }).join(',');
            adminToolsProvider.ClearDataBase(collections);

            checkedCollections.forEach(function (col) {
                var index = self.collections.indexOf(col);
                if (index !== -1) {
                    self.collections.splice(index, 1);
                }
            });
        }

        self.createDefaultCategories = function() {
            adminToolsProvider.CreateDefaultCategories();
        }

        self.createDefaultSecurityKeys = function () {
            securityService.addDefaultKeys();
        }

        self.init();

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.SetupPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.SetupPage]) {
                    urlService.defaultRedirect();
                }
            });
        }
    };

    angular.module('boligdrift').controller('dangerZoneController',
        [
            'adminToolsProvider',
            'securityService',
            'urlService',
            'employeesManagementProvider',
            '$http',
            'dateHelper',
            'cancellingTemplatesProvider',
            dangerZoneController
        ]);
})();
