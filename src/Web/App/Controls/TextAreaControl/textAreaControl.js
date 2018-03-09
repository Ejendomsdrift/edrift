(function () {
    var textAreaControl = function () {
        var controller = function () {
            var self = this;

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerValidate = function () {
                if (self.config.isValidationRequired) {
                    self.includeValidation = true;
                    self.isValidValue = angular.isDefined(self.config.value) && self.config.value !== null && self.config.value !== '';
                    return self.isValidValue;
                }

                return true;
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.value;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.value;
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit || self.editControlConfig.mode == ControlMode.create;
            }

            self.isConfigValueValid = function () {
                return !self.includeValidation || self.isValidValue;
            }

            self.setValue = function () {
                if (self.editControlConfig.mode == ControlMode.create) {
                    self.config.onSave(self.config.value);
                }
            }

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
            templateUrl: '/App/Controls/TextAreaControl/textAreaControl.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'textAreaCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('textAreaControl', [textAreaControl]);
})();