(function () {
    var teamPicker = function ($filter, groupProvider) {
        var controller = function () {
            var self = this;
            self.states = TeamPickerState;

            self.config.triggerRefresh = function () {
                if (!self.config.isOpenedState && self.config.teamLeadId) {
                    self.config.isOpenedState = true;
                }

                self.isActiveWaitIcon = true;

                copySavedData();
                fillGroups();
                fillMode();
                refreshState();
            }

            self.config.triggerCopySavedData = function () {
                copySavedData();
            }

            self.config.triggerValidate = function () {
                self.includeValidation = self.config.isRequired;

                if (self.config.selectedGroup && self.state == self.states.openToAll) {
                    return true;
                }

                validateControl();

                return self.isGroupIdValid && self.isUserIdListValid && self.isTeamLeadIdValid;
            }

            self.config.triggerCheckControlChanged = function () {
                return checkUserIdListChanged() ||
                       checkGroupChanged() ||
                       self.config.teamLeadId != self.savedData.teamLeadId ||                       
                       self.config.isAssignedToAllUsers != self.savedData.isAssignedToAllUsers;
            }

            self.selectGroup = function (group) {
                if (self.editControlConfig.mode !== ControlMode.edit && self.editControlConfig.mode !== ControlMode.create) {
                    self.editControlConfig.mode = ControlMode.edit;
                }
                clearSelectedMembers();

                self.state = self.states.openToAll;

                if (group) {
                    self.config.isAssignedToAllUsers = true;
                    self.config.isOpenedState = true;
                    self.config.groupId = group.id;
                    self.config.selectedGroup = getGroup();
                    fillMembers();
                    self.isGroupIdValid = true;
                    self.isTeamPickerActive = true;
                    self.config.isUnassignAll = false;
                } else {
                    self.memberPickerConfig.members = [];
                    self.leadPickerConfig.members = [];
                    self.config.isAssignedToAllUsers = false;
                    self.config.groupId = undefined;
                    self.config.selectedGroup = null;
                    self.isGroupIdValid = false;
                    self.isTeamPickerActive = false;
                    self.config.isUnassignAll = true;
                }

                self.config.onChange(self.isTeamPickerActive);
            }

            self.changeState = function () {
                self.config.isAssignedToAllUsers = self.state != self.states.assignedToSelected;

                if (self.state == self.states.openToAll) {
                    self.config.userIdList = [];
                    self.memberPickerConfig.members = [];
                    self.memberPickerConfig.triggerClear();

                    self.config.teamLeadId = null;
                    self.leadPickerConfig.members = [];
                    self.leadPickerConfig.triggerClear();
                }
                else if (self.state == self.states.assignedToAll) {
                    self.config.userIdList = [];
                    self.memberPickerConfig.triggerClear();
                }

                fillMembers();
                self.isUserIdListValid = true;
                self.isTeamLeadIdValid = true;
                self.isTeamPickerActive = true;
                self.config.onChange(self.isTeamPickerActive);
            }

            self.isValidMemberSelector = function () {
                return (!self.includeValidation || self.isUserIdListValid) &&
                       (!self.teamLeadRequired || self.config.isAssignedToAllUsers || self.memberPickerConfig.selectedIds.length);
            }

            self.isValidTeamLeadSelector = function () {
                return self.isTeamLeadIdValid || (!self.includeValidation && !self.teamLeadRequired);
            }

            function membersChanged() {
                self.config.userIdList = self.memberPickerConfig.selectedIds;
                self.leadPickerConfig.members = self.memberPickerConfig.selectedMembers;
                self.config.teamLeadId = getTeamLeadId();
                self.leadPickerConfig.selectedId = self.config.teamLeadId;
                self.leadPickerConfig.triggerRefresh();
            }

            function clearSelectedMembers() {
                self.config.userIdList = [];
                self.memberPickerConfig.triggerClear();
                self.config.teamLeadId = null;
                self.leadPickerConfig.triggerClear();
            }

            function selectMember(member) {
                self.isUserIdListValid = self.memberPickerConfig.selectedIds.length ? true : false;
                self.isTeamLeadIdValid = self.config.teamLeadId ? true : false;
                membersChanged();
                self.isTeamPickerActive = true;
                self.config.onChange(self.isTeamPickerActive);
            }

            function removeMember(member) {
                self.isUserIdListValid = true;
                membersChanged();
                self.config.onChange();
            }

            function selectTeamLead(member) {
                self.config.teamLeadId = member.memberId;
                self.isTeamLeadIdValid = true;
                self.isTeamPickerActive = true;
                self.config.onChange(self.isTeamPickerActive);
            }

            function removeTeamLead() {
                if (self.editControlConfig.mode !== ControlMode.edit && self.editControlConfig.mode !== ControlMode.create) {
                    self.editControlConfig.mode = ControlMode.edit;
                }

                self.config.teamLeadId = null;
                self.isTeamLeadIdValid = false;
                self.config.onChange();
            }

            function getTeamLeadId() {
                if (self.state == self.states.openToAll) {
                    return null;
                }

                if (self.leadPickerConfig.members.length == 1) {
                    return self.leadPickerConfig.members[0].memberId;
                }

                var memberIds = self.leadPickerConfig.members.map(function (item) { return item.memberId; });
                return memberIds.indexOf(self.config.teamLeadId) > -1 ? self.config.teamLeadId : null;
            }

            function fillMode() {
                self.editControlConfig.mode = self.config.mode;
            }

            function copySavedData() {
                self.savedData = {
                    groupId: self.config.groupId,
                    teamLeadId: self.config.teamLeadId,
                    userIdList: self.config.userIdList.slice(),
                    isAssignedToAllUsers: self.config.isAssignedToAllUsers,
                    selectedGroup: self.config.selectedGroup
                };
            }

            function validateControl() {
                self.isGroupIdValid = self.config.selectedGroup ? true : false;
                self.isUserIdListValid = self.config.userIdList.length > 0 || self.config.isAssignedToAllUsers ? true : false;
                self.isTeamLeadIdValid = self.config.teamLeadId ? true : false;
            }

            function checkUserIdListChanged() {
                return self.config.userIdList.length != self.savedData.userIdList.length ||
                       self.config.userIdList.filter(function (i) { return self.savedData.userIdList.indexOf(i) < 0 }).length > 0;
            }

            function fillGroups() {
                groupProvider.GetGroupedMembers(self.config.timeViewDayScope).then(function (result) {
                    self.groups = result.data;
                    result.data[result.data.length - 1].name = $filter('translate')('See all employees without groups');
                    self.config.selectedGroup = getGroup();
                    
                    fillMembers();
                });
            }

            function fillMembers() {
                if (!self.config.selectedGroup) {
                    self.isActiveWaitIcon = false;
                    return;
                }

                self.memberPickerConfig.members = self.config.selectedGroup.members;
                self.memberPickerConfig.selectedIds = self.config.userIdList;
                self.memberPickerConfig.showTimeView = true;
                self.memberPickerConfig.triggerRefresh();

                self.leadPickerConfig.members = getMembersForLeadPicker();
                self.config.teamLeadId = getTeamLeadId();
                self.leadPickerConfig.selectedId = self.config.teamLeadId;
                self.leadPickerConfig.triggerRefresh();

                validateControl();

                if (self.isActiveWaitIcon) {
                    self.isActiveWaitIcon = false;
                }
            }

            function getMembersForLeadPicker() {
                if (self.config.isAssignedToAllUsers) {
                    return self.config.selectedGroup.members;
                }

                var members = self.config.selectedGroup.members.filter(function (member) {
                    return self.memberPickerConfig.selectedIds.indexOf(member.memberId) > -1;
                });

                return members;
            }

            function saveHandler() {
                var isValidControl = self.config.triggerValidate();
                if (self.config.isRequired && !isValidControl) {
                    return false;
                }
                else if (self.state != self.states.openToAll && !self.config.isRequired &&
                        !self.config.teamLeadId && !angular.isUndefined(self.config.groupId)) {
                    self.teamLeadRequired = true;
                    return false;
                }

                if (!self.config.triggerCheckControlChanged()) {
                    return true;
                }

                var model = {
                    groupId: self.config.groupId,
                    userIdList: self.config.userIdList,
                    teamLeadId: self.config.teamLeadId,
                    isAssignedToAllUsers: self.config.isAssignedToAllUsers,
                    isUnassignedAll: self.config.isUnassignAll
                };

                self.config.onSave(model);
                self.config.triggerCopySavedData();
                return true;
            }

            function cancelHandler() {
                self.config.groupId = self.savedData.groupId;
                self.config.teamLeadId = self.savedData.teamLeadId;
                self.config.userIdList = self.savedData.userIdList;
                self.config.isAssignedToAllUsers = self.savedData.isAssignedToAllUsers;
                self.config.selectedGroup = getGroup();
                fillMembers();

                self.includeValidation = false;
                self.teamLeadRequired = false;
                self.config.onChange();

                refreshState();
            }

            function getGroup() {
                if (self.config.groupId || (self.config.groupId == null && self.config.isOpenedState)) {
                    return self.groups.find(function (group) { return group.id === self.config.groupId });
                }
                return null;
            }

            function refreshState() {
                if (!self.config.userIdList.length && !self.config.teamLeadId) {
                    self.state = self.states.openToAll;
                }
                else if (!self.config.isAssignedToAllUsers) {
                    self.state = self.states.assignedToSelected;
                }
                else {
                    self.state = self.states.assignedToAll;
                }
            }
            
            function checkGroupChanged() {
                return self.config.groupId != self.savedData.groupId || (!self.config.groupId && self.config.selectedGroup);
            }

            function initControl() {
                self.memberPickerConfig = new MemberPickerModel(true, ControlMode.create);
                self.memberPickerConfig.onSelect = selectMember;
                self.memberPickerConfig.onRemove = removeMember;

                self.leadPickerConfig = new MemberPickerModel(false, ControlMode.create);
                self.leadPickerConfig.defaultTextKey = $filter('translate')('Choose a team lead');
                self.leadPickerConfig.onSelect = selectTeamLead;
                self.leadPickerConfig.onRemove = removeTeamLead;

                self.editControlConfig = new EditControlModel();
                self.editControlConfig.mode = self.config.mode;
                self.editControlConfig.onSave = saveHandler;
                self.editControlConfig.onCancel = cancelHandler;

                self.isActiveWaitIcon = false;

                copySavedData();
                fillGroups();
                refreshState();
            }

            initControl();
        };

        return {
            restrict: 'E',
            templateUrl: '/App/Controls/TeamPicker/teamPicker.html',
            replace: true,
            scope: {},
            bindToController: {
                config: "="
            },
            controllerAs: 'teamPickerCtrl',
            controller: [controller]
        };
    }

    angular.module('boligdrift').directive('teamPicker', ['$filter', 'groupProvider', teamPicker]);
})();