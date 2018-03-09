(function() {
    var operationalTaskProvider = function($http, $q, toastService) {
        var self = {};
        var baseRoute = '/api/operationalTask/';

        var showToastAfter = function (promise) {
            var deferred = $q.defer();
            promise.then(function (result) {
                toastService.showToastSaveSuccessMessage();
                deferred.resolve(result);
            });
            return deferred.promise;
        };

        self.createAdHocTask = function (adHocTask) {
            var deferred = $q.defer();
            showToastAfter($http.post(baseRoute + 'createAdHocTask', adHocTask)).then(function (result) {
                self.GetAdHocTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.createJanitorAdHocTask = function (adHocTask) {
            var deferred = $q.defer();
            $http.post(baseRoute + 'createJanitorAdHocTask', adHocTask).then(function (result) {
                self.GetAdHocTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.createTenantTask = function(task) {
            var deferred = $q.defer();
            showToastAfter($http.post(baseRoute + 'createTenantTask', task)).then(function (result) {
                self.GetTenantTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.createJanitorTenantTask = function (task) {
            var deferred = $q.defer();
            $http.post(baseRoute + 'createJanitorTenantTask', task).then(function (result) {
                self.GetTenantTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.createOtherTask = function (task) {
            var deferred = $q.defer();
            showToastAfter($http.post(baseRoute + 'createOtherTask', task)).then(function (result) {
                self.GetOtherTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.createJanitorOtherTask = function (task) {
            var deferred = $q.defer();
            $http.post(baseRoute + 'createJanitorOtherTask', task).then(function (result) {
                self.GetOtherTask(result.data.dayAssignId).then(function (resultGet) {
                    deferred.resolve(resultGet);
                });
            });
            return deferred.promise;
        }

        self.GetAdHocTask = function (dayAssignId) {
            return $http.get(baseRoute + 'getAdHocTask?dayAssignId=' + dayAssignId);
        }

        self.GetTenantTask = function (dayAssignId) {
            return $http.get(baseRoute + 'getTenantTask?dayAssignId=' + dayAssignId);
        }

        self.GetOtherTask = function (dayAssignId) {
            return $http.get(baseRoute + 'getOtherTask?dayAssignId=' + dayAssignId);
        }

        self.ChangeTaskType = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeTenantTaskType', model));
        }

        self.ChangeTaskUrgency = function(model) {
            return showToastAfter($http.post(baseRoute + 'changeTenantTaskUrgency', model));
        }

        self.ChangeTaskDate = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeTaskDate', model));
        }

        self.ChangeTaskTime = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeTaskTime', model));
        }

        self.SaveResidentName = function(model) {
            return showToastAfter($http.post(baseRoute + 'changeResidentName', model));
        }

        self.SaveResidentPhone = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeResidentPhone', model));
        }

        self.SaveComment = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeTenantTaskComment', model));
        }

        self.ChangeCategory = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeCategory', model));
        }

        self.ChangeEstimate = function(model) {
            return showToastAfter($http.post(baseRoute + 'changeEstimate', model));
        }

        self.ChangeTitle = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeTitle', model));
        }

        self.ChangeDescription = function (model) {
            return showToastAfter($http.post(baseRoute + 'changeDescription', model));
        }

        self.assignEmployees = function (model) {
            return showToastAfter($http.post(baseRoute + 'assignEmployees', model));
        }

        self.getTaskCreationInfo = function (jobId) {
            return $http.get(baseRoute + 'getTaskCreationInfo?jobId=' + jobId);
        }

        self.GetUploads = function(jobAssignId) {
            return $http.get(baseRoute + 'getUploads?jobAssignId=' + jobAssignId);
        }

        return self;
    }

    angular.module('boligdrift').service('operationalTaskProvider', ['$http', '$q', 'toastService', operationalTaskProvider]);
})();