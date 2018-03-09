(function () {
    var operationCategoriesController = function (toastService, categoryProvider, securityService, urlService) {
        var self = this;

        self.categoryTreeConfig = {
            showTaskCount: true
        };
        self.categoryTreeSelected = function (category) {
            self.nodeToEdit = category;
            return true;
        };

        self.add = function () {
            self.nodeToEdit = { parentId: self.nodeToEdit.id };
        };

        self.execute = function (command) {
            var handler = categoryProvider[command];
            if (!handler) return;

            handler(self.nodeToEdit).then(function () {
                toastService.showToastSaveSuccessMessage();
            });
        };

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.SetupPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.SetupPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        checkSecurityPermission();
    };

    angular.module('boligdrift').controller('operationCategoriesController', ['toastService', 'categoryProvider', 'securityService', 'urlService', operationCategoriesController]);
})();
