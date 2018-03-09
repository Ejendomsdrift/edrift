var State = Object.freeze({
    Authorization: 'authorization',

    Home: 'Home',

    Employees: 'Employees',

    History: 'History',

    Statistics: 'Statistics',

    Setup: 'setup',
    SetupCategories: 'setup.categories',
    SetupTranslation: 'setup.translations',
    SetupDangerZone: 'setup.dangerzone',
    AbsenceTemplates: 'setup.absencetemplates',
    CancellingTemplates: 'setup.cancellingTemplates',
    CoordinatorTemplates: 'setup.coordinatorTemplates',

    Groups: 'groups',
    Group: 'group',
    GroupCreate: 'group.create',
    GroupEdit: 'group.edit',
    GroupEditGeneral: 'group.edit.general',

    YearPlan: 'yearplan',
    YearPlanTaskOverview: 'yearplan.taskoverview',
    YearPlanDepartmentOverview: 'yearplan.departmentoverview',

    WeekPlan: 'weekplan',
    WeekPlanGridView: 'weekplan.gridview',
    WeekPlanListView: 'weekplan.listview',
    WeekPlanTimeView: 'weekplan.timeview',
    WeekPlanListViewCurrentTasks: 'weekplan.listview.currenttasks',
    WeekPlanListViewCompletedTasks: 'weekplan.listview.completedtasks',
    WeekPlanListViewNotCompletedTasks: 'weekplan.listview.notcompletedtasks',

    FacilityTask: 'facilitytask',
    FacilityTaskCreate: 'facilitytask.create',
    FacilityTaskEdit: 'facilitytask.edit',
    FacilityTaskGeneral: 'facilitytask.edit.general',
    FacilityTaskLocation: 'facilitytask.edit.location',
    FacilityTaskInterval: 'facilitytask.edit.interval',
    FacilityTaskGuide: 'facilitytask.edit.guide',
    FacilityTaskDocument: 'facilitytask.edit.document',
    FacilityTaskAssign: 'facilitytask.edit.assign',
    FacilityTaskDayAssign: 'facilitytask.edit.dayAssign',
    FacilityTaskHistory: 'facilitytask.edit.history',
    FacilityTaskResponsible: 'facilitytask.edit.responsible',

    OperationalTask: 'operationaltask',
    OperationalTaskCreate: 'operationaltask.create',
    OperationalTaskEdit: 'operationaltask.edit',
    OperationalTaskCreateAdHoc: 'operationaltask.create.adhoc',
    OperationalTaskEditAdHoc: 'operationaltask.edit.adhoc',
    OperationalTaskHistoryAdHoc: 'operationaltask.edit.adhochistory',
    OperationalTaskCreateTenant: 'operationaltask.create.tenant',
    OperationalTaskEditTenant: 'operationaltask.edit.tenant',
    OperationalTaskHistoryTenant: 'operationaltask.edit.tenanthistory',
    OperationalTaskCreateOther: 'operationaltask.create.other',
    OperationalTaskEditOther: 'operationaltask.edit.other',
    OperationalTaskHistoryOther: 'operationaltask.edit.otherhistory',

    JanitorCentrallFeed: 'centrallfeed',
    JanitorMyTasks: 'centrallfeed.mytasks',
    JanitorOpenedTasks: 'centrallfeed.opentasks',
    JanitorCompletedTasks: 'centrallfeed.completedtasks',
    JanitorTaskDetails: 'centrallfeed.taskdetails',
    JanitorSettings: 'centrallfeed.settings',
    JanitorMap: 'centrallfeed.map'
});