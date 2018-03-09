(function () {
    var departmentPicker = function (departmentProvider, jobProvider) {
        var controller = function ($scope, memberProvider, $filter) {
            var self = this;
            self.isActiveWaitIcon = false;
            
            self.config.triggerValidate = function () {
                self.includeValidation = true;
                self.isDepartmentIdValid = self.ngModel ? true : false;
                return self.isDepartmentIdValid;
            }

            self.changeDepartment = function ($item) {
                self.isDepartmentIdValid = $item ? true : false;

                var updatedModel = $item ? $item.obj : null;                
                self.config.onChange();
                self.config.onSelect(updatedModel);                
            }

            function reloadDepartments() {  
                self.isActiveWaitIcon = true;
                memberProvider.GetCurrentUserContext().then(function (result) {
                    self.currentUserContext = result.data;

                    if (self.config.jobId) {
                        jobProvider.GetHousingDepartmentsForGroupingTasks(self.config.jobId).then(function (result) {
                            updateDepartments(result.data);
                            self.isActiveWaitIcon = false;
                        });
                    }
                    else {
                        departmentProvider.getDepartments().then(function (result) {
                            updateDepartments(result.data);
                            self.isActiveWaitIcon = false;
                        });
                    }
                });
            };

            function getDataListModel(list) {
                var result = [];
                if (list) {
                    for (var i = 0; i < list.length; i++) {
                        result.push({ value: list[i].name, id: list[i].id, obj: list[i] });
                    }
                }

                return result;
            }

            function updateDepartments(data) {
                self.selectControlConfig.dataList = getDataListModel(data);
                self.selectControlConfig.triggerUpdateDataList();
                mapSelectedItems();
                self.config.onSelect(self.ngModel);
                
                if (self.ngModel) {
                    self.selectControlConfig.selected = { value: self.ngModel.name, id: self.ngModel.id, obj: self.ngModel };
                } else {
                    self.selectControlConfig.selected = null;
                }
                self.selectControlConfig.triggerSetValue();
            }

            function mapSelectedItems() {
                var isClearMultiselectDepartmentPicker = isDepartmentMultiselectShouldBeCleared();
                if (isClearMultiselectDepartmentPicker) {
                    self.ngModel = null;
                }
            }

            function isDepartmentMultiselectShouldBeCleared() {
                var selectedManagement = self.currentUserContext.selectedManagementDepartment;
                var currentRole = self.currentUserContext.memberModel.currentRole;

                var result = self.ngModel && selectedManagement && self.ngModel.managementDepartmentId !== selectedManagement.id && currentRole !== Role.Administrator;
                return result;
            }

            function initSelectControl() {
                self.selectControlConfig = new SelectControlModel();
                self.selectControlConfig.placeholder = $filter('translate')('Choose Housing department');
                self.selectControlConfig.onUpdate = self.changeDepartment;
                self.selectControlConfig.onRemove = self.changeDepartment;
                self.selectControlConfig.showAll = self.config.showAll;
                self.selectControlConfig.showAllTranslationKey = self.config.showAllTranslationKey;
            }

            var managementListener = $scope.$on('managementDepartmentChanged', reloadDepartments);

            $scope.$watch(function () { return self.config.showAll; }, function (newValue, oldValue) {
                if (newValue != oldValue) {
                    self.selectControlConfig.showAll = newValue;

                    if (self.ngModel) {
                        self.selectControlConfig.selected = { value: self.ngModel.name, id: self.ngModel.id, obj: self.ngModel };
                    } else {
                        self.selectControlConfig.selected = null;
                    }
                    self.selectControlConfig.triggerSetValue();
                }
            });

            $scope.$on('$destroy', function () {
                managementListener();
            });

            initSelectControl();

            reloadDepartments();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/DepartmentPicker/SingleSelectDepartmentPicker/departmentPicker.html',
            scope: {},
            bindToController: {
                config: "=",
                ngModel: '='
            },
            controllerAs: 'departmentPickerCtrl',
            controller: ['$scope', 'memberProvider', '$filter', controller]
        };
    }

    angular.module('boligdrift').directive('departmentPicker', ['departmentProvider', 'jobProvider', departmentPicker]);
})();