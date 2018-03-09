(function () {
    var configuration = function ($stateProvider,
        $urlRouterProvider,
        $translateProvider,
        $httpProvider) {
        $translateProvider.preferredLanguage('da-DK');
        $translateProvider.useLoader('translationLoader');
        $translateProvider.useSanitizeValueStrategy(null);
        $translateProvider.useMissingTranslationHandler('translationCreator');

        $httpProvider.interceptors.push('httpInterceptorService');

        $urlRouterProvider.otherwise('/');

        $stateProvider
            .state(State.Home,
            {
                url: '/'
            })
            .state(State.Employees,
            {
                url: '/employees',
                views: {
                    'main': {
                        templateUrl: '/app/components/operatingDepartment/employees/employees.html',
                        controller: 'employeesController',
                        controllerAs: 'employeesCtrl'
                    }
                }
            })
            .state(State.History,
            {
                url: '/history',
                views: {
                    'main': {
                        controller: 'historyController',
                        controllerAs: 'historyCtrl',
                        templateUrl: '/app/components/Administration/history/history.html'
                    }
                }
            })
            .state(State.Statistics,
            {
                url: '/statistics',
                views: {
                    'main': {
                        templateUrl: '/app/components/statistics/statistics.html',
                        controller: 'statisticsController',
                        controllerAs: 'statisticsCtrl'
                    }
                }
            })
            .state(State.FacilityTask,
            {
                url: '/facilitytask',
                abstract: true
            })
            .state(State.FacilityTaskCreate,
            {
                url: '/create',
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'createController',
                controllerAs: 'createCtrl',
                templateUrl: '/App/Components/FacilityTask/Create/create.html',
                size: 'lg',
                roleList: [1, 4]
            })
            .state(State.FacilityTaskEdit,
            {
                url: '?id&department&dayAssignId&{week:int}&{day:int}&redirectState&{isBackLogJob:bool}&{isEditJobPopupClosed:bool}&{isShowCategoryPanel:bool}&{weekDay:int}',
                params: {
                    id: null,
                    department: null,
                    dayAssignId: null,
                    week: null,
                    day: null,
                    redirectState: State.YearPlanTaskOverview,
                    isBackLogJob: false,
                    weekDay: null
                },
                abstract: true,
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'editController',
                controllerAs: 'editCtrl',
                templateUrl: '/App/Components/FacilityTask/Edit/edit.html',
                size: 'lg'
            })
            .state(State.FacilityTaskGeneral,
            {
                url: '',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'generalController',
                        controllerAs: 'generalCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/General/general.html'
                    }
                }
            })
            .state(State.FacilityTaskLocation,
            {
                url: '/location',
                params: {
                    redirectState: State.YearPlanTaskOverview,
                    department: null
                },
                views: {
                    'modalBody@': {
                        controller: 'locationController',
                        controllerAs: 'locationCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Location/location.html'
                    }
                }
            })
            .state(State.FacilityTaskInterval,
            {
                url: '/interval',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'editTaskIntervalController',
                        controllerAs: 'intervalCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Interval/interval.html'
                    }
                }
            })
            .state(State.FacilityTaskGuide,
            {
                url: '/guide',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'guideController',
                        controllerAs: 'guideCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Guide/guide.html'
                    }
                }
            })
            .state(State.FacilityTaskDocument,
            {
                url: '/document',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'documentController',
                        controllerAs: 'documentCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Document/document.html'
                    }
                }
            })
            .state(State.FacilityTaskAssign,
            {
                url: '/assign',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'editTaskAssignController',
                        controllerAs: 'assignCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Assign/assign.html'
                    }
                }
            })
            .state(State.FacilityTaskDayAssign,
            {
                url: '/dayassign',
                params: { redirectState: State.WeekPlanGridView },
                views: {
                    'modalBody@': {
                        controller: 'dayAssignController',
                        controllerAs: 'dayAssignCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/DayAssign/dayAssign.html'
                    }
                }
            })
            .state(State.FacilityTaskHistory,
            {
                url: '/history',
                params: {
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'modalHistoryController',
                        controllerAs: 'modalHistoryCtrl',
                        templateUrl: '/App/Components/OperatingTasks/History/history.html'
                    }
                }
            })
            .state(State.FacilityTaskResponsible,
            {
                url: '/responsible',
                params: { redirectState: State.YearPlanTaskOverview },
                views: {
                    'modalBody@': {
                        controller: 'responsibleController',
                        controllerAs: 'responsibleCtrl',
                        templateUrl: '/App/Components/FacilityTask/Edit/Responsible/responsible.html'
                    }
                }
            })
            .state(State.OperationalTask,
            {
                url: '/operationaltask',
                abstract: true
            })
            .state(State.OperationalTaskCreate,
            {
                url: '/create',
                abstract: true,
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'operatingTaskController',
                controllerAs: 'operatingTaskCtrl',
                templateUrl: '/App/Components/OperatingTasks/operatingTask.html',
                size: 'lg'
            })
            .state(State.OperationalTaskCreateAdHoc,
            {
                url: '/adhoc?redirectState&{jobType:int}&departmentId',
                params: {
                    redirectState: null,
                    jobType: null,
                    departmentId: null
                },
                views: {
                    'modalBody@': {
                        controller: 'adHocController',
                        controllerAs: 'adHocCtrl',
                        templateUrl: '/App/Components/OperatingTasks/AdHoc/adHoc.html'
                    }
                }
            })
            .state(State.OperationalTaskCreateTenant,
            {
                url: '/tenant?redirectState&{jobType:int}&departmentId',
                params: {
                    redirectState: null,
                    jobType: null,
                    departmentId: null
                },
                views: {
                    'modalBody@': {
                        controller: 'tenantController',
                        controllerAs: 'tenantCtrl',
                        templateUrl: '/App/Components/OperatingTasks/Tenant/tenant.html'
                    }
                }
            })
            .state(State.OperationalTaskCreateOther, {
                url: '/other?redirectState&{jobType:int}&departmentId',
                params: {
                    redirectState: null,
                    jobType: null,
                    departmentId: null
                },
                views: {
                    'modalBody@': {
                        controller: 'otherController',
                        controllerAs: 'otherCtrl',
                        templateUrl: '/App/Components/OperatingTasks/Other/other.html'
                    }
                }
            })
            .state(State.OperationalTaskEdit,
            {
                url: '/edit?dayAssignId&departmentId',
                params: {
                    dayAssignId: null,
                    departmentId: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                abstract: true,
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'operatingTaskController',
                controllerAs: 'operatingTaskCtrl',
                templateUrl: '/App/Components/OperatingTasks/operatingTask.html',
                size: 'lg'
            })
            .state(State.OperationalTaskEditAdHoc,
            {
                url: '/adhoc?id&{jobType:int}&redirectState',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'adHocController',
                        controllerAs: 'adHocCtrl',
                        templateUrl: '/App/Components/OperatingTasks/AdHoc/adHoc.html'
                    }
                }
            })
            .state(State.OperationalTaskEditTenant,
            {
                url: '/tenant?id&{jobType:int}&redirectState',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'tenantController',
                        controllerAs: 'tenantCtrl',
                        templateUrl: '/App/Components/OperatingTasks/Tenant/tenant.html'
                    }
                }
            })
            .state(State.OperationalTaskEditOther, {
                url: '/other?id&{jobType:int}&redirectState',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'otherController',
                        controllerAs: 'otherCtrl',
                        templateUrl: '/App/Components/OperatingTasks/Other/other.html'
                    }
                }
            })
            .state(State.OperationalTaskHistoryTenant,
            {
                url: '/history?id&{jobType:int}',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'modalHistoryController',
                        controllerAs: 'modalHistoryCtrl',
                        templateUrl: '/App/Components/OperatingTasks/History/history.html'
                    }
                }
            })
            .state(State.OperationalTaskHistoryAdHoc,
            {
                url: '/history?id&{jobType:int}',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'modalHistoryController',
                        controllerAs: 'modalHistoryCtrl',
                        templateUrl: '/App/Components/OperatingTasks/History/history.html'
                    }
                }
            })
            .state(State.OperationalTaskHistoryOther,
            {
                url: '/history?id&{jobType:int}',
                params: {
                    id: null,
                    jobType: null,
                    redirectState: State.WeekPlanGridView,
                    listViewCurrentTab: WeekPlanListViewTab.current
                },
                views: {
                    'modalBody@': {
                        controller: 'modalHistoryController',
                        controllerAs: 'modalHistoryCtrl',
                        templateUrl: '/App/Components/OperatingTasks/History/history.html'
                    }
                }
            })
            .state(State.YearPlan,
            {
                url: '/yearplan?{showDisabled:bool}&{showAll:bool}&{isShowCategoryPanel:bool}',
                params: {
                    showDisabled: false,
                    showAll: false,
                    isShowCategoryPanel: false
                },
                views: {
                    'main': {
                        controller: 'yearPlanController',
                        controllerAs: 'yearPlanCtrl',
                        templateUrl: '/app/components/administration/YearPlan/yearPlan.html'
                    }
                }
            })
            .state(State.YearPlanTaskOverview,
            {
                url: '/taskoverview',
                views: {
                    'content@yearplan': {
                        templateUrl: '/app/components/administration/YearPlan/taskOverviewContent.html'
                    }
                }
            })
            .state(State.YearPlanDepartmentOverview,
            {
                url: '/departmentoverview',
                views: {
                    'content@yearplan': {
                        templateUrl: '/app/components/administration/YearPlan/departmentOverviewContent.html'
                    }
                }
            })
            .state(State.Setup,
            {
                url: '/setup',
                abstract: true,
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'commonPageController',
                controllerAs: 'commonCtrl',
                templateUrl: '/app/components/setup/adminModal.html',
                size: 'lg',
                roleList: [1, 4]
            })
            .state(State.SetupCategories,
            {
                url: '/categories',
                views: {
                    'modalBody@': {
                        controller: 'operationCategoriesController',
                        controllerAs: 'operationCtrl',
                        templateUrl: '/app/components/setup/OperationalCategories/operationalCategories.html'
                    }
                },
                roleList: [1, 4]
            })
            .state(State.SetupDangerZone,
            {
                url: '/dangerzone',
                views: {
                    'modalBody@': {
                        controller: 'dangerZoneController',
                        controllerAs: 'dangerZoneCtrl',
                        templateUrl: '/app/components/setup/DangerZone/dangerZone.html'
                    }
                },
                roleList: [1, 4]
            })
            .state(State.SetupTranslation,
            {
                url: '/translations',
                views: {
                    'modalBody@': {
                        controller: 'adminTranslationsController',
                        controllerAs: 'adminTranslationsCtrl',
                        templateUrl: '/app/components/setup/Translations/translations.html'
                    }
                },
                roleList: [1, 4]
            })
            .state(State.AbsenceTemplates,
            {
                url: '/absencetemplates',
                views: {
                    'modalBody@': {
                        controller: 'absenceTemplatesController',
                        controllerAs: 'absenceTemplatesCtrl',
                        templateUrl: '/app/components/setup/absenceTemplates/absenceTemplates.html'
                    }
                },
                roleList: [1, 4]
            })
            .state(State.CancellingTemplates,
            {
                url: '/cancellingtemplates',
                views: {
                    'modalBody@': {
                        controller: 'cancellingTemplatesController',
                        controllerAs: 'cancellingTemplatesCtrl',
                        templateUrl: '/app/components/setup/cancellingTemplates/cancellingTemplates.html'
                    }
                },
                roleList: [1, 4]
            })
            .state(State.CoordinatorTemplates,
                {
                    url: '/coordinatortemplates',
                    views: {
                        'modalBody@': {
                            controller: 'coordinatorTemplatesController',
                            controllerAs: 'coordinatorTemplatesCtrl',
                            templateUrl: '/app/components/setup/coordinatorTemplates/coordinatorTemplates.html'
                        }
                    }
                })
            .state(State.WeekPlan,
            {
                url: '/weekplan',
                views: {
                    'main': {
                        controller: 'weekPlanController',
                        controllerAs: 'weekPlanCtrl',
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlan.html'
                    }
                }
            })
            .state(State.WeekPlanGridView,
            {
                url: '/gridview?{isEditJobPopupClosed:bool}',
                params: { isEditJobPopupClosed: false },
                views: {
                    'content@weekplan': {
                        controller: 'weekPlanGridController',
                        controllerAs: 'weekPlanGridCtrl',
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanGrid/weekPlanGridView.html'
                    }
                }
            })
            .state(State.WeekPlanListView,
            {
                url: '/listview',
                params: { listViewCurrentTab: WeekPlanListViewTab.current },
                views: {
                    'content@weekplan': {
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanList/weekPlanListView.html'
                    }
                }
            })
            .state(State.WeekPlanTimeView,
            {
                url: '/timeview',
                views: {
                    'content@weekplan': {
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanTime/weekPlanTimeView.html'
                    }
                }
            })
            .state(State.WeekPlanListViewCurrentTasks,
            {
                url: '/currenttasks',
                views: {
                    'content@weekplan.listview': {
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanList/currentTasksContent.html'
                    }
                }
            })
            .state(State.WeekPlanListViewCompletedTasks,
            {
                url: '/completedtasks',
                views: {
                    'content@weekplan.listview': {
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanList/completedTasksContent.html'
                    }
                }
            })
            .state(State.WeekPlanListViewNotCompletedTasks,
            {
                url: '/notcompletedtasks',
                views: {
                    'content@weekplan.listview': {
                        templateUrl: '/app/components/operatingDepartment/weekPlan/weekPlanList/notCompletedTasksContent.html'
                    }
                }
            })
            .state(State.Groups,
            {
                url: '/groups',
                views: {
                    'main': {
                        controller: 'groupOverviewController',
                        controllerAs: 'groupOverviewCtrl',
                        templateUrl: '/app/components/operatingDepartment/groupOverview/groupOverview.html'
                    }
                },
                roleList: [1, 4]

            })
            .state(State.Group,
            {
                url: '/group',
                abstract: true
            })
            .state(State.GroupCreate,
            {
                url: '/create',
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'groupCreateController',
                controllerAs: 'groupCreateCtrl',
                templateUrl: '/app/components/group/create/groupCreate.html',
                size: 'lg'
            })
            .state(State.GroupEdit,
            {
                url: '?id',
                params: {
                    id: null
                },
                abstract: true,
                modal: true,
                animation: true,
                backdrop: 'static',
                controller: 'groupEditController',
                controllerAs: 'groupEditCtrl',
                templateUrl: '/app/components/group/edit/groupEdit.html',
                size: 'lg'
            })
            .state(State.GroupEditGeneral,
            {
                url: '/assign',
                views: {
                    'modalBody@': {
                        controller: 'groupGeneralController',
                        controllerAs: 'groupGeneralCtrl',
                        templateUrl: '/app/components/group/edit/general/groupGeneral.html'
                    }
                }
            })
            .state(State.Authorization,
            {
                url: '/login',
                views: {
                    'login': {
                        templateUrl: '/app/components/authorization/authorizationview.html'
                    }
                }
            })
            .state(State.JanitorCentrallFeed,
            {
                url: '/centrallfeed',
                views: {
                    'main': {
                        controller: 'centralFeedController',
                        controllerAs: 'centralFeedCtrl',
                        templateUrl: '/app/components/janitor/centralFeed.html'
                    }
                }
            })
            .state(State.JanitorMyTasks,
            {
                url: '/mytasks',
                views: {
                    'content@centrallfeed': {
                        controller: 'myTasksController',
                        controllerAs: 'myTaskCtrl',
                        templateUrl: '/app/components/janitor/mytasks/mytasks.html'
                    }
                }
            })
            .state(State.JanitorOpenedTasks,
            {
                url: '/opentasks',
                views: {
                    'content@centrallfeed': {
                        controller: 'openTasksController',
                        controllerAs: 'openTaskCtrl',
                        templateUrl: '/app/components/janitor/opentasks/opentasks.html'
                    }
                }
            })
            .state(State.JanitorCompletedTasks,
            {
                url: '/completedtasks',
                views: {
                    'content@centrallfeed': {
                        controller: 'completedTasksController',
                        controllerAs: 'completedTaskCtrl',
                        templateUrl: '/app/components/janitor/completedtasks/completedtasks.html'
                    }
                }
            })
            .state(State.JanitorTaskDetails,
            {
                url: '/taskdetails?id&dayAssignId&previousState&jobType&previousListPage',
                params: { id: null, dayAssignId: null, previousState: null, jobType: null, previousListPage: null },
                views: {
                    'content@centrallfeed': {
                        controller: 'taskDetailsController',
                        controllerAs: 'taskDetailsCtrl',
                        templateUrl: '/app/components/janitor/taskdetails/taskdetails.html'
                    }
                }
            })
            .state(State.JanitorSettings,
            {
                url: '/settings',
                views: {
                    'content@centrallfeed': {
                        controller: 'janitorSettingsController',
                        controllerAs: 'janitorSettingsCtrl',
                        templateUrl: '/app/components/janitor/settings/janitorSettings.html'
                    }
                }
            })
            .state(State.JanitorMap,
            {
                url: '/map',
                views: {
                    'content@centrallfeed': {
                        controller: 'janitorMapController',
                        controllerAs: 'janitorMapCtrl',
                        templateUrl: '/app/components/janitor/map/janitorMap.html'
                    }
                }
            });
    };

    var toastConfig = function (ngToast) {
        ngToast.configure({
            horizontalPosition: 'center'
        });
    }

    var run = function ($rootScope, $state) {
        $rootScope.$state = $state;
    };

    angular.module('boligdrift')
        .config(['$stateProvider', '$urlRouterProvider', '$translateProvider', '$httpProvider', configuration])
        .config(['ngToastProvider', toastConfig])
        .run(['$rootScope', '$state', run]);

})();
