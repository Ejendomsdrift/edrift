(function () {
    var weekPlanController = function(
        $scope,
        $state,
        calendarProvider,
        departmentProvider,
        weekPlanProvider,
        weekPlanService,
        memberProvider,
        securityService,
        urlService,
        $rootScope) {

        var self = this;
        self.weekendTaskCount = 0;
        self.state = State;

        self.init = function() {
            checkSecurityPermission();

            self.filter = weekPlanService.getFilter();
            self.isShowWeekend = self.filter.isShowWeekend;

            self.yearWeekSelectorConfig = {
                isShowYear: true,
                isShowWeek: true,
                selectedYear: self.filter && self.filter.year ? self.filter.year : null,
                selectedWeek: self.filter && self.filter.week ? self.filter.week : null,
                onChange: self.changeWeek,
                disabled: !self.selectedDepartment
            }

            departmentProvider.getDepartments().then(getDepartmentsCallback);
            initDepartmentPicker();
            initMemberPicker();
        }

        function checkSecurityPermission(){
            securityService.hasAccessByKeyList({ keyList: [ControlSecurityKey.WeekPlanPage] }).then(function (result) {
                if (!result.data[ControlSecurityKey.WeekPlanPage]) {
                    urlService.defaultRedirect();
                }
            });
        }

        function initDepartmentPicker() {
            self.departmentPickerConfig = new DepartmentPickerModel();
            self.departmentPickerConfig.onSelect = self.changeDepartment;
            self.departmentPickerConfig.showAll = !self.isGridSubPage();
        }

        function getDepartmentsCallback(result) {
            self.departments = result.data;
            self.selectedDepartment = weekPlanService.getSelectedDepartment(result.data, self.filter);
        }

        self.changeWeek = function (year, week) {
            self.selectedYear = year;
            self.selectedWeek = week;
            weekPlanService.storeFilter(self.selectedYear, self.selectedWeek, self.selectedDepartment);

            informFilterChanged();
        }

        self.changeDepartment = function (selectedDepartment) {
            self.selectedDepartment = selectedDepartment;
            weekPlanService.storeFilter(self.selectedYear, self.selectedWeek, self.selectedDepartment);
            clearDepartmentFilterIfSelectedEmpty();
            informFilterChanged();
            self.yearWeekSelectorConfig.disabled = !self.selectedDepartment;
        }

        self.isListSubPage = function () {
            return $rootScope.$state.current.name.indexOf(State.WeekPlanListView) > -1;
        };

        self.isGridSubPage = function () {
            return $rootScope.$state.current.name === State.WeekPlanGridView;
        };

        self.isTimeSubPage = function () {
            return $rootScope.$state.current.name === State.WeekPlanTimeView;
        };

        self.weekWithWeekend = function () {
            self.isShowWeekend = !self.isShowWeekend;
            if (self.isListSubPage()) {
                self.isShowWeekend = false;
            }
            weekPlanService.storeFilter(self.selectedYear, self.selectedWeek, self.selectedDepartment, self.isShowWeekend);
        };

        self.redirectToGridView = function () {
            if (!self.isGridSubPage()) {
                if (self.selectedDepartment && !self.selectedDepartment.id) {
                    self.selectedDepartment = null;
                }
                self.departmentPickerConfig.showAll = false;
                loadMembers(State.WeekPlanGridView);
                $state.go(State.WeekPlanGridView);
            }
        }

        self.redirectToListView = function () {
            if (!self.isListSubPage()) {
                self.departmentPickerConfig.showAll = true;
                loadMembers(State.WeekPlanListView);
                $state.go(State.WeekPlanListView);
            }
        }

        self.redirectToTimeSubPage = function() {
            if (!self.isTimeSubPage()) {
                loadMembers(State.WeekPlanTimeView);
                $state.go(State.WeekPlanTimeView);
            }
        }

        self.createOperationalTask = function () {
            $state.go(State.OperationalTaskCreateAdHoc, { redirectState: $state.current.name, jobType: JobType.AdHoc, departmentId: self.selectedDepartment.id });
        }

        self.scrollToTop = function () {
            $("body, html").animate({
                scrollTop: 0
            }, 400);
        }

        angular.element(window).on("scroll", function (e) {
            $scope.$apply(function () {
                self.isVisibleButton = window.pageYOffset > 300;
            });
        });

        function clearDepartmentFilterIfSelectedEmpty() {
            if (!self.selectedDepartment) {
                weekPlanService.clearDepartmentFilter();
            }
        }

        function informFilterChanged() {
            if (self.selectedWeek && self.selectedDepartment) {
                refreshMembers($state.current.name);
            }
        }

        function refreshMembers(state) {
            if (state === State.WeekPlanTimeView) {
                memberProvider.GetMembers().then(function (result) {
                    membersLoaded(result);
                    broadcastEvent();
                });
            }
            else {
                memberProvider.GetAllowedMembersForJob().then(function (result) {
                    membersLoaded(result);
                    broadcastEvent();
                });
            }
        }

        function broadcastEvent() {
            $scope.$broadcast('weekPlanFilterChanged', {
                selectedWeek: self.selectedWeek,
                selectedYear: self.selectedYear,
                selectedDepartment: self.selectedDepartment,
                selectedMemberIds: self.memberPickerConfig.selectedIds
            });
        }

        function loadMembers(state) {
            if (state === State.WeekPlanTimeView) {
                memberProvider.GetMembers().then(membersLoaded);
            }
            else {
                memberProvider.GetAllowedMembersForJob().then(membersLoaded);
            }
        }

        function membersLoaded(result){
            self.memberPickerConfig.members = result.data;
            self.memberPickerConfig.selectedIds = weekPlanService.getMemberFilter();
            self.memberPickerConfig.triggerRefresh();
        }

        function memberFilterChanged() {
            weekPlanService.storeMemberFilter(self.memberPickerConfig.selectedIds);
            $scope.$broadcast('weekPlanMemberFilterChanged', {
                selectedMemberIds: self.memberPickerConfig.selectedIds
            });
        }

        function initMemberPicker() {
            self.memberPickerConfig = new MemberPickerModel(true, ControlMode.create);
            self.memberPickerConfig.onSelect = memberFilterChanged;
            self.memberPickerConfig.onRemove = memberFilterChanged;
            self.memberPickerConfig.members = [];
            loadMembers($state.current.name);
        }

        var isShowWeekendChanged = $scope.$on('isShowWeekendChanged', function (event, data) {
            self.isShowWeekend = data.isShowWeekend;
        });

        $scope.$on('$destroy', isShowWeekendChanged);

        var weekEndJobCountInformationEvent = $scope.$on('weekEndJobCountInformationEvent', function (event, data) {
            self.weekendJobCount = data.weekendJobCount;
        });

        $scope.$on('$destroy', weekEndJobCountInformationEvent);
    }

    angular.module("boligdrift").controller('weekPlanController',
        [
            '$scope',
            '$state',
            'calendarProvider',
            'departmentProvider',
            'weekPlanProvider',
            'weekPlanService',
            'memberProvider',
            'securityService',
            'urlService',
            '$rootScope',
            weekPlanController]);
})();