angular.module('boligdrift').controller('ConfirmCtrl', [
    '$scope', '$uibModalInstance', function($scope, $uibModalInstance) {
        var self = this;

        self.ok = function() {
            $uibModalInstance.close(true);
        };

        self.cancel = function() {
            $uibModalInstance.close(false);
        };
    }
]);