(function () {
    var addressControl = function () {
        var controller = function ($scope, jobProvider, $filter) {
            var self = this;

            self.isEditableMode = function () {
                return self.editControlConfig.mode == ControlMode.edit || self.editControlConfig.mode == ControlMode.create;
            }

            self.isDisabledMap = function () {
                return self.editControlConfig.mode == ControlMode.disable;
            }

            self.updateMapAddress = function () {
                self.mapAddress = self.config.address;

                if (self.editControlConfig.mode == ControlMode.create) {
                    self.config.onSave(self.config.address);
                }
            }

            self.updateAddress = function (address) {
                self.config.address = address;
                self.mapAddress = address;

                if (self.editControlConfig.mode == ControlMode.create) {
                    self.config.onSave(address);
                }
            }

            self.updateAddressFromSelectControl = function (addressModel) {
                var address = addressModel ? addressModel.value : null;
                self.updateAddress(address);
            }

            self.config.triggerRefresh = function () {
                self.includeValidation = false;
                self.updateMapAddress();
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
                updatePickerAddress(self.config.address);
            }

            self.config.triggerValidate = function () {
                self.includeValidation = self.config.isRequired;
                self.isValidAddress = angular.isDefined(self.config.address) && self.config.address !== null && self.config.address !== '';
                return self.isValidAddress;
            }

            self.config.triggerCopySavedData = function () {
                self.savedAddress = self.config.address;
            }

            self.config.triggerCheckControlChanged = function () {
                return self.savedAddress !== self.config.address;
            }

            function saveHandler() {
                if (self.config.isRequired && !self.config.triggerValidate()) {
                    return false;
                }

                if (self.config.triggerCheckControlChanged()) {
                    self.config.onSave(self.config.address);
                    self.config.triggerCopySavedData();
                }

                return true;
            }

            function cancelHandler() {
                self.config.address = self.savedAddress;
                self.updateMapAddress();

                self.includeValidation = false;
                self.isValidAddress = true;
            }

            function getDataListModel(list) {
                var result = [];
                if (list) {
                    for (var i = 0; i < list.length; i++) {
                        result.push({ value: list[i], id: i });
                    }
                }

                return result;
            }

            function initAddressSelectControl() {
                self.selectControlConfig = new SelectControlModel();
                self.selectControlConfig.placeholder = $filter('translate')('Please add location');
                self.selectControlConfig.onUpdate = self.updateAddressFromSelectControl;
                self.selectControlConfig.onRemove = self.updateAddressFromSelectControl;

                self.departmentId = self.config.departmentId;
                jobProvider.GetCurrentHousingDepartmentAddresses(self.departmentId).then(function (result) {
                    self.selectControlConfig.dataList = getDataListModel(result.data);
                    self.selectControlConfig.triggerUpdateDataList();
                    if (self.config.address) {
                        updatePickerAddress(self.config.address);
                    }
                });
            }

            function updatePickerAddress(newAddress) {
                var address = newAddress ? newAddress : null;
                self.selectControlConfig.selected = { value: address };
                self.selectControlConfig.triggerSetValue();
            }

            function initControl() {
                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;
                self.editControlConfig.showConfirmBeforeSaving = true;
                self.editControlConfig.confirmMessageKey = "You changed the address. Do you want to save it before continue?";

                self.mapAddress = self.config.address;

                self.config.triggerCopySavedData();

                initAddressSelectControl();
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/AddressControl/addressControl.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'addressCtrl',
            controller: ['$scope', 'jobProvider', '$filter', controller]
        };
    }

    angular.module('boligdrift').directive('addressControl', [addressControl]);
})();