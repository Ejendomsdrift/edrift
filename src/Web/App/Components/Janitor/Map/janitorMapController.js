(function() {
    var janitorMapController = function (jobProvider) {
        var self = this;

        function initControl() {
            jobProvider.GetMyJobsHeaderList().then(function (result) {
                self.jobs = result.data;
            });
        }

        initControl();
    }

    angular.module('boligdrift').controller('janitorMapController', ['jobProvider', janitorMapController]);
})();