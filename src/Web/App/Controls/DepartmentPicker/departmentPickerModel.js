function DepartmentPickerModel() {
    this.isControlDisabled = false;
    this.triggerValidate = function () { };
    this.triggerSetDepartment = function () { };
    this.currentDepartment = {};
    this.jobId = null;
    this.departments = [];
    this.securityKey = '';
    this.onChange = function() {};
    this.showAll = false;
    this.showAllTranslationKey = 'Show for all departments';
}