using System.Collections.Generic;
using Corspro.Data.External;
using Corspro.Domain.Dto;

namespace Corspro.Business.External
{
    public class UserBL
    {
        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto ValidateUser(UserDto clientLogin)
        {
            var userDl = new UserDL();
            return userDl.ValidateUser(clientLogin);
        }

        /// <summary>
        /// Gets the user by login identifier and password.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto GetUserByLoginIDAndPassword(UserDto clientLogin)
        {
            var userDl = new UserDL();
            return userDl.ValidateUser(clientLogin);
        }

        /// <summary>
        /// Gets the user by user identifier and client identifier.
        /// </summary>
        /// <param name="clientLogin">The client login.</param>
        /// <returns></returns>
        public UserDto GetUserByUserIDAndClientID(UserDto clientLogin)
        {
            var userDl = new UserDL();
            return userDl.GetUserByUserIDAndClientID(clientLogin);
        }

        /// <summary>
        /// Gets the active users.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetActiveUsers(int clientId)
        {
            var userDl = new UserDL();
            return userDl.GetActiveUsers(clientId);
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetUsers(int clientId)
        {
            var userDl = new UserDL();
            return userDl.GetUsers(clientId);
        }

        public List<UserDto> GetUsersWithManager(int clientId)
        {
            var userDl = new UserDL();
            return userDl.GetUsersWithManager(clientId);
        }

        /// <summary>
        /// Gets the managers.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetManagers(UserDto clientId)
        {
            var userDl = new UserDL();
            return userDl.GetManagers(clientId);
        }

        /// <summary>
        /// Gets the user tree by manager.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public List<UserDto> GetUserTreeByManager(UserDto clientId)
        {
            var userDl = new UserDL();
            return userDl.GetUserTreeByManager(clientId);
        }

        /// <summary>
        /// Determines whether this instance [can delete user] the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public bool CanDeleteUser(int userId)
        {
            var userDl = new UserDL();
            return userDl.CanDeleteUser(userId);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public int DeleteUser(int userId)
        {
            var userDl = new UserDL();
            return userDl.DeleteUser(userId);
        }

        /// <summary>
        /// Deletes the users.
        /// </summary>
        /// <param name="uList">The u list.</param>
        /// <returns></returns>
        public string DeleteUsers(string uList)
        {
            var userDl = new UserDL();
            string result = userDl.DeleteUsers(uList);

            return result;
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int UpdateUser(UserDto userDto)
        {
            var userDl = new UserDL();
            return userDl.UpdateUser(userDto);
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
            var userDl = new UserDL();
            return userDl.UpdateUserField(userId, field, newValue);
        }

        /// <summary>
        /// Updates the user manager.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        public int UpdateUserManager(int userId, int newValue)
        {
            var userDl = new UserDL();
            return userDl.UpdateUserManager(userId, newValue);
        }

        /// <summary>
        /// Updates the user email sent.
        /// </summary>
        /// <param name="uList">The u list.</param>
        /// <returns></returns>
        public string UpdateUserEmailSent(string uList)
        {
            var userDl = new UserDL();
            return userDl.UpdateUserEmailSent(uList);
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int ResetPassword(UserDto userDto)
        {
            var userDl = new UserDL();
            return userDl.ResetPassword(userDto);
        }

        /// <summary>
        /// Adds the user.
        /// </summary>
        /// <param name="userDto">The user dto.</param>
        /// <returns></returns>
        public int AddUser(UserDto userDto)
        {
            var userDl = new UserDL();
            return userDl.AddUser(userDto);
        }

        /// <summary>
        /// Gets the name of the user view by.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public string GetUserViewByName(UserViewDto userView)
        {
            var userDl = new UserDL();
            return userDl.GetUserViewByName(userView);
        }

        /// <summary>
        /// Gets the user view by identifier.
        /// </summary>
        /// <param name="userViewId">The user view identifier.</param>
        /// <returns></returns>
        public string GetUserViewById(int userViewId)
        {
            var userDl = new UserDL();
            return userDl.GetUserViewById(userViewId);
        }

        /// <summary>
        /// Gets the user views by user and client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<UserViewDto> GetUserViewsByUserAndClient(int clientId, int userId)
        {
            var userDl = new UserDL();
            return userDl.GetUserViewsByUserAndClient(clientId, userId);
        }

        public List<UserViewDto> GetAllUserViewsByUserAndClient(int clientId, int userId)
        {
            var userDl = new UserDL();
            return userDl.GetAllUserViewsByUserAndClient(clientId, userId);
        }


        /// <summary>
        /// Gets the user views by user and client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public UserDto GetByUserIdAndClientId(int clientId, int userId)
        {
            var userDl = new UserDL();
            return userDl.GetByUserIdAndClientId(clientId, userId);
        }

        /// <summary>
        /// Saves the new user view.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public int SaveNewUserView(UserViewDto userView)
        {
            var userDl = new UserDL();
            return userDl.SaveNewUserView(userView);
        }

        public int UpdateUserView(UserViewDto userView)
        {
            var userDl = new UserDL();
            return userDl.UpdateUserView(userView);
        }

        /// <summary>
        /// Saves the name of the user view by.
        /// </summary>
        /// <param name="userView">The user view.</param>
        /// <returns></returns>
        public int SaveUserViewByName(UserViewDto userView)
        {
            var userDl = new UserDL();
            return userDl.SaveUserViewByName(userView);
        }

        /// <summary>
        /// Deletes the user views.
        /// </summary>
        /// <param name="uvList">The uv list.</param>
        /// <returns></returns>
        public int DeleteUserViews(string uvList)
        {
            var userDl = new UserDL();
            int result = userDl.DeleteUserViews(uvList);

            return result;
        }

        public void updateEmailRequested(string email)
        {
            var userDl = new UserDL();
            userDl.updateEmailRequested(email);
        }

        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="clientID"></param>
        public void UpdateUserRegistrationDT(int userID, int clientID)
        {
            var userDl = new UserDL();
            userDl.UpdateUserRegistrationDT(userID, clientID);
        }

        public void UpdateUserLastCheckDT(int userID, int clientID)
        {
            var userDl = new UserDL();
            userDl.UpdateUserLastCheckDT(userID, clientID);
        }
    }
}
