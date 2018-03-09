(function() {
    var selectControl = function () {

        var controller = function ($document, $scope, $location, $anchorScroll, $filter) {
            var self = this;
            self.counter = -1;
            self.showAllSelectItem = { value: $filter('translate')(self.config.showAllTranslationKey), id: '', obj: {} };


            self.isEmpty = function () {
                return self.selected === null || self.selected === undefined || self.selected.value == null;
            }

            self.config.triggerSetValue = function() {
                self.selected = self.config.selected;
            }

            self.config.triggerUpdateDataList = function() {
                initControl();
            }

            self.onUpdateHandler = function (item, event) {
                if (item.obj && item.obj.isDisabled) {
                    event.stopPropagation();
                    return;
                }

                self.counter = -1;
                self.dataList = angular.copy(self.config.dataList);
                self.selected = angular.copy(item);
                self.config.selected = angular.copy(item);
                self.config.onUpdate(self.selected);
                self.isOpened = false;
            }

            self.onRemoveHandler = function () {
                self.counter = -1;
                self.dataList = angular.copy(self.config.dataList);
                self.selected = null;
                self.config.selected = undefined;
                self.config.onUpdate(self.selected);              
            }

            self.openSelect = function ($event) {
                self.counter = -1;
                $event.stopPropagation();
                self.dataList = angular.copy(self.config.dataList);
                self.isOpened = true;
            }

            self.searchHandler = function () {
                self.counter = -1;
                self.dataList = _.filter(self.storedDataList, function (item) {
                    return item.value.toLowerCase().indexOf(self.searchValue.toLowerCase()) > -1;
                });
            }

            self.handleEnterClick = function (event) {
                if (event.which === 13) {
                    self.selected = self.dataList && self.dataList.length > 0 ? angular.copy(self.dataList[0]) : null;
                    self.searchValue = null;
                    self.isOpened = false;
                    self.config.onUpdate(self.selected);
                }                    
            }

            self.keyDownHandler = function ($event) {
                if ($event.keyCode === 38) {                    
                    if (self.counter > 0) {
                        self.counter--;
                    } else {
                        self.counter = 0;
                    }                    
                }                    
                if ($event.keyCode === 40) {                    
                    if (self.counter < self.storedDataList.length - 1) {
                        self.counter++;   
                    } else {
                        self.counter = self.storedDataList.length - 1;
                    }
                }
                setScroll();
            }

            self.isItemDisabled = function (item) {
                if (item) {
                    return item.isDisabled;
                }
            }

            function setScroll() {                                
                var newHash = 'list-item-' + self.counter;
                if ($location.hash() !== newHash)
                {
                    $location.hash(newHash);
                } else {
                    $anchorScroll();
                }
            }

            function documentClick() {
                $scope.$apply(function() {
                    self.isOpened = false;
                    self.searchValue = null;
                });                
            }

            $document.on('click', documentClick);

            $scope.$on('$destroy', function () {                
                $document.off('click', documentClick);
            });

            function initControl() {
                self.isOpened = false;
                self.selected = self.config.selected;
                self.dataList = angular.copy(self.config.dataList);
                self.storedDataList = angular.copy(self.config.dataList);
                self.placeholder = self.config.placeholder;
            }

            initControl();

        }

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/SelectControl/selectControl.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'selectCtrl',
            controller: ['$document', '$scope', '$location', '$anchorScroll', '$filter', controller]
        };
    }
    angular.module('boligdrift').directive('selectControl', [selectControl]);
})();