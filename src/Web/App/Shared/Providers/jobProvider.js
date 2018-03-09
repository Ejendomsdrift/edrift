var jobProvider = function ($http, $q, $rootScope, toastService) {
    var self = {};
    var baseRoute = '/api/job';

    var showToastAfter = function (promise) {
        var deferred = $q.defer();
        promise.then(function (result) {
            toastService.showToastSaveSuccessMessage();
            deferred.resolve(result);
        });
        return deferred.promise;
    };

    self.GetFacilityTask = function (id, housingDepartmentId) {
        return $http.get(baseRoute + '/getJob?id=' + id + '&housingDepartmentId=' + housingDepartmentId);
    }

    self.GetJobAssigns = function (id, contentType) {
        return $http.get(baseRoute + '/getJobAssigns?jobId=' + id + '&contentType=' + contentType);
    }

    self.GetJobDepartments = function (id) {
        return $http.get(baseRoute + '/getJobDepartments?jobId=' + id);
    }

    self.GetHousingDepartmentsForGroupingTasks = function (id) {
        return $http.get(baseRoute + '/getHousingDepartmentsForGroupingTasks?jobId=' + id);
    }

    self.GetOpenedJobsHeaderList = function () {
        return $http.get(baseRoute + '/getOpenedJobsHeaderList');
    }

    self.GetMyJobsHeaderList = function () {
        return $http.get(baseRoute + '/getMyJobsHeaderList');
    }

    self.GetClosedJobsHeaderList = function () {
        return $http.get(baseRoute + '/getClosedJobsHeaderList');
    }

    self.GetJobCounters = function () {
        return $http.get(baseRoute + '/getJobCounters');
    }

    self.GetJobDetailsById = function (dayAssignId) {
        return $http.get(baseRoute + '/getJobDetailsByDayAssignId?dayAssignId=' + dayAssignId);
    }

    self.GetTenantJobsRelatedByAddress = function (jobId) {
        return $http.get(baseRoute + '/getTenantJobsRelatedByAddress?jobId=' + jobId);
    }

    self.StartJob = function (dayAssignId) {
        return $http.get(baseRoute + '/startJob?dayAssignId=' + dayAssignId);
    }

    self.UnassignJob = function (model) {
        return $http.post(baseRoute + '/unassignJob', model);
    }

    self.PauseJob = function (dayAssignId) {
        return $http.get(baseRoute + '/pauseJob?dayAssignId=' + dayAssignId);
    }

    self.CompleteJob = function (model) {
        return $http.post(baseRoute + '/completeJob', model);
    }

    self.AssignJob = function (dayAssignId) {
        return $http.get(baseRoute + '/assignJob?dayAssignId=' + dayAssignId);
    }

    self.GetUploadedFile = function (dayAssignId) {
        return $http.get(baseRoute + '/getUploadedFile?dayAssignId=' + dayAssignId);
    }

    self.ReopenJob = function (dayAssignId) {
        return $http.get(baseRoute + '/reopenJob?dayAssignId=' + dayAssignId);
    }

    self.Create = function (categoryId, title, housingDepartmentId) {
        var model = {
            categoryId: categoryId,
            title: title
        };

        var deferred = $q.defer();
        showToastAfter($http.post(baseRoute + '/create', model)).then(function (result) {
            self.GetFacilityTask(result.data, housingDepartmentId).then(function (resultGet) {
                deferred.resolve(resultGet);
            });
        });

        return deferred.promise;
    }

    self.Assign = function (model) {
        return showToastAfter($http.post(baseRoute + '/assign', model));
    }

    self.ChangeTitle = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveTitle', model));
    }

    self.SaveDescription = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveDescription', model));
    }

    self.ChangeCategory = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveCategory', model));
    }

    self.saveTillYear = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveTillYear', model));
    }

    self.saveWeekList = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveWeekList', model));
    }

    self.SaveJobShedule = function (model) {
        return showToastAfter($http.post(baseRoute + '/saveJobShedule', model));
    }

    self.Hide = function (model) {
        return showToastAfter($http.post(baseRoute + '/hide', model));
    }

    self.LockInterval = function (model) {
        return showToastAfter($http.post(baseRoute + '/lockInterval', model));
    }

    self.AssignsMembersGroup = function (model) {
        return $http.post(baseRoute + '/assignsMembersGroup', model);
    }

    self.ChangeDayAssignDate = function (model) {
        return showToastAfter($http.post(baseRoute + '/changeDayAssignDate', model));
    }

    self.ChangeDayAssignEstimate = function (model) {
        return showToastAfter($http.post(baseRoute + '/changeDayAssignEstimate', model));
    }

    self.ChangeJobAssignEstimate = function (model) {
        return showToastAfter($http.post(baseRoute + '/changeJobAssignEstimate', model));
    }

    self.ChangeJobAssignTeam = function(model) {
        return showToastAfter($http.post(baseRoute + '/changeJobAssignTeam', model));
    }

    self.GetJobAssignEstimate = function(jobId, housingDepartmentId) {
        return $http.get(baseRoute + '/getJobAssignEstimate?jobId=' + jobId + '&housingDepartmentId='+housingDepartmentId);
    }

    self.GetJobAssignTeam = function(jobId, housingDepartmentId) {
        return $http.get(baseRoute + '/getJobAssignTeam?jobId=' + jobId + '&housingDepartmentId=' + housingDepartmentId);
    }

    self.DayAssignCancel = function (model) {
        return showToastAfter($http.post(baseRoute + '/dayAssignCancel', model));
    }

    self.GetWeekTask = function (model) {
        return $http.post(baseRoute + '/getWeekTask', model);
    }

    self.GetLocationModel = function (id, housingDepartmentId) {
        return $http.get(baseRoute + '/getLocationModel?jobId=' + id + "&housingDepartmentId=" + housingDepartmentId);
    }

    self.SaveAddress = function (jobId, model) {
        return showToastAfter($http.post(baseRoute + '/saveAddress?jobId=' + jobId, model));
    }

    self.GetCurrentHousingDepartmentAddresses = function (housingDepartmentId) {
        return $http.get(baseRoute + '/getCurrentHousingDepartmentAddresses?housingDepartmentId=' + housingDepartmentId);
    }

    self.IsAllowedTaskGrouping = function (id) {
        return $http.get(baseRoute + '/isAllowedTaskGrouping?jobId=' + id);
    }

    self.AddTaskToRelationGroup = function (jobId, housingDepartmentId) {
        return $http.get(baseRoute + '/addTaskToRelationGroup?jobId=' + jobId + "&housingDepartmentId=" + housingDepartmentId);
    }

    self.IsGroupedTask = function (jobId) {
        return $http.get(baseRoute + '/isGroupedTask?jobId=' + jobId);
    }

    self.IsChildGroupedTask = function (jobId) {
        return $http.get(baseRoute + '/isChildGroupedTask?jobId=' + jobId);
    }

    self.CreateTaskRelationGroup = function (jobId, housingDepartmentId) {
        return $http.get(baseRoute + '/createTaskRelationGroup?jobId=' + jobId + "&housingDepartmentId=" + housingDepartmentId);
    }

    self.GetJobGuideComments = function(jobId) {
        return $http.get(baseRoute + '/getJobGuideComments?jobId=' + jobId);
    }

    self.SaveOrUpdateGuideComment = function (model) {
        return $http.post(baseRoute + '/saveOrUpdateGuideComment', model);
    }

    self.RemoveGuideComment = function (commentId) {
        return $http.get(baseRoute + '/removeGuideComment?commentId=' + commentId);
    }

    self.getApproximateSpentTime = function (model) {
        return $http.post(baseRoute + '/getApproximateSpentTime', model);
    }

    return self;
}

angular.module('boligdrift').service('jobProvider', ['$http', '$q', '$rootScope', 'toastService', jobProvider]);