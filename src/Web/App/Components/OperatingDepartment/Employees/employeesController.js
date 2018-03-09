(function () {
    var employeesController = function ($scope, $filter, employeesManagementProvider, absenceTemplatesProvider, dateHelper) {
        var self = this;
        self.employeesAbsencesList = [];
        self.absenceTemplates = [];
        var message = $filter('translate')('Are you sure?');

        self.isCustomAbsenceTextValid = function (text) {
            return text ? true : false;
        }

        self.isValidTemplateOption = function (option) {
            return option && option.id;
        }

        self.getFormattedDateString = function (dateString) {
            return dateHelper.getLocalDateString(dateString);
        }

        self.toogleAbsenceBlock = function(absence, value) {
            absence.isAddFormVisible = value;
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

        function isFormValid(option, text) {
            self.formValidationResult = self.isValidTemplateOption(text, true) || self.isCustomAbsenceTextValid(option, true);
            return self.formValidationResult;
        }

        function getTomorrowDate() {
            var date = new Date();
            date.setDate(date.getDate() + 1);
            return date;
        }

        function getDatePickerConfig(isStartDateModel, memberId) {
            var model = new DatePickerModel(ControlMode.create);
            model.haveStartDateRestriction = false;
            model.date = dateHelper.getLocalDateString(getTomorrowDate());
            model.view = DatePickerViewType.month;
            model.format = DatePickerFormatType.dayMonthYear;
            model.onChange = isStartDateModel ? startDateChanged : endDateChanged;
            model.memberId = memberId;
            return model;
        }

        function startDateChanged(date, memberId) {
            var employeeAbsence = self.employeesAbsencesList.find(function(absence) { return absence.member.memberId == memberId });
            var endDate = employeeAbsence.newAbsence.endDatePickerConfig.triggerGetDate();
            if (date > endDate) {
                employeeAbsence.newAbsence.endDatePickerConfig.date = date.format(DatePickerFormatType.dayMonthYear.toUpperCase());
                employeeAbsence.newAbsence.endDatePickerConfig.triggerRefresh();
            }
        }

        function endDateChanged(date, memberId) {
            var employeeAbsence = self.employeesAbsencesList.find(function (absence) { return absence.member.memberId == memberId });
            var startDate = employeeAbsence.newAbsence.startDatePickerConfig.triggerGetDate();
            if (startDate > date) {
                employeeAbsence.newAbsence.startDatePickerConfig.date = date.format(DatePickerFormatType.dayMonthYear.toUpperCase());
                employeeAbsence.newAbsence.startDatePickerConfig.triggerRefresh();
            }
        }

        function loadEmployeesAbsences() {
            employeesManagementProvider.getAbsencesForCurrentManagementDepartment().then(function (result) {
                self.employeesAbsencesList = result.data;
                self.employeesAbsencesList.forEach(function (item) {
                    item.newAbsence = {
                        startDatePickerConfig: getDatePickerConfig(true, item.member.memberId),
                        endDatePickerConfig: getDatePickerConfig(false, item.member.memberId),
                        memberId: item.member.memberId
                    };
                });
                loadAbsencesTemplates();
            });
        }

        function loadAbsencesTemplates() {
            absenceTemplatesProvider.GetAll().then(function (result) {
                self.absenceTemplates = result.data;
                var customTemplateOption = { text: $filter('translate')('Other reason') }
                self.absenceTemplates.push(customTemplateOption);
            });
        }

        self.deleteAbsence = function (absences, absence) {
            if (confirm(message)) {
                employeesManagementProvider.deleteAbsence(absence.id).then(function() {
                    var i = absences.indexOf(absence);
                    absences.splice(i, 1);
                });
            }
        }

        self.deleteAllAbsencesForMember = function (absences, memberId) {
            if (confirm(message)) {
                employeesManagementProvider.DeleteAllAbsencesForMember(memberId).then(function() {
                    while (absences.length) {
                        absences.pop();
                    }
                });
            }
        }


        self.addAbsence = function (absences, absence, employeeAbsences) {
            absence.isValidationActive = true;
            if (isFormValid(absence.customAbsenceText, absence.selectedTemplate)) {
                var model = {
                    text: absence.isCustomTemplateMode ? absence.customAbsenceText : absence.selectedTemplate.text,
                    startDate: absence.startDatePickerConfig.triggerGetDate(),
                    endDate: absence.endDatePickerConfig.triggerGetDate(),
                    memberId: absence.memberId,
                    absenceTemplateId: absence.selectedTemplate.id
                };

                employeesManagementProvider.addAbsenceForMember(model).then(function (result) {
                    if (result.data.isSucceeded) {
                        absences = absences ? absences : [];
                        absences.push(result.data.absence);
                        resetNewAbsenceValues(absence);
                        self.toogleAbsenceBlock(employeeAbsences, false);
                    } else {
                        var msg = $filter('translate')('Time range is intersect with existing in the list.');
                        alert(msg);
                    }
                });
            }
        }

        function resetNewAbsenceValues(absence) {
            absence.isValidationActive = false;
            absence.isCustomTemplateMode = false;
            absence.startDatePickerConfig.data = getTomorrowDate();
            absence.startDatePickerConfig.triggerRefresh();
            absence.endDatePickerConfig.data = getTomorrowDate();
            absence.endDatePickerConfig.triggerRefresh();
            absence.customAbsenceText = "";
            absence.selectedTemplate = null;
        }

        self.absenceTemplateSelected = function (selectedTemplate, absence) {
            if (!selectedTemplate.id) {
                absence.isCustomTemplateMode = true;
            }
        }

        var managementListener = $scope.$on('managementDepartmentChanged', loadEmployeesAbsences);
        $scope.$on('$destroy', function () {
            managementListener();
        });

        function initControl() {
            loadEmployeesAbsences();
        }

        initControl();
    };

    angular.module("boligdrift")
        .controller('employeesController',
        [
            '$scope',
            '$filter',
            'employeesManagementProvider',
            'absenceTemplatesProvider',
            'dateHelper',
            employeesController
        ]);
})();