(function () {
    var memberPicker = function () {
        var controller = function ($filter, dateHelper) {
            var self = this;

            self.onSelect = function (member) {
                if (!member && !self.config.isMultiple) {
                    self.config.triggerClear();
                    self.config.onRemove();
                    return;
                }

                var index = self.config.selectedIds.indexOf(member.memberId);
                if (self.config.isMultiple && index == -1) {
                    self.config.selectedIds.push(member.memberId);
                    var orderedMembers = $filter('orderBy')(self.config.selectedMembers, 'name');
                    self.config.selectedMembers = orderedMembers;
                }
                else {
                    self.config.selectedIds = [member.memberId];
                    self.config.selectedId = member.memberId;
                    self.userAvatarModel.triggerChangeUrl(member.avatar);
                }
                self.config.onSelect(member);
            }

            self.onRemove = function (member) {
                if (self.config.isCanRemove) {
                    self.config.isCanRemove(member).then(function (result) {
                        if (result.data) {
                            removeMember(member);
                        }
                        else {
                            fillSelectedMembers();
                            alert(self.config.removeMessage);
                        }
                    });
                }
                else {
                    removeMember(member);
                }
            }

            self.config.triggerClear = function () {
                self.config.selectedIds = [];
                self.config.selectedMembers = [];
                self.config.selectedId = null;
                self.config.selectedMember = null;
                if (!self.config.isMultiple) {
                    self.userAvatarModel.triggerChangeUrl();
                }
            }

            self.config.triggerRefresh = function () {
                copyData();
                fillSelectedMembers();
                self.userAvatarModel.triggerChangeUrl(getMemberAvatarUrl());
            }

            self.getMinutesToViewFormat = function (minutes, forceMinutesDisplay) {
                return dateHelper.minutesToViewFormat(minutes, forceMinutesDisplay);
            }

            function removeMember(member) {
                var index = self.config.selectedIds.indexOf(member.memberId);
                if (index > -1) {
                    self.config.selectedIds.splice(index, 1);
                }
                self.config.selectedId = angular.noop;
                self.config.onRemove(member);
            }

            function fillSelectedMembers() {
                if (!self.config.selectedIds) {
                    return;
                }

                if (self.config.isMultiple) {
                    self.config.selectedMembers = self.config.members.filter(function (member) {
                        return self.config.selectedIds.indexOf(member.memberId) > -1;
                    });
                }
                else {
                    self.config.selectedMember = self.config.members.find(function (member) {
                        return self.config.selectedId == member.memberId;
                    });

                    if (self.config.selectedMember) {
                        self.config.selectedId = null;
                    }
                }
            }

            function getMemberAvatarUrl() {
                return self.config.selectedMember ? self.config.selectedMember.avatar : "";
            }

            function copyData() {
                self.savedData = angular.copy(self.config.selectedIds);
            }

            function saveHandler() {
                self.config.onSave(self.config.selectedIds);
            }

            function cancelHandler() {
                self.config.selectedIds = angular.copy(self.savedData);
                self.config.triggerRefresh();
            }

            function initControl() {
                fillSelectedMembers();

                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.userAvatarModel = new UserAvatarModel();
                self.userAvatarModel.url = getMemberAvatarUrl();
                self.userAvatarModel.name = self.config.selectedMember ? self.config.selectedMember.name : "";
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/MemberPicker/memberPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: '='
            },
            controllerAs: 'memberPickerCtrl',
            controller: ['$filter', 'dateHelper', controller]
        };
    }

    angular.module('boligdrift').directive('memberPicker', [memberPicker]);
})();