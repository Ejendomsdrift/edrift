const content = './Content/';
const dest = './Content/Compiled/';

const libs = content + 'libs/';
const cssFolder = content + 'css/';
const theme = cssFolder + 'theme/';

const app = './App/';
const components = app + 'Components/';
const controls = app + 'Controls/';
const shared = app + 'Shared/';
const scripts = './Scripts/';

var less = {
    in: [app + 'app.less'],
    outFile: 'bundle.css',
    watch: app + '**/*.less'
};

const cssLibsPaths = [
    libs + 'bootstrap.min.css',
    cssFolder + 'pe-icon-7-stroke.css',
    cssFolder + 'helper.css',
    cssFolder + 'font-awesome.css',
    theme + 'metisMenu.css',
    theme + 'animate.css',
    theme + 'style.css',
    libs + 'blueimp-gallery/css/blueimp-gallery.css'
];

const jsLibsPath = [
    scripts + 'jquery.signalR-2.2.1.min.js',
    libs + 'blueimp-gallery/js/blueimp-gallery.js',
    libs + 'underscore/underscore-min.js',
    libs + 'stack-worker/stack-worker.js',
    libs + 'pipeline/pipeline.js',
    libs + 'chart-middleware/app-common.js',
    libs + 'chart-middleware/app-bootstrap-date-picker-provider.js',
    libs + 'chart-middleware/app-chartjs-provider.js',
    libs + 'chart-middleware/app-xhr-blob-download-file-provider.js',
    libs + 'chart-middleware/chart-middleware.js'
];

const jsFiles = [
    app + 'app.module.js',
    app + 'app.config.js',

    shared + 'polyfills.js',
    shared + 'toastService.js',

    shared + 'Enums/jobStatus.js',
    shared + 'Enums/weekDays.js',
    shared + 'Enums/role.js',
    shared + 'Enums/jobType.js',
    shared + 'Enums/state.js',
    shared + 'Enums/onDemandUploaderView.js',
    shared + 'Enums/weekDays.js',
    shared + 'Enums/contentType.js',
    shared + 'Enums/controlMode.js',
    shared + 'Enums/keyCodes.js',
    shared + 'Enums/tenantTaskType.js',
    shared + 'Enums/datePickerViewType.js',
    shared + 'Enums/datePickerFormatType.js',
    shared + 'Enums/inputType.js',
    shared + 'Enums/tabSecurityKey.js',
    shared + 'Enums/controlSecurityKey.js',
    shared + 'Enums/phoneMask.js',
    shared + 'Enums/pages.js',
    shared + 'Enums/month.js',
    shared + 'Enums/weekPlanListViewTab.js',
    shared + 'Enums/historyControlColumns.js',
    shared + 'Enums/jobStatusPlatformKey.js',
    shared + 'Enums/teamPickerState.js',
    
    shared + 'Directives/confirmation.js',
    shared + 'Directives/focus.js',
    shared + 'Directives/enter.js',
    shared + 'Directives/security.js',
    shared + 'Directives/fileDownloader.js',
    shared + 'Directives/lazyLoad.js',
    
    shared + 'Modal/confirmCtrl.js',

    shared + 'Providers/adminToolsProvider.js',
    shared + 'Providers/authorizationProvider.js',
    shared + 'Providers/categoryProvider.js',
    shared + 'Providers/departmentProvider.js',
    shared + 'Providers/jobProvider.js',
    shared + 'Providers/operationalTaskProvider.js',
    shared + 'Providers/uploadDataProvider.js',
    shared + "Providers/managementProvider.js",
    shared + 'Providers/yearPlanProvider.js',
    shared + 'Providers/calendarProvider.js',
    shared + 'Providers/weekPlanProvider.js',
    shared + 'Providers/groupProvider.js',
    shared + 'Providers/memberProvider.js',
    shared + 'Providers/settingProvider.js',
    shared + 'Providers/securityProvider.js',
    shared + 'Providers/employeesManagementProvider.js',
    shared + 'Providers/absenceTemplatesProvider.js',
    shared + 'Providers/cancellingTemplatesProvider.js',
    shared + 'Providers/historyProvider.js',
    shared + 'Providers/statisticsProvider.js',

    shared + 'Services/securityService.js',
    shared + 'Services/urlService.js',
    shared + 'Services/operationalTaskService.js',

    shared + 'Interceptors/httpInterceptorService.js',

    shared + 'Helpers/dateHelper.js',
    
    shared + 'Helpers/treeHelper.js',

    components + 'Administration/YearPlan/yearPlanController.js',
    components + 'Administration/YearPlan/yearPlanService.js',
    components + 'Administration/YearPlan/YearPlanDirective/yearPlan.js',
    components + 'Administration/History/historyController.js',

    components + 'OperatingDepartment/WeekPlan/weekPlanController.js',
    components + 'OperatingDepartment/WeekPlan/weekPlanGrid/weekPlanGridController.js',
    components + 'OperatingDepartment/WeekPlan/weekPlanList/weekPlanListController.js',
    components + 'OperatingDepartment/WeekPlan/weekPlanTime/weekPlanTimeController.js',
    components + 'OperatingDepartment/WeekPlan/weekPlanService.js',

    components + 'OperatingDepartment/GroupOverview/groupOverviewController.js',
    components + 'OperatingDepartment/GroupOverview/groupOverviewService.js',
    components + 'OperatingDepartment/Employees/employeesController.js',

    components + 'FacilityTask/Create/createController.js',
    components + 'FacilityTask/Edit/Assign/assignController.js',
    components + 'FacilityTask/Edit/DayAssign/dayAssignController.js',
    components + 'FacilityTask/Edit/Document/documentController.js',
    components + 'FacilityTask/Edit/General/generalController.js',
    components + 'FacilityTask/Edit/Guide/guideController.js',
    components + 'FacilityTask/Edit/Interval/intervalController.js',
    components + 'FacilityTask/Edit/editController.js',
    components + 'FacilityTask/Edit/Location/locationController.js',
    components + 'FacilityTask/Edit/Responsible/responsibleController.js',

    components + 'OperatingTasks/AdHoc/adHocController.js',
    components + 'OperatingTasks/Tenant/tenantController.js',
    components + 'OperatingTasks/Other/otherController.js',
    components + 'OperatingTasks/History/modalHistoryController.js',
    components + 'OperatingTasks/operatingTaskController.js',

    components + 'Group/Create/groupCreateController.js',
    components + 'Group/Edit/General/groupGeneralController.js',
    components + 'Group/Edit/groupEditController.js',

    components + 'Authorization/authorizationController.js',

    components + 'Header/headerController.js',
    
    components + 'Common/commonPageController.js',    

    components + 'Setup/DangerZone/dangerZoneController.js',
    components + 'Setup/OperationalCategories/operationCategoriesController.js',
    components + 'Setup/Translations/adminTranslationsController.js',
    components + 'Setup/absenceTemplates/absenceTemplatesController.js',
    components + 'Setup/cancellingTemplates/cancellingTemplatesController.js',
    components + 'Setup/coordinatorTemplates/coordinatorTemplatesController.js',

    components + 'TopNavigation/topNavigationController.js',
    components + 'TopNavigation/topNavigationProvider.js',

    components + 'Janitor/centralFeedController.js',
    components + 'Janitor/MyTasks/myTasksController.js',
    components + 'Janitor/OpenTasks/openTasksController.js',
    components + 'Janitor/CompletedTasks/completedTasksController.js',
    components + 'Janitor/TaskDetails/taskDetailsController.js',

    components + '/Statistics/statisticsController.js',
    
    components + 'Janitor/janitorService.js',
    components + 'Janitor/Settings/janitorSettingsController.js',
    components + 'Janitor/Map/janitorMapController.js',

    controls + 'DepartmentPicker/departmentPickerModel.js',
    controls + 'DepartmentPicker/SimpleDepartmentPicker/simpleDepartmentPicker.js',
    controls + 'DepartmentPicker/MultiSelectDepartmentPicker/multiDepartmentPicker.js',
    controls + 'DepartmentPicker/SingleSelectDepartmentPicker/departmentPicker.js',

    controls + 'ManagementDepartmentPicker/managementDepartmentPicker.js',

    controls + 'MultiSelectChecker/multiSelectChecker.js',

    controls + 'NativeCarousel/nativeCarousel.js',
    controls + 'CategoryTree/categoryTree.js',
    controls + 'GoogleMap/googleMap.js',
    controls + 'UploadsManagement/uploadsManagement.js',
    controls + 'TeamPicker/teamPicker.js',
    controls + 'TeamPicker/teamPickerModel.js',
    controls + 'FileUploader/fileUploader.js',
    controls + 'FileUploader/fileUploaderModel.js',
    controls + 'DaysPerWeekPicker/daysPerWeekPicker.js',
    controls + 'DaysPerWeekPicker/daysPerWeekPickerModel.js',
    controls + 'WeeksPicker/weeksPicker.js',
    controls + 'WeeksPicker/weeksPickerModel.js',
    controls + 'DatePicker/datePicker.js',
    controls + 'DatePicker/datePickerModel.js',
    controls + 'TimePicker/timePicker.js',
    controls + 'TimePicker/timePickerModel.js',
    controls + 'UserAvatar/userAvatar.js',
    controls + 'UserAvatar/userAvatarModel.js',
    controls + 'EditControl/editControl.js',
    controls + 'EditControl/editControlModel.js',
    controls + 'MemberPicker/memberPicker.js',
    controls + 'MemberPicker/memberPickerModel.js',
    controls + 'TeamViewer/teamViewer.js',
    controls + 'TeamViewer/teamViewerModel.js',
    controls + 'YearWeekSelector/yearWeekSelector.js',
    controls + 'TextControl/textControl.js',
    controls + 'TextControl/textControlModel.js',
    controls + 'AssignDepartmentPicker/assignDepartmentPicker.js',
    controls + 'AssignDepartmentPicker/assignDepartmentPickerModel.js',
    controls + 'AddressControl/addressControl.js',
    controls + 'AddressControl/addressControlModel.js',
    controls + 'CategoryTreePicker/categoryTreePicker.js',
    controls + 'JobMapControl/jobMapControl.js',
    controls + 'OrdinalDatePicker/ordinalDatePicker.js',
    controls + 'CategoryTreePicker/categoryTreePickerModel.js',
    controls + 'PerWeekJobSchedulePicker/perWeekJobSchedulePicker.js',
    controls + 'PerWeekJobSchedulePicker/perWeekJobSchedulePickerModel.js',
    controls + 'RichTextEditorControl/richTextEditor.js',
    controls + 'RichTextEditorControl/richTextEditorModel.js',
    controls + 'UploadedContentViewer/uploadedContentViewer.js',
    controls + 'UploadedContentViewer/uploadedContentViewerModel.js',
    controls + 'TextAreaControl/textAreaControl.js',
    controls + 'TextAreaControl/textAreaControlModel.js',
    controls + 'TenantTypePicker/tenantTaskPicker.js',
    controls + 'TenantTypePicker/tenantTaskPickerModel.js',
    controls + 'PhoneInputControl/phoneInputControl.js',
    controls + 'PhoneInputControl/phoneInputControlModel.js',
    controls + 'JanitorUploadedContentViewer/janitorUploadedContentViewerModel.js',
    controls + 'JanitorUploadedContentViewer/janitorUploadedContentViewer.js',
    controls + 'HistoryControl/historyControl.js',
    controls + 'HistoryControl/historyModel.js',
    controls + 'SelectControl/selectControl.js',
    controls + 'SelectControl/selectControlModel.js',
    controls + 'CancellingReason/cancellingReason.js',
    controls + 'CancellingReason/cancellingReasonModel.js'
];

var js = {
    in: jsFiles,
    outFile: 'scripts.js',
    watch: app + '**/*.js'
}

var jsLibs = {
    in: jsLibsPath,
    outFile: 'libs.js'
}

var cssLibs = {
    in: cssLibsPaths,
    outFile: 'libs.css'
};

module.exports = {
    js: js,
    jsLibs: jsLibs,
    dest: dest,
    less: less,
    cssLibs: cssLibs,
    clean: [dest + '**/*']
};