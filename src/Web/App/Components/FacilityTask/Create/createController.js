(function () {
    var createController = function ($state, jobProvider, memberProvider, $filter, yearPlanService, securityService, urlService) {
        var self = this;
        self.maxTitleLength = 400;

        self.categoryTreeConfig = {
            simpleView: false
        };

        self.create = function () {
            if (validateRequiredFieldsForTaskCreation()) {
                jobProvider.Create(self.selectedCategory.id, self.taskTitle, self.filter.departmentid).then(function (result) {
                    $state.go(State.FacilityTaskAssign, { id: result.data.job.id });
                });
            }
        };

        self.assign = function () {
            if (validateRequiredFieldsForTaskAssignement()) {
                var ids = self.assignDepartments.map(function(v) { return v.id });
                var model = {
                    jobId: self.selectedTask.id,
                    assignedDepartmentIds: ids,
                    unassignedDepartmentIds: []
                };

                jobProvider.Assign(model).then(function() {
                    $state.go(State.FacilityTaskInterval, { id: self.selectedTask.id });
                });
            }
        };

        self.userMadeChanges = function() {
            controlHasChanges();
        }

        self.categoryTreeSelected = function (task, category) {
            controlHasChanges();
            var isItemSelected = false;
            if (category && !category.children.length) { //task can be created only under the lowest level category
                self.selectedCategory = category;
                self.selectedId = category.id;
                isItemSelected = true;
                self.selectedTask = null;
            }

            if (task) {
                self.isValidationRequiredForCreation = false;
                self.selectedTask = task;
                self.selectedId = task.id;
                isItemSelected = true;
            } else {
                self.isValidationRequiredForAssignement = false;
            }

            return isItemSelected;
        };

        self.isTaskSelected = function () {
            if (!self.isValidationRequiredForAssignement) {
                return true;
            }

            return self.selectedTask != undefined;
        }

        self.isAssignedDepartmentValid = function () {
            if (!self.isValidationRequiredForAssignement) {
                return true;
            }

            return self.assignDepartments && self.assignDepartments.length;
        }

        self.isCategoryValid = function () {
            if (!self.isValidationRequiredForCreation) {
                return true;
            }

            return self.selectedCategory != undefined;
        }

        self.isTitleValid = function () {
            if (!self.isValidationRequiredForCreation) {
                return true;
            }

            return self.taskTitle && self.taskTitle.length <= self.maxTitleLength;
        }

        self.isTitleLengthExceeded = function () {
            return self.taskTitle && self.taskTitle.length > self.maxTitleLength;
        }

        self.resetValidation = function() {
            self.isValidationRequiredForCreation = false;
            self.isValidationRequiredForAssignement = false;
        }

        self.redirect = function () {
            if (confirmRedirect()) {
                $state.go(self.state.YearPlanTaskOverview);   
            }            
        }

        function confirmRedirect() {
            var changedElements = document.querySelectorAll('.js-user-made-changes');
            if (changedElements.length) {
                var msg = $filter('translate')('Are you sure?');
                if (!self.taskId && !confirm(msg)) {
                    return false;
                }
            }

            return true;
        }

        function controlHasChanges() {
            self.rootElement.classList.add('js-user-made-changes');
        }

        function validateRequiredFieldsForTaskCreation() {
            self.isValidationRequiredForCreation = true;
            return self.isCategoryValid() & self.isTitleValid();
        }

        function validateRequiredFieldsForTaskAssignement() {
            self.isValidationRequiredForAssignement = true;
            return self.isAssignedDepartmentValid() & self.isTaskSelected();
        }

        function initControl() {

            checkSecurityPermission();

            self.state = State;
            self.filter = yearPlanService.getFilter();
            self.isValidationRequiredForCreation = false;
            self.isValidationRequiredForAssignement = false;
            memberProvider.GetCurrentUserContext().then(function (result) {
                self.currentUserContext = result.data;
                self.rootElement = document.getElementsByClassName('create-popup')[0];
            });
        }

        function checkSecurityPermission() {
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.FacilityTaskCreatePage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.FacilityTaskCreatePage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        initControl();

    };

    angular.module('boligdrift').controller('createController', ['$state', 'jobProvider', 'memberProvider', '$filter', 'yearPlanService', 'securityService', 'urlService', createController]);
})();