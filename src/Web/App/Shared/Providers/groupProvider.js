(function () {
    var groupProvider = function ($http, $q, toastService) {
        var self = this;
        var baseUrl = '/api/group/';

        var showToastAfter = function (promise) {
            var deferred = $q.defer();
            promise.then(function (result) {
                toastService.showToastSaveSuccessMessage();
                deferred.resolve(result);
            });
            return deferred.promise;
        };

        self.CreateGroup = function (name) {
            var encodedGroupName = encodeURI(name);
            return showToastAfter($http.get(baseUrl + 'createGroup?name=' + encodedGroupName));
        }

        self.GetGroupedMembers = function(timeViewDayScope) {
            return $http.post(baseUrl + 'getGroupedMembers', timeViewDayScope);
        }

        self.GetExistingGroups = function () {
            return $http.get(baseUrl + 'getExistingGroups');
        }

        self.GetGroup = function (id) {
            return $http.get(baseUrl + 'get?id=' + id);
        }

        self.DeleteGroup = function (id) {
            return showToastAfter($http.post(baseUrl + 'delete?id=' + id));
        }

        self.AssignMembers = function (model) {
            return showToastAfter($http.post(baseUrl + 'assign', model));
        }

        self.RemoveMember = function (groupId, memberId) {
            var model = { groupId: groupId, value: memberId };
            return showToastAfter($http.post(baseUrl + 'removeMember', model));
        }

        self.IsUniqueName = function (name) {
            var encodedGroupName = encodeURI(name);
            return $http.get(baseUrl + 'isUniqueName?name=' + encodedGroupName);
        }

        self.ChangeName = function (model) {
            return showToastAfter($http.post(baseUrl + 'changeName', model));
        }

        self.isCanRemoveMember = function (groupId, memberId) {
            return $http.get(baseUrl + 'isCanRemoveMember?groupId=' + groupId +'&memberId=' +memberId);
        }
    }

    angular.module('boligdrift').service('groupProvider', ['$http', '$q', 'toastService', groupProvider]);
})();