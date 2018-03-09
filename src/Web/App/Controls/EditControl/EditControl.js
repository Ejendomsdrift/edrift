(function () {
    var editControl = function () {
        var controller = function ($scope, $filter) {
            var self = this;

            $scope.$watch(function () { return self.config.mode; }, function (newValue, oldValue) {
                if (newValue != oldValue) {
                    self.triggerChangeMode(newValue);
                }
            });


            self.save = function (isSaveButton) {
                if (!self.isActiveEditMode) {
                    return;
                }

                var msg = $filter('translate')(self.config.confirmMessageKey);
                if (!isSaveButton && self.config.showConfirmBeforeSaving && !confirm(msg)) {
                    self.config.onCancel();
                    self.triggerChangeMode(ControlMode.view);
                    return;
                }

                var saveResult = self.config.onSave();
                if (saveResult != false) {
                    self.triggerChangeMode(ControlMode.view);
                }
            }

            self.cancel = function () {
                if (!self.isActiveEditMode) {
                    return;
                }

                var cancelResult = self.config.onCancel();
                if (cancelResult != false) {
                    self.triggerChangeMode(ControlMode.view);
                }
            }

            self.activateEditMode = function () {
                if (!self.isActiveViewMode) {
                    return;
                }

                self.triggerChangeMode(ControlMode.edit);
            }

            self.triggerChangeMode = function (mode) {
                self.config.mode = mode;
                updateControlState();
            }

            function updateControlState() {
                if (self.config) {
                    self.isActiveEditMode = self.config.mode == ControlMode.edit;
                    self.isActiveViewMode = self.config.mode == ControlMode.view;
                    self.isActiveDisadledMode = self.config.mode == ControlMode.disable;
                    self.isActiveCreateMode = self.config.mode == ControlMode.create;
                }
            }

            function initControl() {
                updateControlState();
            }

            initControl();
        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/EditControl/editControl.html',
            scope: {},
            transclude: true,
            replace: true,
            bindToController: {
                config: "="
            },
            controllerAs: 'editControlCtrl',
            controller: ['$scope', '$filter', controller]
        };
    }

    angular.module('boligdrift').directive('editControl', [editControl]);
})();