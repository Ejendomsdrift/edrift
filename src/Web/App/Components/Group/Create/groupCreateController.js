(function () {
    var groupCreateController = function ($state, groupProvider) {
        var self = this;
        self.state = State;
        self.isUnique = true;

        self.onEnter = function () {
            if (!self.groupName) {
                return;
            }
            self.create();
        }

        self.create = function () {
            groupProvider.IsUniqueName(self.groupName).then(function (result) {
                self.isUnique = result.data;
                if (self.isUnique) {
                    groupProvider.CreateGroup(self.groupName).then(function (result) {
                        $state.go(State.GroupEditGeneral, { id: result.data });
                    });
                }
            });
        };
    };

    angular.module('boligdrift').controller('groupCreateController', ['$state', 'groupProvider', groupCreateController]);
})();