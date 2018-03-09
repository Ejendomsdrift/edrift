(function () {
    var tenantTaskPicker = function () {
        var controller = function ($filter) {
            var self = this;

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
            }

            self.config.triggerValidate = function () {
                self.includeValidation = true;
                self.isValidValue = angular.isDefined(self.config.value) && self.config.value !== null && self.config.value !== '';
                return self.isValidValue;
            }

            self.config.triggerCopySavedData = function () {
                self.taskType = getTaskTypeArray();
                if (typeof (self.config.value) === 'number') {
                    self.config.value = self.taskType.find(function (i) { return i.id == self.config.value; });
                }
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
                if (self.editControlConfig.mode == ControlMode.create && self.config.value) {
                    self.config.onSave(self.config.value.id);
                }
            }

            self.isTypeValid = function () {
                return !self.includeValidation || self.isValidValue;
            }

            function getTaskTypeArray() {
                var result = [];

                for (var name in TenantTaskType) {
                    if (TenantTaskType.hasOwnProperty(name)) {
                        var text = $filter('translate')("TenantTaskType_" + name);
                        var item = { id: TenantTaskType[name], name: text };
                        result.push(item);
                    }
                }

                result = result.sortByKey('name', function (item) { return item.id == TenantTaskType.Others; });
                return result;
            }

            function saveHandler() {
                if (!self.config.triggerValidate()) {
                    return false;
                }

                if (self.config.triggerCheckControlChanged()) {
                    self.config.onSave(self.config.value.id);
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
            templateUrl: '/App/Controls/TenantTypePicker/tenantTaskPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'taskTypePickerCtrl',
            controller: ['$filter', controller]
        };
    }

    angular.module('boligdrift').directive('tenantTaskPicker', [tenantTaskPicker]);
})();