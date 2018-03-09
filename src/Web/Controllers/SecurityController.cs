using SecurityCore.Contract.Interfaces;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Infrastructure.Enums;
using Infrastructure.Extensions;
using MemberCore.Contract.Enums;
using MemberCore.Contract.Interfaces;
using Web.Enums;
using Web.Models.Security;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Controllers
{
    [RoutePrefix("api/security")]
    public class SecurityController : ApiController
    {
        private readonly ISecurityService securityService;
        private readonly IJobService jobService;
        private readonly IMemberService memberService;
        private readonly IMemberModel currentUser;
        private readonly IDayAssignService dayAssignService;

        public SecurityController(
            ISecurityService securityService,
            IJobService jobService,
            IMemberService memberService,
            IDayAssignService dayAssignService)
        {
            this.securityService = securityService;
            this.jobService = jobService;
            this.memberService = memberService;
            this.dayAssignService = dayAssignService;

            currentUser = this.memberService.GetCurrentUser();
        }

        [HttpPost, Route("hasAccessByKeyList")]
        public Dictionary<string, bool> HasAccessByKeyList(SequrityQueryViewModel model)
        {
            var isMobile = HttpContext.Current != null && HttpContext.Current.Request.Browser.IsMobileDevice;
            var query = new SequrityQueryViewModel
            {
                KeyList = model.KeyList,
                Page = model.Page,
                Member = currentUser,
                CurrentPlatformType = isMobile ? PlatformType.Mobile : PlatformType.Desktop
            };

            var mappedQuery = query.Map<ISecurityQuery>();
            var result = securityService.HasAccessByKeyList(mappedQuery);
            return result;
        }

        [HttpPost, Route("hasAccessByGroupName")]
        public Dictionary<string, bool> HasAccessByGroupName(SequrityQueryViewModel model)
        {
            IDayAssign dayAssign = null;
            IJob job = jobService.GetJobById(model.JobId).Result;
            var isMobile = HttpContext.Current != null && HttpContext.Current.Request.Browser.IsMobileDevice;

            if (model.DayAssignId.HasValue)
            {
                dayAssign = dayAssignService.GetDayAssignById(model.DayAssignId.Value);
            }

            model.CreatorRole = job?.CreatedByRole;
            model.Member = currentUser;
            model.DayAssignStatus = dayAssign?.StatusId;
            model.DayAssignDate = dayAssign?.Date;
            model.CurrentPlatformType = isMobile ? PlatformType.Mobile : PlatformType.Desktop;
            model.IsGroupedTask = job?.ParentId.HasValue() ?? false;

            var mappedQuery = model.Map<ISecurityQuery>();
            var result = securityService.HasAccessByGroupName(mappedQuery);
            return result;
        }

        [HttpGet, Route("addDefaultKeys")]
        public void AddDefaultKeys()
        {
            var keys = new List<SecurityPermissionViewModel>();
            keys.AddRange(GetDefaultSidebarSecurityKeyModel());
            keys.AddRange(GetDefaultHeaderSecurityKeyModel());
            keys.AddRange(GetDefaulYearPlanSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskCreateTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskTabSecutiryKeyModel());
            keys.AddRange(GetDefaultFacilityTaskGeneralTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskLocationTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskIntervalTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskGuideTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskDocumentTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskAssignTabSecurityKeyModel());
            keys.AddRange(GetDefaultFacilityTaskDayAssignTabSecurityKeyModel());
            //keys.AddRange(GetDefaultFacilityTaskResponsibleTabSecurityKeyModel());
            keys.AddRange(GetDefaultOperationalTaskSecurityKeyModel());
            keys.AddRange(GetDefaultOperationalTaskTypeSecurityKeyModel());
            keys.AddRange(GetCommonSecurityKeyModel());
            keys.AddRange(GetCommonSecurityPageModel());

            foreach (var key in keys)
            {
                var permission = key.Map<ISecurityPermissionModel>();
                securityService.Save(permission);
            }
        }

        [HttpPost, Route("addKey")]
        public void AddKey(SecurityPermissionViewModel model)
        {
            var permission = model.Map<ISecurityPermissionModel>();
            securityService.Save(permission);
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultSidebarSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_WeekPlanGridView",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_Groups",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_History",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_Statistics",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_Employees",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_YearPlanTaskOverview",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.SuperAdmin,
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_SetupTranslation",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_JanitorMyTasks",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "LeftMenuItem_JanitorOpenedTasks",
                    GroupName = "LeftMenu",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultHeaderSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "HeaderItem_SwitchPlatformButton",
                    GroupName = "Header",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            IsUserShouldHaveAllRoles = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Janitor
                            },
                            UserRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Janitor
                            },
                            AllowedPlatformList = new List<PlatformType>
                            {
                                PlatformType.Desktop
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaulYearPlanSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_CreateButton",
                    GroupName = "YearPlan",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            }
                        }
                    }

                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskTabSecutiryKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_GeneralTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_LocationTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_IntervalTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_GuideTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_DocumentTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_AssignTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_DayAssignTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_HistoryTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_ResponsibleTab",
                    GroupName = "FacilityTaskTabs",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            /*
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                            */
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskCreateTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_Create_Category",
                    GroupName = "FacilityTask_Create",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_Create_AddTaskUnderSelectedCategory",
                    GroupName = "FacilityTask_Create",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_Create_TitleAdmin",
                    GroupName = "FacilityTask_Create",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_Create_TitleCoordinator",
                    GroupName = "FacilityTask_Create",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FacilityTask_Create_SubmitButton",
                    GroupName = "FacilityTask_Create",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskGeneralTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "General_Title",
                    GroupName = "FacilityTask_General",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "General_Category",
                    GroupName = "FacilityTask_General",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "General_HideTask",
                    GroupName = "FacilityTask_General",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "General_GroupTask",
                    GroupName = "FacilityTask_General",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskLocationTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Location_Address",
                    GroupName = "FacilityTask_Location",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskIntervalTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Interval_GlobalWeeks",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Interval_GlobalShedule",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Interval_GlobalDate",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Interval_LocalWeeks",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Interval_LocalShedule",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Interval_LocalDate",
                    GroupName = "FacilityTask_Interval",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskGuideTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Guide_GlobalDescription",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_GlobalImages",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_GlobalVideos",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_LocalDescription",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_LocalImages",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_LocalVideos",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Guide_RemoveComment",
                    GroupName = "FacilityTask_Guide",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskDocumentTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Document_GlobalDocument",
                    GroupName = "FacilityTask_Document",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            IsDisabledForGroupingTask = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Document_LocalDocument",
                    GroupName = "FacilityTask_Document",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskAssignTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Assign_Departments",
                    GroupName = "FacilityTask_Assign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = false,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskDayAssignTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "DayAssign_Estimate",
                    GroupName = "FacilityTask_DayAssign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "DayAssign_DaysPerWeek",
                    GroupName = "FacilityTask_DayAssign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "DayAssign_Team",
                    GroupName = "FacilityTask_DayAssign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "DayAssign_OpenTask",
                    GroupName = "FacilityTask_DayAssign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "DayAssign_CancelTask",
                    GroupName = "FacilityTask_DayAssign",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultFacilityTaskResponsibleTabSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "estimate",
                    GroupName = "FacilityTask_Responsible",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "team",
                    GroupName = "FacilityTask_Responsible",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.YearPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            },
                            EditRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.Coordinator
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultOperationalTaskSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "Category",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Date",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "EstimatedTime",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Title",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Description",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Address",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Documents",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Responsible",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "HousingDepartment",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "TenantType",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Time",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "NameOfResident",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Phone",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Comment",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "FileUploader",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "DeleteDocument",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "CancelTask",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "ReOpenTask",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "Urgent",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            IsEditable = true,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "SaveNewOperationalTask",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "AssignNewOperationalTask",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "OpenNewOperationalTask",
                    GroupName = "OperationalTask",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.History
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetDefaultOperationalTaskTypeSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "AdHocCreate",
                    GroupName = "OperationalTaskType",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "TenantCreate",
                    GroupName = "OperationalTaskType",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "OtherCreate",
                    GroupName = "OperationalTaskType",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            Page = Pages.WeekPlan,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        },
                        new RuleViewModel
                        {
                            Page = Pages.MyTasks,
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetCommonSecurityKeyModel()
        {
            return new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "AllowedRolesForAssigningOnJob",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                }
            };
        }

        private IEnumerable<SecurityPermissionViewModel> GetCommonSecurityPageModel()
        {
            var result = new List<SecurityPermissionViewModel>
            {
                new SecurityPermissionViewModel
                {
                    Key = "yearPlan_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.SuperAdmin,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "weekPlan_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "myTasks_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Janitor
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "history_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.SuperAdmin,
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "groups_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "setup_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "facilityTask_Create_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "facilityTask_Edit_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "adHockTask_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Janitor,
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "tenantTask_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Janitor,
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "otherTask_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Janitor,
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                },
                new SecurityPermissionViewModel
                {
                    Key = "statistics_Page",
                    Rules = new List<RuleViewModel>
                    {
                        new RuleViewModel
                        {
                            ViewRoleList = new List<RoleType>
                            {
                                RoleType.Coordinator,
                                RoleType.Administrator,
                                RoleType.SuperAdmin
                            }
                        }
                    }
                }
            };

            return result;
        }
    }
}