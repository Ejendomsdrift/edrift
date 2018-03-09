var CategoryProvider = function ($http) {
    var baseUrl = '/api/category';

    var get = function (includeGroupedTasks, includeHiddenTasks, showTasks) {
        return $http.get(baseUrl + '/get?includeGroupedTasks=' + includeGroupedTasks + '&includeHiddenTasks=' + includeHiddenTasks + '&showTasks=' + showTasks);
    };

    var save = function (node) {
        return $http.post(baseUrl + '/save', node);
    };

    var hide = function (node) {
        return $http.post(baseUrl + '/hide', node);
    };

    var show = function (node) {
        return $http.post(baseUrl + '/show', node);
    };

    return {
        getTree: get,
        save: save,
        hide: hide,
        show: show
    };
};

angular.module('boligdrift').factory('categoryProvider', ['$http', CategoryProvider]);
