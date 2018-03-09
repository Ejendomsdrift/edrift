(function () {
    var multiDepartmentPicker = function (departmentProvider) {
        var controller = function ($scope) {
            var self = this;
            
            function reloadDepartments() {
                departmentProvider.getDepartments().then(function(result) {
                    self.departments = getFilteredDepartments(result.data);
                });
            };

            var managementListener = $scope.$on('managementDepartmentChanged', reloadDepartments);

            $scope.$on('$destroy', function () {
                managementListener();
            });

            reloadDepartments();

            self.hideUnassignButton = function (department) {
                return department.isDisabled;
            }

            self.isDisabled = function (department) {
                var result = self.ngHidden.length && self.ngHidden.map(function (d) { return d.id }).indexOf(department.id) != -1;
                return result;
            };

            function getFilteredDepartments(housingDepartments) {
                var result = [];

                if (self.ngModel) {
                    var assignedDepartmentIds = self.ngModel ? self.ngModel.map(function (d) { return d.id }) : [];

                    for (var k = 0; k < housingDepartments.length; k++) {
                        if (assignedDepartmentIds.indexOf(housingDepartments[k].id) === -1) {
                            result.push(housingDepartments[k]);
                        }
                    }
                } else {
                    result = housingDepartments;
                }

                return result;
            }
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/DepartmentPicker/MultiSelectDepartmentPicker/multiDepartmentPicker.html',
            scope: {},
            bindToController: {
                ngModel: '=',
                ngHidden: '=',
                onSelect: '&',
                onRemove: '&'
            },
            controllerAs: 'multiDepartmentPickerCtrl',
            controller: ['$scope', controller]
        };
    }

    angular.module('boligdrift').directive('multiDepartmentPicker', ['departmentProvider', multiDepartmentPicker]);
})();