(function () {
    var managementDepartmentPicker = function () {
        var controller = function () {
            var self = this;

            self.isMultiple = true;
            if (!self.config.selectedDepartments) {
                self.config.selectedDepartments = [];
            }

            self.config.unselectedDepartments = [];

            self.onSelect = function (department) {
                if (self.config.onSelect) {
                    self.config.onSelect(department);
                }
            };

            self.onRemove = function (department) {
                self.config.unselectedDepartments.push(department);
                if (self.config.onRemove) {
                    self.config.onRemove(department);
                }
            };
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/ManagementDepartmentPicker/managementDepartmentPicker.html',
            scope: {
                isMultiple: "=?"
            },
            bindToController: {
                config: '=',
                departments: '='
            },
            controllerAs: 'managementDepartmentCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('managementDepartmentPicker', [
        managementDepartmentPicker
    ]);
})();