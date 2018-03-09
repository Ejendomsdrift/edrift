(function () {
    var phoneInputControl = function () {
        var controller = function () {
            var self = this;

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerCopySavedData = function () {
                self.savedValue = self.config.value;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.value;
            }

            self.setValue = function () {
                if (self.editControlConfig.mode == ControlMode.create) {
                    self.config.onSave(self.config.value);
                }
            }

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit || self.editControlConfig.mode == ControlMode.create;
            }

            self.isWindowsPhone = function () {
                return navigator.userAgent.match(/Windows Phone/i) == 'Windows Phone';
            }

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    self.config.onSave(self.config.value);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.value = self.savedValue;
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
            templateUrl: '/App/Controls/PhoneInputControl/phoneInputControl.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'phoneCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('phoneInputControl', [phoneInputControl]);
})();