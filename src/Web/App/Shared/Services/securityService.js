(function () {
    var securityService = function (securityProvider, toastService) {
        var self = this;

        self.hasAccessByKeyList = function (model) {
            return securityProvider.hasAccessByKeyList(model);
        }

        self.hasAccessByGroupName = function (model) {
            return securityProvider.hasAccessByGroupName(model);
        }

        self.addKey = function (model) {
            return securityProvider.addKey(model);
        }

        self.addDefaultKeys = function() {
            return securityProvider.addDefaultKeys().then(function () {
                toastService.showToastSaveSuccessMessage();
            });
        }
    };

    angular.module('boligdrift').service('securityService', ['securityProvider', 'toastService', securityService]);

})();