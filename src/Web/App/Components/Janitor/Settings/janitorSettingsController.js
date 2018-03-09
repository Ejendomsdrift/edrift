(function() {
    var janitorSettingsController = function (memberProvider, janitorService, $window) {
        var self = this;

        memberProvider.GetCurrentEmployee().then(function (result) {
            self.currentUser = result.data.memberModel;
            self.departments = result.data.departments;
            self.showDepartmentControl = self.departments.length > 1;
            self.aheadDays = janitorService.getDaysAheadList();
            self.aheadDay = self.currentUser.daysAhead
                                ? self.aheadDays.filter(function(d){return d.value == self.currentUser.daysAhead})[0]
                                : self.aheadDays[self.aheadDays[0]];
            self.department = self.currentUser.activeManagementDepartmentId
                                ? self.departments.filter(function (d) { return d.id == self.currentUser.activeManagementDepartmentId })[0]
                                : self.departments[0];
        });

        self.save = function () {
            var model = {
                memberId: self.currentUser.memberId,
                daysAhead: self.aheadDay.value,
                department: self.department.id
            };            

            memberProvider.UpdateEmployeeSettings(model).then(function() {
                $window.location.reload();
            });
        };
    }

    angular.module('boligdrift').controller('janitorSettingsController', ['memberProvider', 'janitorService', '$window', janitorSettingsController]);
})();