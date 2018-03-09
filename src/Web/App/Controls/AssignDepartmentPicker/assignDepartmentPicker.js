(function() {
    var assignDepartmentPicker = function() {
        var controller = function() {
            var self = this;
            self.assignDepartments = [];
            self.unassignDepartments = [];

            self.config.triggerRefresh = function () {
                self.config.triggerCopySavedData();
                self.editControlConfig.mode = self.config.mode;
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

            self.onSelect = function (department) {
                self.assignDepartments.push(department);
            };

            self.onRemove = function (department) {
                self.unassignDepartments.push(department);
            };

            function saveHandler() {
                if (self.config.triggerCheckControlChanged()) {
                    var assignedDepartmentIds = self.assignDepartments.map(function (dept) { return dept.id });
                    var unassignedDepartmentIds = self.unassignDepartments.map(function (dept) { return dept.id; });

                    self.config.onSave(assignedDepartmentIds, unassignedDepartmentIds);
                    self.config.triggerCopySavedData();

                    self.assignDepartments = [];
                    self.unassignDepartments = [];
                }

                return true;
            }

            function cancelHandler() {
                self.config.value = self.savedValue;
                self.isValidValue = true;

                self.assignDepartments = [];
                self.unassignDepartments = [];
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
            templateUrl: '/App/Controls/AssignDepartmentPicker/assignDepartmentPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'assignDepartmentCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('assignDepartmentPicker', [assignDepartmentPicker]);
})();