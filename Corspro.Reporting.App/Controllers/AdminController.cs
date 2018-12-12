using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AutoMapper;
using Corspro.Business.External;
using Corspro.Domain.Dto;
using Corspro.Reporting.App.Models;
using System.Net.Mail;


namespace Corspro.Reporting.App.Controllers
{
    public class AdminController : Controller
    {
        private UserDto _authUser;
        private readonly UserBL _userBl;
        private readonly OpportunityBL _opportunityBl;
        private readonly QuoteBL _quoteBl;
        private readonly ClientLoginBL _clientLoginBL;
        private readonly ClientDefinedFieldBL _clientDefinedFieldBL;
        private readonly UtilityBL _utilityBL;
        private readonly OppStatusBL _oppStatusBL;
        private List<UserDto> allManagers;
        private List<UserDto> userlist;
        private List<UserDto> usersRemovelist;

        // Define default min and max password lengths.
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "23456789";
        private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        public AdminController()
        {
            _userBl = new UserBL();
            _opportunityBl = new OpportunityBL();
            _quoteBl = new QuoteBL();
            _clientLoginBL = new ClientLoginBL();
            _clientDefinedFieldBL = new ClientDefinedFieldBL();
            _utilityBL = new UtilityBL();
            _oppStatusBL = new OppStatusBL();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["AuthenticatedUser"] != null)
                _authUser = (UserDto)Session["AuthenticatedUser"];
        }

        #region Users
        /// <summary>
        /// Users management.
        /// </summary>
        /// <returns></returns>
        public ActionResult UserManagement()
        {
            if (Session["AuthenticatedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        /// <summary>
        /// Users data grid info.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UserDataGrid()
        {
            var nList = _userBl.GetActiveUsers(_authUser.ClientId);
            Mapper.CreateMap<UserDto, UserModel>()
                .ForMember(dest => dest.ManagerUserID, opt => opt.MapFrom(src => src.ManagerUserID ?? 0))
                .ForMember(dest => dest.ManagerUserName, opt => opt.MapFrom(src => "-------"));
            var peopleVm = Mapper.Map<List<UserDto>, List<UserModel>>(nList);
            //Mapper.AssertConfigurationIsValid();
            foreach (var userModel in peopleVm.Where(userModel => userModel.ManagerUserID != 0))
            {
                userModel.ManagerUserName =
                    nList.Where(u => u.UserId == userModel.ManagerUserID)
                         .Select(u => u.FirstName + " " + u.LastName).FirstOrDefault();
            }

            return Json(peopleVm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// List the Users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UserList()
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
                liUsers = _userBl.GetActiveUsers(_authUser.ClientId);
            }

            var list = new List<UserHandlerModel>();
            list.AddRange(liUsers.Select(user => new UserHandlerModel { StrMessage = user.FirstName + " " + user.LastName, MgUserId = user.UserId }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// List the managers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Managers()
        {
            var nList = _userBl.GetActiveUsers(_authUser.ClientId);

            var list = new List<UserHandlerModel> { new UserHandlerModel { StrMessage = "-------", MgUserId = 0 } };
            list.AddRange(nList.Select(user => new UserHandlerModel { StrMessage = user.FirstName + " " + user.LastName, MgUserId = user.UserId }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">
        /// Minimum password length.
        /// </param>
        /// <param name="maxLength">
        /// Maximum password length.
        /// </param>
        /// <returns>
        /// Randomly generated password.
        /// </returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        [HttpGet]
        public JsonResult Generate(int minLength,
                                  int maxLength)
        {
            // Make sure that input parameters are valid.
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            // Create a local array containing supported password characters
            // grouped by types. You can remove character groups from this
            // array, but doing so will weaken the password strength.
            char[][] charGroups = new char[][] 
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            PASSWORD_CHARS_SPECIAL.ToCharArray()
        };

            // Use this array to track the number of unused characters in each
            // character group.
            int[] charsLeftInGroup = new int[charGroups.Length];

            // Initially, all characters in each group are not used.
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            // Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            // Because we cannot use the default randomizer, which is based on the
            // current time (it will produce the same "random" number within a
            // second), we will use a random number generator to seed the
            // randomizer.

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            //var rng = Encoding.Unicode.GetBytes(plainText);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            // This array will hold password characters.
            char[] password = null;

            // Allocate appropriate memory for the password.
            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            // Index of the next character to be added to password.
            int nextCharIdx;

            // Index of the next character group to be processed.
            int nextGroupIdx;

            // Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            // Index of the last non-processed character in a group.
            int lastCharIdx;

            // Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // Generate password characters one at a time.
            for (int i = 0; i < password.Length; i++)
            {
                // If only one character group remained unprocessed, process it;
                // otherwise, pick a random character group from the unprocessed
                // group list. To allow a special character to appear in the
                // first position, increment the second parameter of the Next
                // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which we will
                // pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                // Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                // If only one unprocessed character is left, pick it; otherwise,
                // get a random character from the unused character list.
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                // If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // There are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            var generatedPwd = new string(password);

            return Json(generatedPwd, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SendEmail()
        {
            string error = "None";
            try
            {
                MailMessage mail = new MailMessage("support@corspro.com", "mfernandez@isthmusit.com");
                SmtpClient client = new SmtpClient("smtp.corspro.com", 25);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                mail.Subject = "this is a test email.";
                mail.Body = "this is my test email body";
                client.Send(mail);
            }
            catch (Exception e)
            {
                error = e.Message;
                if (e.InnerException != null)
                {
                    error += e.InnerException.Message;
                }
            }
            return Json(error, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetManagersHierarchy(UserDto userDto)
        {
            usersRemovelist = new List<UserDto>();
            usersRemovelist.Add(userDto);
            var usersToDisplay = new List<UserDto>();


            // Get Active Users for the specific Client
            userlist = _userBl.GetActiveUsers(_authUser.ClientId);

            //Get the possible users to assign as a manager
            getManagersByUser(usersRemovelist);

            allManagers = _userBl.GetManagers(_authUser);

            if (usersRemovelist.Count > 0)
            {
                foreach (var man in allManagers)
                {
                    var remove = false;
                    foreach (var rem in usersRemovelist)
                    {
                        if (man.UserId == rem.UserId)
                        {
                            remove = true;
                            break;
                        }
                    }
                    if (!remove)
                    {
                        usersToDisplay.Add(man);
                    }
                }
            }
            else
            {
                usersToDisplay = allManagers;
            }

            var list = new List<UserHandlerModel> { new UserHandlerModel { StrMessage = "-------", MgUserId = 0 } };
            list.AddRange(usersToDisplay.Select(user => new UserHandlerModel { StrMessage = user.FirstName + " " + user.LastName, MgUserId = user.UserId }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private void getManagersByUser(List<UserDto> users)
        {
            var tempusers = new List<UserDto>();
            if (users.Count > 0)
            {
                foreach (var val in users)
                {
                    foreach (var man in userlist)
                    {
                        if (man.ManagerUserID == val.UserId)
                        {
                            tempusers.Add(man);
                            usersRemovelist.Add(man);
                            break;
                        }
                    }
                }
                getManagersByUser(tempusers);
            }
        }

        /// <summary>
        /// Deletes a user row.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        public JsonResult DeleteUserRows(string userIdList)
        {
            try
            {
                var userDeleted = new UserHandlerModel { MgUserId = 0, StrMessage = "There was an error" };

                var nList = _userBl.DeleteUsers(userIdList);

                userDeleted.StrMessage = nList;
                if (!string.IsNullOrEmpty(nList)) userDeleted.MgUserId = 1;

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates a user row.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateUserRow(UserDto user)
        {
            if (user.ManagerUserID == 0)
            {
                user.ManagerUserID = null;
            }
            user.CloudLastUpdBy = _authUser.UserId;

            var nList = _userBl.UpdateUser(user);
            var message = "There was an error";
            if (nList > 0)
            {
                message = "Id:" + user.UserId + " was updated successfully";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the user field.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="field">The field.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateUserField(int userId, string field, string newValue)
        {
            try
            {
                if (field.Equals("Administrator"))
                {
                    if (newValue.ToUpper().Equals("TRUE"))
                    { newValue = "Y"; }
                    else
                    { newValue = "N"; }
                }
                var nList = _userBl.UpdateUserField(userId, field, newValue);
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

        [HttpPut]
        public JsonResult SetRequestedEmail(string userIdList)
        {
            try
            {
                var requested = new UserHandlerModel { MgUserId = 0, StrMessage = "There was an error" };

                var nList = _userBl.UpdateUserEmailSent(userIdList);

                requested.StrMessage = nList;
                if (!string.IsNullOrEmpty(nList)) requested.MgUserId = 1;

                return Json(requested, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Updates the user manager.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateUserManager(UserDto user)
        {
            try
            {
                var userEdit = new UserHandlerModel();
                if (user.ManagerUserID != user.UserId)
                {
                    var nList = _userBl.UpdateUserManager(user.UserId, user.ManagerUserID.Value);
                    if (nList > 0)
                    {
                        userEdit.MgUserId = 1;
                        userEdit.StrMessage = "Updated";
                    }
                }
                else
                {
                    userEdit.MgUserId = 0;
                    userEdit.StrMessage = "Error: You cannot be your own manager";
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
        /// Resets the password for a specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult ResetPasswordRow(UserDto user)
        {
            //var tempPwd = Membership.GeneratePassword(8, 1);
            var tempPwd = user.Password;
            user.Password = Utils.Encrypt(tempPwd);
            user.CloudLastUpdBy = _authUser.UserId;
            var nList = _userBl.ResetPassword(user);
            var message = "There was an error";
            if (nList > 0)
            {
                message = "The password change for Id:" + user.UserId + " was successful ";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds a user row.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddUserRow(UserDto user)
        {
            user.ClientId = _authUser.ClientId;
            var tempPwd = user.Password;
            user.Password = Utils.Encrypt(tempPwd);
            if (user.ManagerUserID == 0)
            {
                user.ManagerUserID = null;
            }
            user.CloudLastUpdBy = _authUser.UserId;
            var nList = _userBl.AddUser(user);
            var userAdded = new UserHandlerModel { StrMessage = "There was an error" };

            userAdded.MgUserId = nList;
            userAdded.StrMessage = tempPwd;

            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Clients the defined field MGMT.
        /// </summary>
        /// <returns></returns>
        public ActionResult ClientDefinedFieldMgmt()
        {
            if (Session["AuthenticatedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetClientDefinedFields(string table)
        {
            var nList = _clientDefinedFieldBL.GetClientDefinedFields(_authUser.ClientId, table);

            foreach (var cdf in nList)
            {
                if (string.IsNullOrEmpty(cdf.Format))
                {
                    cdf.Format = string.Empty;
                }
                else
                {
                    if (cdf.Format.ToUpper().Equals("CURRENCY"))
                    {
                        cdf.Format = "c2";
                    }
                    else if (cdf.Format.ToUpper().Equals("NUMBER"))
                    {
                        cdf.Format = "f2";
                    }
                    else
                    {
                        cdf.Format = string.Empty;
                    }
                }
            }

            Mapper.CreateMap<ClientDefinedFieldDto, ClientDefinedFieldModel>();
            var list = Mapper.Map<List<ClientDefinedFieldDto>, List<ClientDefinedFieldModel>>(nList);
            //Mapper.AssertConfigurationIsValid();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the client defined fields.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ClientDefinedFieldList()
        {
            var nList = _clientDefinedFieldBL.GetClientDefinedFields(_authUser.ClientId);

            Mapper.CreateMap<ClientDefinedFieldDto, ClientDefinedFieldModel>();
            var list = Mapper.Map<List<ClientDefinedFieldDto>, List<ClientDefinedFieldModel>>(nList);
            //Mapper.AssertConfigurationIsValid();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Opportunity statuses.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult OppStatuses()
        {
            var nList = _oppStatusBL.GetOppStatuses(_authUser.ClientId);

            var list = new List<UserHandlerModel>();
            list.AddRange(nList.Select(user => new UserHandlerModel { StrMessage = user.OppStatus, MgUserId = user.ID }));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        public JsonResult UpdateCDFField(int cdfId, string field, string newValue)
        {
            try
            {
                bool isValid = true;
                if (field.Equals("Table") || field.Equals("Field"))
                {
                    isValid = _clientDefinedFieldBL.ValidateFieldToUpdate(_authUser.ClientId, cdfId, field, newValue);
                }

                var userEdit = new UserHandlerModel();
                if (isValid)
                {
                    var nList = _clientDefinedFieldBL.UpdateCDFField(cdfId, field, newValue);
                    if (nList > 0)
                    {
                        userEdit.MgUserId = 1;
                        userEdit.StrMessage = "Updated";
                    }
                }
                else
                {
                    userEdit.MgUserId = -2;
                    userEdit.StrMessage = "Error";
                }

                return Json(userEdit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = e.Message };

                if (e.InnerException != null)
                {
                    error.StrMessage += e.InnerException.Message;
                }

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddClientDefinedField(ClientDefinedFieldModel cdfModel)
        {
            Mapper.CreateMap<ClientDefinedFieldModel, ClientDefinedFieldDto>();

            var cdfDto = Mapper.Map<ClientDefinedFieldModel, ClientDefinedFieldDto>(cdfModel);
            //Mapper.AssertConfigurationIsValid();

            cdfDto.ClientID = _authUser.ClientId;

            var cdfs = _clientDefinedFieldBL.GetClientDefinedFields(cdfDto.ClientID, cdfDto.Table, cdfDto.Field);
            var userAdded = new UserHandlerModel();
            if (cdfs.Count > 0)
            {
                userAdded.MgUserId = -2;
                userAdded.StrMessage = "Error";
            }
            else
            {
                var nList = _clientDefinedFieldBL.AddClientDefinedField(cdfDto);

                if (nList > 0)
                {
                    userAdded.MgUserId = nList;
                    userAdded.StrMessage = "Added";
                }
            }

            return Json(userAdded, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public JsonResult DeleteCDFs(string cdfIdList)
        {
            try
            {
                var userDeleted = new UserHandlerModel { MgUserId = 1, StrMessage = "Successful" };

                var nList = _clientDefinedFieldBL.DeleteCDFs(cdfIdList);

                if (!string.IsNullOrEmpty(nList))
                {
                    userDeleted.StrMessage = nList;
                    userDeleted.MgUserId = 2;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OppStatusManagement()
        {
            if (Session["AuthenticatedUser"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public JsonResult OppStatusList()
        {
            var nList = _oppStatusBL.GetOppStatuses(_authUser.ClientId);

            Mapper.CreateMap<OppStatusDto, OppStatusModel>()
                .ForMember(dest => dest.DefaultB, opt => opt.ResolveUsing(src => src.Default.ToUpper() == "Y"));
            var list = Mapper.Map<List<OppStatusDto>, List<OppStatusModel>>(nList);
            //Mapper.AssertConfigurationIsValid();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddOppStatus(OppStatusModel cdfModel)
        {
            try
            {
                Mapper.CreateMap<OppStatusModel, OppStatusDto>()
                    .ForMember(dest => dest.Default, opt => opt.Ignore());

                var cdfDto = Mapper.Map<OppStatusModel, OppStatusDto>(cdfModel);
                //Mapper.AssertConfigurationIsValid();

                cdfDto.ClientID = _authUser.ClientId;
                cdfDto.Default = (cdfModel.DefaultB ? "Y" : "N");

                var cdfs = _oppStatusBL.AddOppStatus(cdfDto);
                var userAdded = new UserHandlerModel { MgUserId = 0, StrMessage = "There was an error" };

                if (cdfs != 0)
                {
                    userAdded.MgUserId = cdfs;
                    userAdded.StrMessage = "Added";
                }

                return Json(userAdded, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };
                if (e.InnerException != null)
                {
                    error.StrMessage += e.InnerException.Message;
                }

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public JsonResult DeleteOppStatuses(string cdfIdList)
        {
            try
            {
                var userDeleted = new UserHandlerModel { MgUserId = 1, StrMessage = "Successful" };

                var nList = _oppStatusBL.DeleteOppStatuses(cdfIdList);

                if (!string.IsNullOrEmpty(nList))
                {
                    userDeleted.StrMessage = nList;//
                    userDeleted.MgUserId = 2;
                }

                return Json(userDeleted, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = "Session timeout: " + e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public JsonResult UpdateOppStatusDefault(int cdfId)
        {
            try
            {
                var userEdit = new UserHandlerModel();
                var nList = _oppStatusBL.UpdateOppStatusDefault(_authUser.ClientId, cdfId);
                if (nList > 0)
                {
                    userEdit.MgUserId = 1;
                    userEdit.StrMessage = "Updated";
                }
                else
                {
                    userEdit.MgUserId = -2;
                    userEdit.StrMessage = "Error";
                }
                return Json(userEdit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut]
        public JsonResult UpdateOppStatusField(int cdfId, string field, string newValue)
        {
            try
            {
                bool isValid = true;
                if (field.Equals("Order"))
                {
                    var validValue = Int32.Parse(newValue);
                    isValid = _oppStatusBL.ValidateOrder(_authUser.ClientId, cdfId, validValue);
                }
                if (field.Equals("DefaultB"))
                {
                    if (newValue.ToUpper().Equals("TRUE"))
                    { newValue = "Y"; }
                    else
                    { newValue = "N"; }
                    field = "Default";
                }
                if (field.Equals("StageType"))
                {
                    //if (newValue.ToUpper().Equals("TRUE"))
                    //{ 
                    //newValue = "Y"; 
                    //}
                    //else
                    //{ newValue = "N"; }
                    //field = "ArchivedStatus";
                }

                var userEdit = new UserHandlerModel();
                if (isValid)
                {
                    var nList = _oppStatusBL.UpdateOppStatusField(cdfId, field, newValue);
                    if (nList > 0)
                    {
                        userEdit.MgUserId = 1;
                        userEdit.StrMessage = "Updated";
                    }
                }
                else
                {
                    userEdit.MgUserId = -2;
                    userEdit.StrMessage = "Error";
                }

                return Json(userEdit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var error = new UserHandlerModel { MgUserId = 0, StrMessage = e.Message };

                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
