using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Corspro.Business.External;
using Corspro.Domain.Dto;
using Corspro.Reporting.App.Models;
using System.Text;

namespace Corspro.Reporting.App.Controllers
{
    public class Utils
    {

        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            char[] arr = plainText.ToCharArray();
            Array.Reverse(arr);
            plainText = new string(arr);

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);

            //return as base64 string
            return Convert.ToBase64String(data);
        }

        public static string Decrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");
            byte[] data = Convert.FromBase64String(plainText);
            string decodedString = Encoding.UTF8.GetString(data);
            char[] arr = decodedString.ToCharArray();
            Array.Reverse(arr);
            plainText = new string(arr);
            return plainText;
        }

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <param name="iCurId">The i current identifier.</param>
        /// <param name="li">The li.</param>
        /// <param name="newList">The new list.</param>
        public static void GetUserList(int iCurId, IEnumerable<UserDto> li, ICollection<UserDto> newList)
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

        public static void GetUsers(List<UserDto> li, List<UserDto> newList)
        {
            var displayUsers = (from use in li
                                join list in newList on use.ManagerUserID equals list.UserId
                                where !((from o in newList
                                         select o.UserId).Contains(use.UserId))
                                select use).ToList();

            if (displayUsers.Count > 0)
            {
                newList.AddRange(displayUsers);
                //li.RemoveAll(x => !newList.Any(y => y.UserId == x.UserId));
                GetUsers(li, newList);
            }
            return;
        }
    }
}