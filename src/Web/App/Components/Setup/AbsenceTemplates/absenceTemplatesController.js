(function() {
    var absenceTemplatesController = function (absenceTemplatesProvider, securityService, urlService) {

        var self = this;
        self.absenceTemplates = [];
        self.newTemplateText = "";

        self.add = function(templateText) {
            absenceTemplatesProvider.Create(templateText)
                .then(function (result) {
                    self.absenceTemplates.push(result.data);
                    self.newTemplateText = "";
                });
        }

        self.delete = function(template) {
            absenceTemplatesProvider.Delete(template.id);
            var i = self.absenceTemplates.indexOf(template);
            self.absenceTemplates.splice(i, 1);
        }

        function loadAbsenceTemplates() {
             absenceTemplatesProvider.GetAll()
                .then(function(result) {
                    self.absenceTemplates = result.data;                  
                });
        }

        function initControl() {
            checkSecurityPermission();
            loadAbsenceTemplates();
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.SetupPage] }).then(function(result) {
                if (!result.data[ControlSecurityKey.SetupPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();
    };

    angular.module('boligdrift')
        .controller('absenceTemplatesController', ['absenceTemplatesProvider', 'securityService', 'urlService', absenceTemplatesController]);
})();