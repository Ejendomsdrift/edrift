(function () {
    var cancellingReason = function() {
        var controller = function() {
            var self = this;

            self.create = function() {
                var model = {
                    text: self.newTemplateText,
                    jobTypeList: self.selectedTaskTypeList,
                    isCoordinatorReason: self.config.isCoordinatorReason
            };

                self.config.onSave(model);
            }

            self.delete = function (template) {
                self.config.onDelete(template);
            }

            self.config.triggerCleanTemplateData = function() {
                cleanData();
            }

            self.onSelectTaskType = function(typeId) {
                self.selectedTaskTypeList.push(typeId);
            }

            self.onRemoveTaskType = function(typeId) {
                var index = self.selectedTaskTypeList.indexOf(typeId);
                self.selectedTaskTypeList.splice(index, 1);
            }

            function cleanData() {
                self.cancellingTemplates = [];
                self.selectedTaskTypeList = [];
                self.selectedTaskType = [];
                self.newTemplateText = "";
            }

            function getTaskTypeList() {
                var result = [];
                for (var name in JobType) {
                    if (JobType.hasOwnProperty(name)) {
                        result.push({ id: JobType[name], name: name });
                    }
                }

                return result;
            }

            function initControl() {
                cleanData();
                self.taskTypeList = getTaskTypeList();
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/CancellingReason/cancellingReason.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'cancellingReasonCtrl',
            controller: [controller]
        }
    }

    angular.module('boligdrift').directive('cancellingReason', [cancellingReason]);
})();