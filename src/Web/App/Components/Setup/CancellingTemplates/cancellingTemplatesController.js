(function () {
    var cancellingTemplatesController = function (cancellingTemplatesProvider, securityService, urlService) {
        var self = this;

        function createTemplate (model) {
            cancellingTemplatesProvider.Create(model).then(function (result) {
                self.config.templates.push(result.data);
                self.config.triggerCleanTemplateData();
            });
        }

        function deleteTemplate (template) {
            cancellingTemplatesProvider.Delete(template.id).then(function() {
                var index = self.config.templates.indexOf(template);
                self.config.templates.splice(index, 1);
            });
        }

        function loadCancellingTemplates() {
            var getForCoordinator = false;
            cancellingTemplatesProvider.GetByFilter(getForCoordinator).then(function (result) {
                self.config.templates = result.data;
            });
        }

        function initDirective() {
            self.config = new CancellingReasonModel();
            self.config.onSave = createTemplate;
            self.config.onDelete = deleteTemplate;
        }

        function initControl() {
            checkSecurityPermission();
            initDirective();
            loadCancellingTemplates();
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.SetupPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.SetupPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    };

    angular.module('boligdrift').controller('cancellingTemplatesController', ['cancellingTemplatesProvider', 'securityService', 'urlService', cancellingTemplatesController]);
})();