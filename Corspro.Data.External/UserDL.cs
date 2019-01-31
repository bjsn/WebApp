using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System.Reflection;

namespace Corspro.Data.External
{
    public class UserDL
    {
        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto ValidateUser(UserDto clientLogin)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var myPassword = clientLogin.Password;

                    var existingUser =
                        sdaCloudEntities.Users.FirstOrDefault(
                            i =>
                            i.LoginID == clientLogin.LoginID &&
                            i.Password == myPassword && i.DeleteInd != "Y");

                    if (existingUser == null)
                    {
                        return null;
                    }

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var person = Mapper.Map<User, UserDto>(existingUser);
                    //////Mapper.AssertConfigurationIsValid();
                    return person;
                }
            }
        }

        /// <summary>
        /// Gets the user by login identifier.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto GetUserByLoginID(UserDto clientLogin)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUser =
                        sdaCloudEntities
                        .Users
                        .Where(i => i.LoginID == clientLogin.LoginID && i.DeleteInd != "Y")
                        .FirstOrDefault();
                            
                    if (existingUser == null)
                    {
                        return null;
                    }

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var person = Mapper.Map<User, UserDto>(existingUser);
                    //////Mapper.AssertConfigurationIsValid();
                    return person;
                }
            }
        }

        /// <summary>
        /// Gets the user by user identifier and client identifier.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto GetUserByUserIDAndClientID(UserDto clientLogin)
        {
            using (var SdaCloudEntities = new SDACloudEntities())
            {
                using (SdaCloudEntities)
                {
                    var existingUser =
                        SdaCloudEntities
                        .Users
                        .Where( i =>
                            i.UserID == clientLogin.UserId &&
                            i.ClientID == clientLogin.ClientId &&
                            i.DeleteInd != "Y")
                        .FirstOrDefault();

                    if (existingUser == null)
                    {
                        return null;
                    }

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var person = Mapper.Map<User, UserDto>(existingUser);
                    ////Mapper.AssertConfigurationIsValid();
                    return person;
                }
            }
        }

        /// <summary>
        /// Gets the active users.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetActiveUsers(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                        sdaCloudEntities.Users.Where(u => u.ClientID == clientId && (u.DeleteInd == null || u.DeleteInd != "Y")).ToList();

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var peopleVm = Mapper.Map<List<User>, List<UserDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetUsers(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                         sdaCloudEntities.Users.Where(u => u.ClientID == clientId).ToList();

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var peopleVm = Mapper.Map<List<User>, List<UserDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        public List<UserDto> GetUsersWithManager(int clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                         sdaCloudEntities.Users.Where(u => u.ClientID == clientId && u.ManagerUserID.HasValue).ToList();

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var peopleVm = Mapper.Map<List<User>, List<UserDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        public List<UserDto> GetManagers(UserDto clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                        sdaCloudEntities.Users.Where(u => u.ClientID == clientId.ClientId && u.DeleteInd == "N").ToList();

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var peopleVm = Mapper.Map<List<User>, List<UserDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="iCurId">The i current identifier.</param>
        /// <param name="li">The li.</param>
        /// <param name="newList">The new list.</param>
        private static void GetUserList(int iCurId, IEnumerable<UserDto> li, ICollection<UserDto> newList)
        {
            var userDtos = li as IList<UserDto> ?? li.ToList();
            foreach (var cat in userDtos)
            {
                if ((cat.ManagerUserID) != iCurId) continue;
                var tmp = cat;
                newList.Add(tmp);
                GetUserList(cat.UserId, userDtos, newList);
            }
        }

        /// <summary>
        /// Gets the user tree by manager.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetUserTreeByManager(UserDto clientId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                        sdaCloudEntities.Users.Where(u => u.ClientID == clientId.ClientId && u.DeleteInd == "N").ToList();

                    Mapper.CreateMap<User, UserDto>()
                        .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                        .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                        .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                        .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                        .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));
                    var peopleVm = Mapper.Map<List<User>, List<UserDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();

                    var liUsers = new List<UserDto>();
                    GetUserList(clientId.UserId, peopleVm, liUsers);
                    var tUser = peopleVm.FirstOrDefault(u => u.UserId == clientId.UserId);
                    liUsers.Add(tUser);

                    foreach (var u in liUsers)
                    {
                        u.ManagerUserName = peopleVm.Where(v => u.ManagerUserID != null && v.UserId == u.ManagerUserID.Value).Select(v => v.FirstName + " " + v.LastName).FirstOrDefault() ?? "-------";
                    }

                    return liUsers;
                }
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int UpdateUser(UserDto userDto)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userDto.UserId);

                    if (existingUser != null)
                    {
                        existingUser.Administrator = (userDto.Administrator ? "Y" : "N");
                        existingUser.FirstName = userDto.FirstName;
                        existingUser.LastName = userDto.LastName;
                        existingUser.LoginID = userDto.LoginID;
                        existingUser.ManagerUserID = userDto.ManagerUserID;
                        existingUser.CloudLastUpdBy = userDto.CloudLastUpdBy;
                        existingUser.CloudLastUpdDT = DateTime.UtcNow;
                    }
                    result = sdaCloudEntities.SaveChanges();
                    transactionScope.Complete();
                }
            }
            return result;
        }


        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int UpdateUserLastCheckDT(UserDto userDto)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    //&& i.ClientID == userDto.ClientId maybe not
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userDto.UserId);
                    if (existingUser != null)
                    {
                        existingUser.CloudLastUpdBy = userDto.UserId;
                        existingUser.LastCheckDT = DateTime.UtcNow;
                        existingUser.CloudLastUpdDT = DateTime.UtcNow;
                    }
                    result = sdaCloudEntities.SaveChanges();
                    transactionScope.Complete();
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the user email sent field.
        /// </summary>
        /// <param name="uList">The u list.</param>
        /// <returns></returns>
        public string UpdateUserEmailSent(string uList)
        {
            string result = string.Empty;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = uList.Split(',').ToList();
                var users = new List<User>();
                foreach (var uv in tagIds)
                {
                    int uvId;
                    Int32.TryParse(uv, out uvId);
                    var userView = sdaCloudEntities.Users.Where(q => q.UserID == uvId).SingleOrDefault();
                    if (userView != null)
                    {
                        userView.EmailSent = "Requested";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result)) result += ",";
                        result += uv;
                    }
                }

                sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Verifies the login identifier.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="loginId">The login identifier.</param>
        /// <returns></returns>
        public int VerifyLoginID(string loginId)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.LoginID == loginId);

                    if (existingUser != null)
                    {
                        if (existingUser.DeleteInd.Equals("N"))
                        {
                            result = -2;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the user field.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="field">The field.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public int UpdateUserField(int userId, string field, string newValue)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingQuote = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userId);

                        if (existingQuote != null)
                        {
                            Type t = existingQuote.GetType();
                            PropertyInfo info = t.GetProperty(field);

                            if (info == null)
                                return -1;
                            if (!info.CanWrite)
                                return -1;
                            info.SetValue(existingQuote, newValue, null);
                        }

                        result = sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the user manager.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public int UpdateUserManager(int userId, int newValue)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingQuote = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userId);

                        if (existingQuote != null)
                        {
                            if (newValue > 0)
                            {
                                existingQuote.ManagerUserID = newValue;
                            }
                            else
                            {
                                existingQuote.ManagerUserID = null;
                            }
                        }

                        result = sdaCloudEntities.SaveChanges();

                        transactionScope.Complete();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int ResetPassword(UserDto userDto)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userDto.UserId);

                    if (existingUser != null)
                    {
                        existingUser.Password = userDto.Password;
                        existingUser.CloudLastUpdBy = userDto.CloudLastUpdBy;
                        existingUser.CloudLastUpdDT = DateTime.UtcNow;
                    }

                    result = sdaCloudEntities.SaveChanges();

                    transactionScope.Complete();
                }
            }
            return result;
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int AddUser(UserDto userDto)
        {
            int result = 0;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.LoginID == userDto.LoginID);

                    if (existingUser == null)
                    {

                        existingUser = new User
                        {
                            ClientID = userDto.ClientId,
                            LoginID = userDto.LoginID,
                            Password = userDto.Password,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            ManagerUserID = userDto.ManagerUserID,
                            Administrator = (userDto.Administrator ? "Y" : "N"),
                            DeleteInd = "N",
                            CloudLastUpdBy = userDto.CloudLastUpdBy,
                            CloudLastUpdDT = DateTime.UtcNow
                        };

                        sdaCloudEntities.Users.AddObject(existingUser);
                    }
                    else
                    {
                        if (existingUser.DeleteInd.Equals("Y") && existingUser.ClientID == userDto.ClientId)
                        {
                            existingUser.Password = userDto.Password;
                            existingUser.FirstName = userDto.FirstName;
                            existingUser.LastName = userDto.LastName;
                            existingUser.ManagerUserID = userDto.ManagerUserID;
                            existingUser.Administrator = (userDto.Administrator ? "Y" : "N");
                            existingUser.DeleteInd = "N";
                            existingUser.CloudLastUpdBy = userDto.CloudLastUpdBy;
                            existingUser.CloudLastUpdDT = DateTime.UtcNow;
                        }
                        else
                        {
                            result = -2;
                        }
                    }

                    sdaCloudEntities.SaveChanges();

                    if (result != -2) result = existingUser.UserID;

                    transactionScope.Complete();
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether this instance [can delete user] the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public bool CanDeleteUser(int userId)
        {
            bool result = false;

            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<int> opportunityIds = sdaCloudEntities.Quotes.Where(qu => qu.SDALastUpdBy == userId).Select(id => id.OppID).ToList();

                List<Opportunity> opportunities = sdaCloudEntities.Opportunities.Where(opp =>
                    opp.OppStatus != 4 &&
                    opp.OppOwner != userId &&
                    opportunityIds.Contains(opp.OppID)).ToList();

                if (opportunities.Count == 0)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int DeleteUser(int userId)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                var existinguser = sdaCloudEntities.Users.FirstOrDefault(i => i.UserID == userId);
                if (existinguser != null) existinguser.DeleteInd = "Y";
                result = sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Deletes a list of users.
        /// </summary>
        /// <param name="uList">The user list.</param>
        /// <returns></returns>
        public string DeleteUsers(string uList)
        {
            string result = string.Empty;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = uList.Split(',').ToList();
                var users = new List<User>();
                foreach (var uv in tagIds)
                {
                    int uvId;
                    Int32.TryParse(uv, out uvId);
                    var userView = sdaCloudEntities.Users.Where(q => q.UserID == uvId).SingleOrDefault();
                    if (userView != null)
                    {
                        var isManager = sdaCloudEntities.Users.Where(q => q.ClientID == userView.ClientID && q.ManagerUserID == uvId && (q.DeleteInd == null || q.DeleteInd != "Y")).ToList();
                        if (isManager.Count < 1)
                        {
                            userView.DeleteInd = "Y";
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(result)) result += ",";
                            result += uv;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(result)) result += ",";
                        result += uv;
                    }
                }

                sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the user view by.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public string GetUserViewByName(UserViewDto userView)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUser =
                        sdaCloudEntities.UserViews.SingleOrDefault(
                            i =>
                            i.UserID == userView.UserID && i.ClientID == userView.ClientID && i.ViewName.ToUpper().Trim().Equals(userView.ViewName.ToUpper().Trim()));

                    if (existingUser != null)
                    {
                        return existingUser.View;
                    }

                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the user view by identifier.
        /// </summary>
        /// <param name="userViewId">The user view identifier.</param>
        /// <returns></returns>
        public string GetUserViewById(int userViewId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUser =
                        sdaCloudEntities.UserViews.FirstOrDefault(
                            i =>
                            i.UserViewID == userViewId);

                    if (existingUser != null)
                    {
                        return existingUser.View;
                    }

                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the user views by user and client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<UserViewDto> GetUserViewsByUserAndClient(int clientId, int userId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                        sdaCloudEntities.UserViews.Where(
                            i =>
                            i.ClientID == clientId && i.UserID == userId).ToList();

                    Mapper.CreateMap<UserView, UserViewDto>();
                    var peopleVm = Mapper.Map<List<UserView>, List<UserViewDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Gets the user views by user and client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserDto GetByUserIdAndClientId(int clientId, int userId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var User =
                        sdaCloudEntities.Users.Where(
                            i =>
                            i.ClientID == clientId &&
                            i.UserID == userId)
                            .FirstOrDefault();

                    Mapper.CreateMap<User, UserDto>()
                       .ForMember(dest => dest.CloudLastUpdDT, opt => opt.Ignore())
                       .ForMember(dest => dest.CloudLastUpdBy, opt => opt.Ignore())
                       .ForMember(dest => dest.ManagerUserName, opt => opt.Ignore())
                       .ForMember(dest => dest.Administrator, opt => opt.MapFrom(src => src.Administrator.ToUpper() == "Y"))
                       .ForMember(dest => dest.DeleteInd, opt => opt.MapFrom(src => src.DeleteInd.ToUpper() == "Y"));

                    var UserDto = Mapper.Map<User, UserDto>(User);
                    return UserDto;
                }
            }
        }


        public List<UserViewDto> GetAllUserViewsByUserAndClient(int clientId, int userId)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var existingUsers =
                        sdaCloudEntities.UserViews.Where(
                            i =>
                            (i.ClientID == clientId && i.UserID == userId) || i.ClientID == 999999).ToList();

                    Mapper.CreateMap<UserView, UserViewDto>();
                    var peopleVm = Mapper.Map<List<UserView>, List<UserViewDto>>(existingUsers);
                    ////Mapper.AssertConfigurationIsValid();
                    return peopleVm;
                }
            }
        }

        /// <summary>
        /// Saves the new user view.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public int SaveNewUserView(UserViewDto userView)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var nList = GetUserViewsByUserAndClient(userView.ClientID, userView.UserID);

                    if (nList.Count < 30)
                    {
                        var existName = nList.Where(u => u.ViewName.ToUpper().Equals(userView.ViewName.ToUpper())).FirstOrDefault();
                        if ((existName == null) && (!userView.ViewName.ToUpper().Equals("DEFAULT")))
                        {
                            var existingUser = new UserView
                            {
                                UserID = userView.UserID,
                                ClientID = userView.ClientID,
                                View = userView.View,
                                ViewName = userView.ViewName
                            };

                            sdaCloudEntities.UserViews.AddObject(existingUser);

                            sdaCloudEntities.SaveChanges();

                            var existingUv = sdaCloudEntities.UserViews.Where(u => u.ClientID == userView.ClientID && u.UserID == userView.UserID && u.ViewName.ToUpper().Equals(userView.ViewName.ToUpper())).FirstOrDefault();

                            result = existingUv.UserViewID;

                            transactionScope.Complete();
                        }
                        else if ((existName != null) && (!userView.ViewName.ToUpper().Equals("DEFAULT")))
                        {
                            var existingUv = sdaCloudEntities.UserViews.Where(u => u.ClientID == userView.ClientID && u.UserID == userView.UserID && u.ViewName.ToUpper().Equals(userView.ViewName.ToUpper())).FirstOrDefault();
                            existingUv.View = userView.View;

                            sdaCloudEntities.SaveChanges();

                            result = existingUv.UserViewID;

                            transactionScope.Complete();
                        }
                        else
                        {
                            result = -2;
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the user view.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public int UpdateUserView(UserViewDto userView)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    UserView exist;

                    if (userView.UserViewID != 0)
                    {
                        exist = sdaCloudEntities.UserViews.SingleOrDefault(u => u.UserViewID == userView.UserViewID);
                    }
                    else
                    {
                        exist =
                            sdaCloudEntities.UserViews.SingleOrDefault(
                                i =>
                                i.UserID == userView.UserID && i.ClientID == userView.ClientID && i.ViewName.ToUpper().Trim().Equals(userView.ViewName.ToUpper().Trim()));
                    }

                    if (exist != null)
                    {
                        exist.View = userView.View;
                        sdaCloudEntities.SaveChanges();

                        result = exist.UserViewID;

                        transactionScope.Complete();
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Saves the name of the user view by.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public int SaveUserViewByName(UserViewDto userView)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.UserViews.FirstOrDefault(i => i.UserID == userView.UserID && i.ClientID == userView.ClientID && i.ViewName.ToUpper() == userView.ViewName.ToUpper());

                    if (existingUser != null)
                    {
                        existingUser.View = userView.View;
                    }
                    else
                    {
                        existingUser = new UserView
                        {
                            UserID = userView.UserID,
                            ClientID = userView.ClientID,
                            View = userView.View,
                            ViewName = userView.ViewName
                        };

                        sdaCloudEntities.UserViews.AddObject(existingUser);
                    }

                    sdaCloudEntities.SaveChanges();

                    result = existingUser.UserViewID;

                    transactionScope.Complete();
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the user views.
        /// </summary>
        /// <param name="uvList">The uv list.</param>
        /// <returns></returns>
        public int DeleteUserViews(string uvList)
        {
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<string> tagIds = uvList.Split(',').ToList();
                var userViews = new List<UserView>();
                foreach (var uv in tagIds)
                {
                    int uvId;
                    Int32.TryParse(uv, out uvId);
                    var userView = sdaCloudEntities.UserViews.Where(q => q.UserViewID == uvId).SingleOrDefault();
                    if (userView != null) userViews.Add(userView);
                }
                foreach (var userview in userViews)
                {
                    //sdaCloudEntities.DeleteObject(userview);
                }
                result = sdaCloudEntities.SaveChanges();
            }
            return result;
        }

        public int updateEmailRequested(String email)
        {
            email = email.ToLower();
            int result;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUser = sdaCloudEntities.Users.FirstOrDefault(i => i.LoginID == email);
                    if (existingUser != null)
                    {
                        existingUser.EmailSent = "Requested";
                    }
                    result = sdaCloudEntities.SaveChanges();
                    transactionScope.Complete();
                    string ServiceName = Utilitary.GetConfigurationVariable("EmailService");
                    Utilitary.IsServiceIsRunning(ServiceName);
                }
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        public void UpdateUserRegistrationDT(int userID, int clientID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUsers = sdaCloudEntities.Users.FirstOrDefault(i =>
                            i.ClientID == clientID && i.UserID == userID);

                    if (existingUsers != null)
                    {
                        if (existingUsers.RegistrationDT == null)
                        {
                            existingUsers.RegistrationDT = DateTime.UtcNow;
                            sdaCloudEntities.SaveChanges();
                        }
                    }
                    transactionScope.Complete();
                }
            }
        }

        public void UpdateUserLastCheckDT(int userID, int clientID)
        {
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (var transactionScope = new TransactionScope())
                {
                    var existingUsers =
                        sdaCloudEntities.Users.FirstOrDefault(i =>
                            i.ClientID == clientID && i.UserID == userID);

                    if (existingUsers != null)
                    {
                        existingUsers.LastCheckDT = DateTime.UtcNow;
                        sdaCloudEntities.SaveChanges();
                    }
                    transactionScope.Complete();
                }
            }
        }
    }
}
