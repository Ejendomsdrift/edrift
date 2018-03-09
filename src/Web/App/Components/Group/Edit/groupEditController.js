(function () {
    var groupEditController = function ($stateParams) {
        var self = this;
        self.state = State;
        self.groupId = $stateParams.id;
    };

    angular.module('boligdrift').controller('groupEditController', ['$stateParams', groupEditController]);
})();
