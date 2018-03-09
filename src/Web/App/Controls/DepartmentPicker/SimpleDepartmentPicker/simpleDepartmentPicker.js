(function () {
    var simpleDepartmentPicker = function () {
        var controller = function ($scope) {
            var self = this;

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

            $scope.$watch(function watchDepartments() {
                return self.departments;
            }, function checkSelectedDepartments() {
                if (self.config.selectedDepartments.length) {
                    self.config.selectedDepartments = _.intersection(self.config.selectedDepartments, self.departments);
                }
            });
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/DepartmentPicker/SimpleDepartmentPicker/simpleDepartmentPicker.html',
            scope: {},
            bindToController: {
                config: '=',
                departments: '='
            },
            controllerAs: 'simpleDepartmentCtrl',
            controller: ['$scope', controller]
        };
    }

    angular.module('boligdrift').directive('simpleDepartmentPicker', [simpleDepartmentPicker]);
})();