(function () {
    var categoryTreePicker = function () {
        var controller = function () {
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
                self.savedValue = self.config.value;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedValue !== self.config.value;
            }

            self.isEditMode = function () {
                return self.editControlConfig.mode == ControlMode.edit;
            }

            self.isCreateMode = function () {
                return self.editControlConfig.mode == ControlMode.create;
            }

            self.isViewMode = function () {
                return self.editControlConfig.mode == ControlMode.view;
            }

            self.isDisabledMode = function() {
                return self.editControlConfig.mode == ControlMode.disable;
            }

            self.categoryTreeSelected = function (category) {
                var isItemSelected = false;
                if (category && !category.children.length) { //task can be created only under the lowest level category
                    isItemSelected = true;
                    self.config.value = category.id;

                    if (self.isCreateMode()) {
                        self.config.onSave(self.config.value);
                        self.config.triggerCopySavedData();
                    }
                }

                return isItemSelected;
            };

            self.isCategoryValid = function() {
                return !self.includeValidation || self.isValidValue;
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
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/CategoryTreePicker/categoryTreePicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'categoryTreePickerCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('categoryTreePicker', [categoryTreePicker]);
})();