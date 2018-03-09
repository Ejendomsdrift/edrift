(function () {
    var richTextEditor = function () {
        var controller = function ($filter) {
            var self = this;

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerValidate = function () {
                if (self.config.value) {
                    self.includeValidation = true;
                    self.isValidValue = self.config.value.length <= self.config.maxLength;
                    self.maxDescriptionLength = self.config.maxLength - self.config.value.length + ' ' + $filter('translate')('characters left');
                    return self.isValidValue;
                } else {
                    return true;
                }
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.value;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.value;
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit;
            }

            self.summernoteOptions = {
                height: 100,
                toolbar: [
                  ['style', ['bold', 'italic']],
                  ['para', ['ul', 'ol']],
                  ['height', ['height']]
                ],
                disableDragAndDrop: true
            };

            function saveHandler() {
                if (!self.config.triggerValidate()) {
                    return false;
                }

                if (self.config.triggerCheckControlChanged()) {
                    self.config.onSave(self.config.value);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.value = self.savedValue;
                self.includeValidation = false;
                self.isValidValue = true;
            }

            function initControl() {
                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.config.triggerCopySavedData();
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/RichTextEditorControl/richTextEditor.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'richTextCtrl',
            controller: ['$filter', controller]
        };
    }

    angular.module('boligdrift').directive('richTextEditor', [richTextEditor]);
})();