(function () {
    var groupGeneralController = function ($stateParams, $filter, groupProvider) {
        var self = this;
        self.isUnique = true;

        var loadGroup = function () {
            groupProvider.GetGroup($stateParams.id).then(function (result) {
                self.group = result.data;
                self.titlePickerConfig.value = result.data.name;
                self.titlePickerConfig.triggerRefresh();

                self.memberPickerConfig.members = result.data.allowedMembers;
                self.memberPickerConfig.selectedIds = self.group.memberIds;
                self.memberPickerConfig.triggerRefresh();
            });
        }

        function changeMembers(memberIdList) {
            var model = { groupId: self.group.id, memberIds: memberIdList };
            groupProvider.AssignMembers(model);
        }

        function isCanRemoveMember(member) {
            return groupProvider.isCanRemoveMember(self.group.id, member.memberId);
        }

        function changeName(name) {
            groupProvider.IsUniqueName(name).then(function (result) {
                self.isUnique = result.data;
                if (self.isUnique) {
                    var model = { groupId: self.group.id, value: name };
                    groupProvider.ChangeName(model).then(function () {
                        self.group.name = name;
                    });
                }
            });
        }

        function initTitlePicker() {
            self.titlePickerConfig = new TextControlModel(ControlMode.view);
            self.titlePickerConfig.inputType = InputType.text;
            self.titlePickerConfig.onSave = changeName;
        }

        function initMemberPicker() {
            self.memberPickerConfig = new MemberPickerModel(true, ControlMode.view);
            self.memberPickerConfig.onSave = changeMembers;
            self.memberPickerConfig.isCanRemove = isCanRemoveMember;
            self.memberPickerConfig.removeMessage = $filter('translate')('You cant remove this member');
        }

        function initControl() {
            loadGroup();

            initTitlePicker();
            initMemberPicker();
        }

        initControl();
    }

    angular.module('boligdrift').controller('groupGeneralController', ['$stateParams', '$filter', 'groupProvider', groupGeneralController]);
})();