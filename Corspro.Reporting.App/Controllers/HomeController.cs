using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AutoMapper;
using Corspro.Business.External;
using Corspro.Domain.Dto;
using Corspro.Reporting.App.Models;

namespace Corspro.Reporting.App.Controllers
{
    public class HomeController : Controller
    {
        private UserDto _authUser;
        private readonly UserBL _userBl;
        private readonly OpportunityBL _opportunityBl;
        private readonly QuoteBL _quoteBl;
        private readonly ClientLoginBL _clientLoginBL;
        private readonly ClientDefinedFieldBL _clientDefinedFieldBL;
        private readonly UtilityBL _utilityBL;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public HomeController()
        {
            _userBl = new UserBL();
            _opportunityBl = new OpportunityBL();
            _quoteBl = new QuoteBL();
            _clientLoginBL = new ClientLoginBL();
            _clientDefinedFieldBL = new ClientDefinedFieldBL();
            _utilityBL = new UtilityBL();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["AuthenticatedUser"] != null)
                _authUser = (UserDto)Session["AuthenticatedUser"];
        }


        #region UserViews
        /// <summary>
        /// Users the views.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UserViews()
        {
            var displayViews = new List<UserHandlerModel>(); ;

            //displayViews.Add(new UserHandlerModel
            //{
            //    UserViewId = "Views",
            //    StrMessage = "       Views"
            //});

            displayViews.Add(new UserHandlerModel
            {
                UserViewId = "SaveNewViewAs",
                StrMessage = "Save Current View as …"
            });

            displayViews.Add(new UserHandlerModel
            {
                UserViewId = "SaveAsDefault",
                StrMessage = "Set Current View as Default"
            });

            displayViews.Add(new UserHandlerModel
            {
                UserViewId = "DeleteViews",
                StrMessage = "Delete View(s) …"
            });

            displayViews.Add(new UserHandlerModel
            {
                UserViewId = "StandardArchived",
                StrMessage = "Clear View"
            });

            var viewPrefs = _userBl.GetAllUserViewsByUserAndClient(_authUser.ClientId, _authUser.UserId);

            var standardView = viewPrefs.Where(v => v.ViewName.ToUpper().Equals("STANDARD")).FirstOrDefault();

            //if (standardView != null)
            //{
            //displayViews.Add(new UserHandlerModel
            //{
            //    UserViewId = "Standard",//standardView.UserViewID.ToString(),
            //    StrMessage = "Standard (Non-Archived)"
            //});

            viewPrefs.Remove(standardView);
            //}

            //displayViews.Add(new UserHandlerModel
            //{
            //    UserViewId = "Archived",//standardView.UserViewID.ToString(),
            //    StrMessage = "Standard (Archived)"
            //});

            //displayViews.Add(new UserHandlerModel
            //{
            //    id = "StandardArchived",//standardView.UserViewID.ToString(),
            //    text = "Standard (All)",
            //    parentid = "Views"
            //});

            var defaultView = viewPrefs.Where(v => v.ViewName.ToUpper().Equals("DEFAULT")).FirstOrDefault();

            if (defaultView != null)
            {
                displayViews.Add(new UserHandlerModel
                {
                    UserViewId = defaultView.UserViewID.ToString(),
                    StrMessage = "Default"
                });
                viewPrefs.Remove(defaultView);
            }

            foreach (UserViewDto u in viewPrefs)
            {
                var a = new UserHandlerModel
                {
                    UserViewId = u.UserViewID.ToString(),
                    StrMessage = u.ViewName
                };
                displayViews.Add(a);
            }
            return Json(displayViews, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Users the view data grid.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UserViewDataGrid()
        {
            var displayViews = new List<UserHandlerModel>(); ;

            var viewPrefs = _userBl.GetUserViewsByUserAndClient(_authUser.ClientId, _authUser.UserId);

            foreach (UserViewDto u in viewPrefs)
            {
                var a = new UserHandlerModel
                {
                    MgUserId = u.UserViewID,
                    StrMessage = u.ViewName
                };
                displayViews.Add(a);
            }
            return Json(displayViews, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Loads the default user view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadDefaultUserView()
        {
            var userView = new UserViewDto
            {
                UserID = _authUser.UserId,
                ClientID = _authUser.ClientId,
                ViewName = "Default"
            };
            var viewPref = _userBl.GetUserViewByName(userView);
            return Json(viewPref, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Loads the view by identifier.
        /// </summary>
        /// <param name="viewId">The view identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadViewById(string viewId)
        {
            var viewPref = _userBl.GetUserViewById(Int32.Parse(viewId));
            return Json(viewPref, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the default user view.
        /// </summary>
        /// <param name="viewPref">The view preference.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveDefaultUserView(string viewPref)
        {
            var userView = new UserViewDto
            {
                UserID = _authUser.UserId,
                ClientID = _authUser.ClientId,
                View = viewPref,
                ViewName = "Default"
            };
            var userAdded = _userBl.SaveUserViewByName(userView);
            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the new user view.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="viewPref">The view preference.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveNewUserView(string viewName, string viewPref)
        {
            var userView = new UserViewDto
            {
                UserID = _authUser.UserId,
                ClientID = _authUser.ClientId,
                View = viewPref,
                ViewName = viewName
            };
            var nList = _userBl.SaveNewUserView(userView);

            var userAdded = new UserHandlerModel();
            if (nList > 0)
            {
                userAdded.MgUserId = nList;
                userAdded.StrMessage = "Added";
            }
            else if (nList == 0)
            {
                userAdded.MgUserId = nList;
                userAdded.StrMessage = "Cannot store more than 30 views";
            }
            //else if (nList == -2)
            //{
            //    userAdded.MgUserId = nList;
            //    userAdded.StrMessage = "There is already a view with this name";
            //}
            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUserView(string viewId, string viewPref)
        {
            var userView = new UserViewDto
            {
                UserID = _authUser.UserId,
                ClientID = _authUser.ClientId,
                UserViewID = Int32.Parse(viewId),
                ViewName = "Default",
                View = viewPref
            };

            var nList = _userBl.UpdateUserView(userView);

            var userAdded = new UserHandlerModel();
            if (nList > 0)
            {
                userAdded.MgUserId = nList;
                userAdded.StrMessage = "Updated";
            }
            else if (nList == 0)
            {
                userAdded.MgUserId = nList;
                userAdded.StrMessage = "Error";
            }

            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the user views.
        /// </summary>
        /// <param name="uvList">The uv list.</param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult DeleteUserViews(string uvList)
        {
            try
            {
                var userDeleted = new UserHandlerModel();

                var nList = _userBl.DeleteUserViews(uvList);
                if (nList > 0)
                {
                    userDeleted.StrMessage = "Ids:" + uvList + " were deleted successfully";
                    userDeleted.MgUserId = 1;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Opportunities
        /// <summary>
        /// Opportunities management page.
        /// </summary>
        /// <returns></returns>
        public ActionResult OpportunityManagement()
        {
            if (Session["AuthenticatedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        /// <summary>
        /// Get if the client can manage opportunitities in CRM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ManageOpportunitiesInCRM()
        {
            var manageOpportunitiesInCRM = _clientLoginBL.ManageOpportunitiesInCRM(_authUser.ClientId);
            return Json(manageOpportunitiesInCRM, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a opportunity row.
        /// </summary>
        /// <param name="opportunity">The opportunity.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOpportunityRow(OpportunityModel opportunity)
        {
            Mapper.CreateMap<OpportunityModel, OpportunityDto>()
                  .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                  .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                  .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                  .ForMember(dest => dest.OppStatus, opt => opt.MapFrom(src => src.OppStatusId))
                  .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                  .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                  .ForMember(dest => dest.SDALastUpdDT, opt => opt.Ignore())
                  .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                  .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                  .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                  .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
                  .ForMember(dest => dest.Quotes, opt => opt.Ignore());

            var opportunityDto = Mapper.Map<OpportunityModel, OpportunityDto>(opportunity);
            //Mapper.AssertConfigurationIsValid();

            opportunityDto.ClientID = _authUser.ClientId;
            opportunityDto.CloudLastUpdById = _authUser.UserId;
            opportunityDto.CloudLastUpdDT = DateTime.Today.ToString(CultureInfo.InvariantCulture);
            opportunityDto.SDALastUpdBy = 0;
            opportunityDto.SDALastUpdDT = DateTime.Today.ToString(CultureInfo.InvariantCulture);
            var nList = _opportunityBl.AddOpportunity(opportunityDto);
            var userAdded = new UserHandlerModel();
            if (nList > 0)
            {
                var newOpp = _opportunityBl.GetNonDeletedOpportunityByClientIDAndOppID(_authUser.ClientId, nList);
                userAdded.MgUserId = nList;
                userAdded.StrMessage = newOpp.CRMOppID;
            }
            else
            {
                userAdded.StrMessage = opportunityDto.ToString();
            }

            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes a opportunity row.
        /// </summary>
        /// <param name="opportunityId">The opportunity identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult DeleteOpportunityRow(int opportunityId)
        {
            try
            {
                var userDeleted = new UserHandlerModel();
                var nList = _opportunityBl.DeleteOpportunity(opportunityId);
                if (nList > 0)
                {
                    userDeleted.StrMessage = "Id:" + opportunityId + " was deleted successfully";
                    userDeleted.MgUserId = 1;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Loads the default probability by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadDefaultProbabilityByStatus(int status)
        {
            var probability = _opportunityBl.GetDefaultProbabilityByStatus(_authUser.ClientId, status);

            return Json(probability, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates a opportunity row.
        /// </summary>
        /// <param name="opportunity">The opportunity.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateOpportunityRow(OpportunityModel opportunity)
        {
            try
            {
                Mapper.CreateMap<OpportunityModel, OpportunityDto>()
                      .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                      .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                      .ForMember(dest => dest.CloudLastUpdById, opt => opt.Ignore())
                      .ForMember(dest => dest.OppStatus, opt => opt.MapFrom(src => src.OppStatusId))
                      .ForMember(dest => dest.QuoteIDMainSite, opt => opt.Ignore())
                      .ForMember(dest => dest.SDALastUpdBy, opt => opt.Ignore())
                      .ForMember(dest => dest.LoginInfo, opt => opt.Ignore())
                      .ForMember(dest => dest.OpportunityTable, opt => opt.Ignore())
                      .ForMember(dest => dest.CRMXrefDefinition, opt => opt.Ignore())
                      .ForMember(dest => dest.CreateBy, opt => opt.Ignore())
                      .ForMember(dest => dest.Quotes, opt => opt.Ignore());
                var opportunityDto = Mapper.Map<OpportunityModel, OpportunityDto>(opportunity);
                //Mapper.AssertConfigurationIsValid();

                opportunityDto.CloudLastUpdById = _authUser.UserId;
                opportunityDto.ClientID = _authUser.ClientId;

                var nList = _opportunityBl.UpdateOpportunityById(opportunityDto);
                var userEdit = new UserHandlerModel();
                if (nList > 0)
                {
                    userEdit.MgUserId = 1;
                    userEdit.StrMessage = "Updated";
                }

                return Json(userEdit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Opportunities data grid info.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult OpportunityDataGrid()
        {

            ///////////////////////////////////////////////////////////////////
            //First get users list
            var liUsers = new List<UserDto>();

            if (_authUser.ManagerUserID.HasValue && _authUser.ManagerUserID.Value != 0)
            {
                var userWManagers = _userBl.GetUsersWithManager(_authUser.ClientId);

                var tUser = userWManagers.SingleOrDefault(u => u.UserId == _authUser.UserId);
                liUsers.Add(tUser);

                var firstLevel = userWManagers.Where(u => u.ManagerUserID == _authUser.UserId).ToList();
                if (firstLevel.Count > 0)
                {

                    liUsers.AddRange(firstLevel);
                    // userWManagers.RemoveAll(x => !liUsers.Any(y => y.UserId == x.UserId));

                    Utils.GetUsers(userWManagers, liUsers);
                }
            }
            else
            {
                liUsers = _userBl.GetUsers(_authUser.ClientId);
            }
            ////////////////////////////////////////////////////////////////////


            Mapper.CreateMap<UserDto, UserModel>()
                        .ForMember(dest => dest.ManagerUserID, opt => opt.MapFrom(src => src.ManagerUserID ?? 0))
                        .ForMember(dest => dest.ManagerUserName, opt => opt.MapFrom(src => "-------"));
            var peopleVm = Mapper.Map<List<UserDto>, List<UserModel>>(liUsers);
            //Mapper.AssertConfigurationIsValid();

            foreach (var userModel in peopleVm.Where(userModel => userModel.ManagerUserID != null))
            {
                userModel.ManagerUserName =
                    liUsers.Where(u => u.UserId == userModel.ManagerUserID)
                         .Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();
            }

            //Get opportunities for specific client
            var nList = _opportunityBl.GetOpportunities(_authUser.ClientId);

            //Get opportunity statuses for specific client
            var statuses = _utilityBL.GetStatuses(_authUser.ClientId);

            var displayOpps = (from opportunityDto in nList
                               join stat in statuses on opportunityDto.OppStatus ?? 0 equals stat.ID
                               join user in peopleVm on opportunityDto.OppOwner equals user.UserId
                               select new OpportunityModel
                               {
                                   CRMOppID = opportunityDto.CRMOppID,
                                   ClientID = opportunityDto.ClientID,
                                   CloudLastUpdDT = opportunityDto.CloudLastUpdDT,
                                   CompanyName = opportunityDto.CompanyName,
                                   NumofQuotes = opportunityDto.NumofQuotes,
                                   OppCloseDate = opportunityDto.OppCloseDate,
                                   OppID = opportunityDto.OppID,
                                   OppName = opportunityDto.OppName,
                                   OppProbability = opportunityDto.OppProbability,
                                   OppStatusId = stat.ID,
                                   OppStatusName = stat.OppStatus,
                                   StageType = stat.StageType,
                                   SDALastUpdDT = opportunityDto.SDALastUpdDT,
                                   OppOwner = opportunityDto.OppOwner,
                                   OppOwnerName = user.FirstName + " " + user.LastName,
                                   Manager = user.ManagerUserName,
                                   QuotedAmount = opportunityDto.QuotedAmount,
                                   QuotedCost = opportunityDto.QuotedCost,
                                   QuotedMargin = opportunityDto.QuotedMargin,
                                   CreateDT = opportunityDto.CreateDT,
                                   ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1,
                                   ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2,
                                   ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3,
                                   ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4,
                                   ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5,
                                   ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6,
                                   ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7,
                                   ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8,
                                   ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9,
                                   ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10,
                                   ClientDefinedText1 = opportunityDto.ClientDefinedText1,
                                   ClientDefinedText2 = opportunityDto.ClientDefinedText2,
                                   ClientDefinedText3 = opportunityDto.ClientDefinedText3,
                                   ClientDefinedText4 = opportunityDto.ClientDefinedText4,
                                   ClientDefinedText5 = opportunityDto.ClientDefinedText5
                               });
            return Json(displayOpps, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OpportunityDataGridfromView()
        {

            var liUsers = new List<UserDto>();

            if (_authUser.ManagerUserID.HasValue && _authUser.ManagerUserID.Value != 0)
            {
                var userWManagers = _userBl.GetUsersWithManager(_authUser.ClientId);

                var tUser = userWManagers.SingleOrDefault(u => u.UserId == _authUser.UserId);
                liUsers.Add(tUser);

                var firstLevel = userWManagers.Where(u => u.ManagerUserID == _authUser.UserId).ToList();
                if (firstLevel.Count > 0)
                {

                    liUsers.AddRange(firstLevel);
                    // userWManagers.RemoveAll(x => !liUsers.Any(y => y.UserId == x.UserId));

                    Utils.GetUsers(userWManagers, liUsers);
                }
            }
            else
            {
                liUsers = _userBl.GetUsers(_authUser.ClientId);
            }

            List<int> liUserIds = liUsers.Select(c => c.UserId).ToList();

            //Get opportunities for specific client
            var nList = _opportunityBl.GetOpportunitiesfromView(_authUser.ClientId, _authUser.UserId, DateTime.MinValue, DateTime.MaxValue, "Open", liUserIds);

            var displayOpps = (from opportunityDto in nList
                               select new OpportunityModel
                               {
                                   CRMOppID = opportunityDto.CRMOppID,
                                   ClientID = opportunityDto.ClientID,
                                   CloudLastUpdDT = opportunityDto.CloudLastUpdDT,
                                   CompanyName = opportunityDto.CompanyName,
                                   NumofQuotes = opportunityDto.NumofQuotes,
                                   OppCloseDate = opportunityDto.OppCloseDate,
                                   OppID = opportunityDto.OppID,
                                   OppName = opportunityDto.OppName,
                                   OppProbability = opportunityDto.OppProbability,
                                   OppStatusId = opportunityDto.OppStatusId,
                                   OppStatusName = opportunityDto.OppStatusName,
                                   StageType = opportunityDto.StageType,
                                   SDALastUpdDT = opportunityDto.SDALastUpdDT,
                                   OppOwner = opportunityDto.OppOwner,
                                   OppOwnerName = opportunityDto.OppOwnerName,
                                   Manager = opportunityDto.Manager,
                                   QuotedAmount = opportunityDto.QuotedAmount,
                                   QuotedCost = opportunityDto.QuotedCost,
                                   QuotedMargin = opportunityDto.QuotedMargin,
                                   CreateDT = opportunityDto.CreateDT,
                                   ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1,
                                   ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2,
                                   ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3,
                                   ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4,
                                   ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5,
                                   ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6,
                                   ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7,
                                   ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8,
                                   ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9,
                                   ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10,
                                   ClientDefinedText1 = opportunityDto.ClientDefinedText1,
                                   ClientDefinedText2 = opportunityDto.ClientDefinedText2,
                                   ClientDefinedText3 = opportunityDto.ClientDefinedText3,
                                   ClientDefinedText4 = opportunityDto.ClientDefinedText4,
                                   ClientDefinedText5 = opportunityDto.ClientDefinedText5
                               });

            return Json(displayOpps, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OpportunityDataByDateRange(DateTime initialDate, DateTime finalDate, string stages)
        {
            var liUsers = new List<UserDto>();

            if (_authUser.ManagerUserID.HasValue && _authUser.ManagerUserID.Value != 0)
            {
                var userWManagers = _userBl.GetUsersWithManager(_authUser.ClientId);

                var tUser = userWManagers.SingleOrDefault(u => u.UserId == _authUser.UserId);
                liUsers.Add(tUser);

                var firstLevel = userWManagers.Where(u => u.ManagerUserID == _authUser.UserId).ToList();
                if (firstLevel.Count > 0)
                {
                    liUsers.AddRange(firstLevel);
                    // userWManagers.RemoveAll(x => !liUsers.Any(y => y.UserId == x.UserId));
                    Utils.GetUsers(userWManagers, liUsers);
                }
            }
            else
            {
                liUsers = _userBl.GetUsers(_authUser.ClientId);
            }
            List<int> liUserIds = liUsers.Select(c => c.UserId).ToList();

            initialDate = initialDate.Date;
            finalDate = finalDate.AddDays(1).Date;

            //Get opportunities for specific client
            var nList = _opportunityBl.GetOpportunitiesfromView(_authUser.ClientId, _authUser.UserId, initialDate, finalDate, stages, liUserIds);

            //where DateTime.Parse(opportunityDto.OppCloseDate) >= initialDate && DateTime.Parse(opportunityDto.OppCloseDate) <= finalDate
            var displayOpps = (from opportunityDto in nList
                               select new OpportunityModel
                               {
                                   CRMOppID = opportunityDto.CRMOppID,
                                   ClientID = opportunityDto.ClientID,
                                   CloudLastUpdDT = opportunityDto.CloudLastUpdDT,
                                   CompanyName = opportunityDto.CompanyName,
                                   NumofQuotes = opportunityDto.NumofQuotes,
                                   OppCloseDate = (!string.IsNullOrEmpty(opportunityDto.OppCloseDate) ? Convert.ToDateTime(opportunityDto.OppCloseDate).ToString("yyyy-MM-dd") : string.Empty),
                                   OppID = opportunityDto.OppID,
                                   OppName = opportunityDto.OppName,
                                   OppProbability = opportunityDto.OppProbability,
                                   OppStatusId = opportunityDto.OppStatusId,
                                   OppStatusName = opportunityDto.OppStatusName,
                                   StageType = opportunityDto.StageType,
                                   SDALastUpdDT = opportunityDto.SDALastUpdDT,
                                   OppOwner = opportunityDto.OppOwner,
                                   OppOwnerName = opportunityDto.OppOwnerName,
                                   Manager = opportunityDto.Manager,
                                   QuotedAmount = opportunityDto.QuotedAmount,
                                   QuotedCost = opportunityDto.QuotedCost,
                                   QuotedMargin = opportunityDto.QuotedMargin,
                                   CreateDT = (!string.IsNullOrEmpty(opportunityDto.CreateDT) ? Convert.ToDateTime(opportunityDto.CreateDT).ToString() : string.Empty),
                                   ClientDefinedTotal1 = opportunityDto.ClientDefinedTotal1,
                                   ClientDefinedTotal2 = opportunityDto.ClientDefinedTotal2,
                                   ClientDefinedTotal3 = opportunityDto.ClientDefinedTotal3,
                                   ClientDefinedTotal4 = opportunityDto.ClientDefinedTotal4,
                                   ClientDefinedTotal5 = opportunityDto.ClientDefinedTotal5,
                                   ClientDefinedTotal6 = opportunityDto.ClientDefinedTotal6,
                                   ClientDefinedTotal7 = opportunityDto.ClientDefinedTotal7,
                                   ClientDefinedTotal8 = opportunityDto.ClientDefinedTotal8,
                                   ClientDefinedTotal9 = opportunityDto.ClientDefinedTotal9,
                                   ClientDefinedTotal10 = opportunityDto.ClientDefinedTotal10,
                                   ClientDefinedText1 = opportunityDto.ClientDefinedText1,
                                   ClientDefinedText2 = opportunityDto.ClientDefinedText2,
                                   ClientDefinedText3 = opportunityDto.ClientDefinedText3,
                                   ClientDefinedText4 = opportunityDto.ClientDefinedText4,
                                   ClientDefinedText5 = opportunityDto.ClientDefinedText5
                               });
            return Json(displayOpps, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExportGrid(string gridData, string fileName)
        {
            if (!fileName.Contains("SalesManagerExport"))
            {
                string result = string.Empty;
                if (gridData != null)
                {
                    string[] lines = gridData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    var cleanText = lines[1].Replace("\"", "");
                    var fileDownloadName = "";
                    var fileCloudId = "";
                    var quote = _quoteBl.GetQuoteByLastFileSavedLocation(cleanText);
                    try
                    {
                        fileDownloadName = Path.GetFileName(cleanText);
                        fileCloudId = quote.FilePlatformFileID;
                    }
                    catch (Exception) { }
                    string csvFile = "\"Last File Saved Location\",\"File Name\",\"File Platform File ID\",\"QuoteID\",";
                    csvFile += " \n"; //jump line in the cvs file
                    if (!String.IsNullOrEmpty(csvFile))
                    {
                        if (cleanText.ToString().Contains("sharepoint"))
                        {
                            csvFile += "\"[" + cleanText + "]\",\"" + fileDownloadName + "\",\"" + fileCloudId + "\",\"" + quote.QuoteID + "\"";
                        }
                        else
                        {
                            csvFile += "\"" + cleanText + "\",\"" + fileDownloadName + "\",\"" + "" + "\",\"" + quote.QuoteID + "\"";
                        }
                    }
                    string path = Server.MapPath("~/Files/" + fileName + ".csv");
                    System.IO.File.WriteAllText(path, csvFile);
                    result = "/Files/" + fileName + ".csv";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string result = string.Empty;
                if (gridData != null)
                {
                    string path = Server.MapPath("~/Files/" + fileName + ".csv");
                    System.IO.File.WriteAllText(path, gridData);
                    result = "/Files/" + fileName + ".csv";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Opportunities reassign data grid info.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ReassignOpportunityDataGrid()
        {
            var liUsers = new List<UserDto>();

            if (_authUser.ManagerUserID.HasValue && _authUser.ManagerUserID.Value != 0)
            {
                var userWManagers = _userBl.GetUsersWithManager(_authUser.ClientId);

                var tUser = userWManagers.SingleOrDefault(u => u.UserId == _authUser.UserId);
                liUsers.Add(tUser);

                var firstLevel = userWManagers.Where(u => u.ManagerUserID == _authUser.UserId).ToList();
                if (firstLevel.Count > 0)
                {

                    liUsers.AddRange(firstLevel);
                    // userWManagers.RemoveAll(x => !liUsers.Any(y => y.UserId == x.UserId));

                    Utils.GetUsers(userWManagers, liUsers);
                }
            }
            else
            {
                liUsers = _userBl.GetUsers(_authUser.ClientId);
            }
            ////////////////////////////////////////////////////////////////////


            Mapper.CreateMap<UserDto, UserModel>()
                        .ForMember(dest => dest.ManagerUserID, opt => opt.MapFrom(src => src.ManagerUserID ?? 0))
                        .ForMember(dest => dest.ManagerUserName, opt => opt.MapFrom(src => "-------"));
            var peopleVm = Mapper.Map<List<UserDto>, List<UserModel>>(liUsers);
            //Mapper.AssertConfigurationIsValid();

            foreach (var userModel in peopleVm.Where(userModel => userModel.ManagerUserID != null))
            {
                userModel.ManagerUserName =
                    liUsers.Where(u => u.UserId == userModel.ManagerUserID)
                         .Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();
            }

            var nList = _opportunityBl.GetNonClosedOpportunities(_authUser.ClientId);
            var statuses = _utilityBL.GetStatuses(_authUser.ClientId);

            var displayOpps = (from opportunityDto in nList
                               join stat in statuses on opportunityDto.OppStatus equals stat.ID
                               join user in peopleVm on opportunityDto.OppOwner equals user.UserId
                               select new OpportunityModel
                               {
                                   CRMOppID = opportunityDto.CRMOppID,
                                   ClientID = opportunityDto.ClientID,
                                   CloudLastUpdDT = opportunityDto.CloudLastUpdDT,
                                   CompanyName = opportunityDto.CompanyName,
                                   NumofQuotes = opportunityDto.NumofQuotes,
                                   OppCloseDate = opportunityDto.OppCloseDate,
                                   OppID = opportunityDto.OppID,
                                   OppName = opportunityDto.OppName,
                                   OppProbability = opportunityDto.OppProbability,
                                   OppStatusId = stat.ID,
                                   OppStatusName = stat.OppStatus,
                                   SDALastUpdDT = opportunityDto.SDALastUpdDT,
                                   OppOwner = opportunityDto.OppOwner,
                                   OppOwnerName = user.FirstName + " " + user.LastName,
                                   Manager = user.ManagerUserName,
                                   QuotedAmount = opportunityDto.QuotedAmount,
                                   QuotedCost = opportunityDto.QuotedCost,
                                   QuotedMargin = opportunityDto.QuotedMargin
                               });

            return Json(displayOpps, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Quotes
        /// <summary>
        /// Get quotes for an opportunity
        /// </summary>
        /// <param name="opportunityID">The opportunity ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult QuoteDataGrid(int opportunityID)
        {
            var nList = _quoteBl.GetQuotes(_authUser.ClientId, opportunityID);
            var displayOpps = (from quoteDto in nList
                               select new QuoteModel
                               {
                                   QuoteID = quoteDto.QuoteID,
                                   OppID = quoteDto.OppID,
                                   QuoteSiteDescription = quoteDto.QuoteSiteDesc,
                                   Rollup = quoteDto.Rollup,
                                   QuotedAmount = quoteDto.QuotedAmount,
                                   QuotedCost = quoteDto.QuotedCost,
                                   QuotedMargin = quoteDto.QuoteMargin,
                                   SDALastUpdBy = quoteDto.SDALastUpdBy,
                                   SDALastUpdByName = quoteDto.SDAUpdatedByName,
                                   LastFileSavedLocation = quoteDto.LastFileSavedLocation,
                                   ClientDefinedTotal1 = quoteDto.ClientDefinedTotal1,
                                   ClientDefinedTotal2 = quoteDto.ClientDefinedTotal2,
                                   ClientDefinedTotal3 = quoteDto.ClientDefinedTotal3,
                                   ClientDefinedTotal4 = quoteDto.ClientDefinedTotal4,
                                   ClientDefinedTotal5 = quoteDto.ClientDefinedTotal5,
                                   ClientDefinedTotal6 = quoteDto.ClientDefinedTotal6,
                                   ClientDefinedTotal7 = quoteDto.ClientDefinedTotal7,
                                   ClientDefinedTotal8 = quoteDto.ClientDefinedTotal8,
                                   ClientDefinedTotal9 = quoteDto.ClientDefinedTotal9,
                                   ClientDefinedTotal10 = quoteDto.ClientDefinedTotal10,
                                   ClientDefinedText1 = quoteDto.ClientDefinedText1,
                                   ClientDefinedText2 = quoteDto.ClientDefinedText2,
                                   ClientDefinedText3 = quoteDto.ClientDefinedText3,
                                   ClientDefinedText4 = quoteDto.ClientDefinedText4,
                                   ClientDefinedText5 = quoteDto.ClientDefinedText5
                               });

            return Json(displayOpps, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes a quote row.
        /// </summary>
        /// <param name="quoteIdList">The quote identifier list.</param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult DeleteQuotes(string quoteIdList)
        {
            try
            {
                var userDeleted = new UserHandlerModel();

                var nList = _quoteBl.DeleteQuotes(quoteIdList);
                if (nList > 0)
                {
                    userDeleted.StrMessage = "Ids:" + quoteIdList + " were deleted successfully";
                    userDeleted.MgUserId = 1;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Reassign some quotes to an another opportunity
        /// </summary>
        /// <param name="quoteIdList">The quoteID's list to reassign</param>
        /// <param name="newOpportunityID">The new opportunity identifier.</param>
        /// <param name="newCRMOppID">The new CRM opp identifier.</param>
        /// <returns></returns>
        public JsonResult ReassignQuotes(string quoteIdList, int newOpportunityID, string newCRMOppID)
        {
            try
            {
                var userDeleted = new UserHandlerModel();

                var nList = _quoteBl.ReassignQuotes(_authUser.ClientId, quoteIdList, newOpportunityID, newCRMOppID);
                if (nList > 0)
                {
                    userDeleted.StrMessage = "Ids:" + quoteIdList + " were reassign successfully";
                    userDeleted.MgUserId = 1;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates a quote row.
        /// </summary>
        /// <param name="opportunityID">The opportunity identifier.</param>
        /// <param name="quoteID">The quote identifier.</param>
        /// <param name="rollup">The rollup.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateQuoteRollup(int opportunityID, string quoteID, string rollup)
        {
            try
            {
                var nList = _quoteBl.UpdateQuoteRollup(opportunityID, quoteID, rollup);
                var userEdit = new UserHandlerModel();
                if (nList > 0)
                {
                    userEdit.MgUserId = 1;
                    userEdit.StrMessage = "Updated";
                }

                return Json(userEdit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}
