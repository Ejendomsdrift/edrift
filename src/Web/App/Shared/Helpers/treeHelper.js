(function () {
    var treeHelper = function () {
        var self = this;
        self.getTreeMapResult = function (tree, childrenSelectorFunc, leafFunc, nodeFunc, isNodeFunc) {
            function treeMapperRec(node) {
                if (isNodeFunc(node)) {
                    var mappedChilds = _.map(childrenSelectorFunc(node), treeMapperRec);
                    return nodeFunc(node, mappedChilds);
                } else {
                    return leafFunc(node);
                }
            }

            return treeMapperRec(tree);
        }

        self.getSelectedCategoriesList = function (selectedCategory) {
            if (selectedCategory) {
                var result = self.getTreeMapResult(selectedCategory,
                    function (node) { return node.children; },
                    function (node) { return [node]; },
                    function (node, mappedChilds) {
                        mappedChilds.push([node]);
                        var result = mappedChilds.reduce(function (a, b) { return a.concat(b); });
                        return result;
                    },
                    function (node) { return node.children.length > 0; }
                );
                return result;
            } else {
                return [];
            }
        }

    };
    angular.module('boligdrift').service('treeHelper', [treeHelper]);
})();