(function () {
    var categoryTree = function ($document, categoryProvider, $timeout, yearPlanService, treeHelper) {
        var controller = function ($scope) {
            var self = this;
            self.treeOptions = {
                nodeChildren: "children",
                dirSelectable: true,
                injectClasses: {
                    ul: "custom-tree",
                    iCollapsed: "fa fa-plus",
                    iExpanded: "fa fa-minus"
                }
            };
            var defaults = {
                dropdownView: false,
                showTaskCount: false,
                showTasks: function () { return false; },
                selectOnlyTasks: function () { return false; },
                simpleView: false,
                showViewAllButton: function () { return false; },
                isIncludeGroupedTasks: false,
                isIncludeHiddenTasks: false
            };
            self.config = angular.extend(defaults, self.config);
            self.isTreeDropdownExpanded = !self.config.dropdownView;

            var showTasks = self.config.showTasks();
            categoryProvider.getTree(self.config.isIncludeGroupedTasks, self.config.isIncludeHiddenTasks, showTasks).then(function (result) {
                self.categoryTreeData = result.data;
                initListeners(result.data);
                if (typeof self.config.onLoaded === 'function') {
                    self.config.onLoaded(result.data);
                }
            });

            self.toggleTreeDropdown = function (e) {
                e && e.stopPropagation();
                if (self.config.dropdownView) {
                    self.isTreeDropdownExpanded = !self.isTreeDropdownExpanded;
                }
            };

            self.getClassForControl = function () {
                return {
                    'show-tasks-count': self.config.showTaskCount,
                    'show-tasks-lines': self.config.showTasks(),
                    'show-dropdown': self.config.dropdownView,
                    'show-simple-view': self.config.simpleView,
                    'show-edit-view': !self.config.simpleView,
                    'show-dropdown-expanded': self.isTreeDropdownExpanded,
                    'show-view-all-button': self.config.showViewAllButton()
                };
            };

            self.taskOnClick = function ($event, task) {
                self.selectedCategory = null;
                self.selectedTask = task;
                self.toggleTreeDropdown($event);
                task.isSelectedTask = !task.isSelectedTask;
                if (self.previousSelectedTask) {
                    self.previousSelectedTask.isSelectedTask = false;
                }
                self.previousSelectedTask = task;
                self.onSelect({ $task: task });
            };

            self.filterOperationalJobs = function (node) {
                return node.jobTypeId === JobType.Facility;
            }

            self.categoryOnClick = function (category) {
                if (self.selectedTask) {
                    self.selectedTask.isSelectedTask = false;
                    self.selectedTask = null;
                }

                if (self.config.selectOnlyTasks()) {
                    expandCollapseCategory(category);
                    return;
                }
                self.selectedCategory = category;
                var isItemSelected = self.onSelect({ $category: category });
                if (isItemSelected) {
                    self.toggleTreeDropdown();
                } else {
                    expandCollapseCategory(category);
                }
            };

            self.seeAllOnClick = function () {
                self.selectedCategory = null;
                self.expandedCategories = [];
                self.onSelect({ $task: null, $category: null, $isCleared: true });
            }

            self.clear = function () {
                self.selectedId = null;
                self.selectedTask = null;
                self.toggleTreeDropdown();

                if (self.selectedCategory) {
                    self.seeAllOnClick();
                    self.selectedCategory = null;
                    yearPlanService.clearCategoryFilter();
                }

                if (!self.selectedTask) {
                    self.expandedCategories = [];
                    self.onSelect({ $task: null });
                }
            }

            self.hasClearButton = function () {
                return self.selectedCategory && !self.config.showTasks() ||
                    self.selectedTask && self.config.showTasks();
            }

            self.config.getSelectedCategories = function () {
                var result = [];

                if (self.selectedCategory) {
                    result = getLeafs(self.selectedCategory);
                }

                return result;
            }

            self.config.findSelectedNode = function (roots, categoryId) {
                if (!roots) {
                    return null;
                }
                return findSelectedNode(roots, categoryId);
            }

            function getLeafs(category) {
                var result = treeHelper.getTreeMapResult(category,
                    function (node) { return node.children; },
                    function (node) { return [node]; },
                    function (node, mappedChilds) {
                        var result = mappedChilds.reduce(function (a, b) { return a.concat(b); });
                        return result;
                    },
                    function (node) { return node.children.length > 0; }
                );
                return result;
            }

            function expandCollapseCategory(category) {
                var index = self.expandedCategories.indexOf(category);
                if (index >= 0) {
                    self.expandedCategories.splice(index);
                } else {
                    self.expandedCategories.push(category);
                }
            }

            function findSelectedNode(roots, categoryId) {
                for (var i = 0; i < roots.length; i++) {
                    var category = searchTree(roots[i], categoryId);
                    if (category) return category;
                }
                return null;
            };

            function searchTree(element, id) {
                if (element.id === id) {
                    return element;
                } else if (element.children) {
                    var result = null;
                    for (var i = 0; result == null && i < element.children.length; i++) {
                        var children = element.children[i];
                        children.parent = element;
                        result = searchTree(element.children[i], id);
                    }
                    return result;
                }
                return null;
            };

            function setPathToActive(node) {
                $timeout(function () {
                    self.pathToActive = [];
                    do {
                        self.pathToActive.unshift(node);
                        node = node.parent;
                    } while (node);

                    if (!self.expandedCategories || !self.expandedCategories.length) {
                        self.expandedCategories = self.pathToActive;
                    }
                }, 0);
            };

            function initListeners(tree) {
                var selectedIdListener = $scope.$watch(function () {
                    return self.selectedId;
                }, function (newValue) {
                    if (newValue) {
                        self.selectedCategory = findSelectedNode(tree, newValue);
                        if (self.selectedCategory) {
                            setPathToActive(self.selectedCategory);
                        }
                    }
                }, true);

                var documentOnClick = function () {
                    if (self.config.dropdownView && self.isTreeDropdownExpanded) {
                        $scope.$apply(function () {
                            self.isTreeDropdownExpanded = false;
                        });
                    }
                };

                $document.on('click', documentOnClick);

                $scope.$on('$destroy', function () {
                    $document.off('click', documentOnClick);
                    selectedIdListener();
                });

                function init() {
                    self.filter = yearPlanService.getFilter();
                }

                init();
            };
        };

        return {
            restrict: 'E',
            transclude: true,
            replace: true,
            templateUrl: '/app/controls/categoryTree/categoryTree.html',
            scope: {},
            controllerAs: 'categoryTreeCtrl',
            bindToController: {
                config: '=?',
                onSelect: '&',
                selectedId: '='
            },
            controller: ['$scope', controller]
        };
    };

    angular.module('boligdrift').directive('categoryTree', ['$document', 'categoryProvider', '$timeout', 'yearPlanService', 'treeHelper', categoryTree]);
})();