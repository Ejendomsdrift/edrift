var AdminTranslationsController = function ($http, toastService, securityService, urlService) {

    var baseRoute = '/api/translation';
    var self = this;

    self.currentUser = null;
    self.languages = [];
    self.resources = [];

    self.deleteResource = function (resource) {
        $http.post(baseRoute + '/delete?alias=' + resource.alias).then(function () {
            toastService.showToastSaveSuccessMessage();
            loadResources();
        });
    }

    self.createResource = function () {
        if (!self.newResourceAlias) return;

        var newResource = {
            alias: self.newResourceAlias,
            translation: ''
        }
        self.newResourceAlias = '';

        $http.post(baseRoute + '/createAndReload', newResource).then(function (result) {
            toastService.showToastSaveSuccessMessage();
            self.resources = result.data;
        });
    }

    self.saveResource = function (resource) {
        resource.isEditMode = false;
        $http.post(baseRoute + '/save', resource).then(function (result) {
            toastService.showToastSaveSuccessMessage();
            resource.description = result.data.description;
            resource.translation = result.data.translation;
        });
    }

    self.editResource = function (res) {
        res.isEditMode = true;
    }

    self.afterImport = function ($flow) {
        $flow.files = [];
        loadResources();
        toastService.showToastSaveSuccessMessage();
    };

    self.exportResources = function () {
        window.open('/api/translation/export', '_self', '');
    };

    function loadResources() {

        checkSecurityPermission();

        $http.get(baseRoute + '/all').then(function (result) {
            self.resources = result.data;
        });
    }

    function checkSecurityPermission() {
        securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.SetupPage] }).then(function (result) {
            if (!result.data[ControlSecurityKey.SetupPage]) {
                urlService.defaultRedirect();
            }
        });
    }

    loadResources();
};

var TranslationCreator = function ($http, $timeout) {
    var notExisting = [];
    return function (alias, lang) {
        if (notExisting.indexOf(alias) > -1) return;
        notExisting.push(alias);
        $timeout(function () {
            $http.post('/api/translation/save', { alias: alias, language: lang });
        }, 100);
    };
};

var TranslationLoader = function ($http, $q) {
    return function (options) {
        var deferred = $q.defer();
        $http.get('/api/translation/translations?language=' + options.key)
            .success(function (data) {
                return deferred.resolve(data);
            })
            .error(function () {
                return deferred.reject(options.key);
            });
        return deferred.promise;
    };
};

angular.module('boligdrift').controller('adminTranslationsController', ['$http', 'toastService', 'securityService', 'urlService', AdminTranslationsController]);
angular.module('boligdrift').factory('translationCreator', ['$http', '$timeout', TranslationCreator]);
angular.module('boligdrift').factory('translationLoader', ['$http', '$q', TranslationLoader]);