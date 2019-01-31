using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Corspro.Data.External
{
    public class UserMachineDataDL
    {
        /// <summary>
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="UserId"></param>
        /// <param name="WindowsUserName"></param>
        /// <param name="MACAddress"></param>
        /// <returns></returns>
        public UserMachineDataDto GetUserMachineData(int ClientId, int UserId, string WindowsUserName, string MACAddress) 
        {
            try
            {
                using (var sdaCloudEntities = new SDACloudEntities())
                {
                    using (sdaCloudEntities)
                    {
                        var existingEntity = sdaCloudEntities
                                            .UserMachineDatas
                                            .Where(i => i.ClientID == ClientId && i.UserID == UserId && i.WindowsUserName.Equals(WindowsUserName) && i.MACAddress.Equals(MACAddress))
                                            .FirstOrDefault();
                        if (existingEntity != null)
                        {
                            Mapper.CreateMap<UserMachineData, UserMachineDataDto>()
                                .ForMember(x => x.LastUpdDT, opt => opt.Ignore())
                                .ForMember(x => x.CreateDT, opt => opt.Ignore());
                            var dto = Mapper.Map<UserMachineData, UserMachineDataDto>(existingEntity);
                            return dto;
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="UserMachineDataDto"></param>
        /// <returns></returns>
        public int AddUserMachineData(UserMachineDataDto UserMachineDataDto) 
        {
            int result = 0;
            try
            {
                using (var sdaCloudEntities = new SDACloudEntities())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingEntity = sdaCloudEntities
                                           .UserMachineDatas
                                           .Where(i => i.ClientID == UserMachineDataDto.ClientID && i.UserID == UserMachineDataDto.UserID && i.WindowsUserName.Equals(UserMachineDataDto.WindowsUserName) && i.MACAddress.Equals(UserMachineDataDto.MACAddress))
                                           .FirstOrDefault();
                        if (existingEntity == null)
                        {
                            existingEntity = new UserMachineData()
                            {
                                ClientID = UserMachineDataDto.ClientID,
                                UserID = UserMachineDataDto.UserID,
                                WindowsUserName = UserMachineDataDto.WindowsUserName,
                                MACAddress = UserMachineDataDto.MACAddress,
                                VersionDotNet = UserMachineDataDto.VersionDotNet,
                                VersionExcel = UserMachineDataDto.VersionExcel,
                                VersionWord = UserMachineDataDto.VersionWord,
                                VersionSDA = UserMachineDataDto.VersionSDA,
                                VersionSalesManager = UserMachineDataDto.VersionSalesManager,
                                VersionWindows = UserMachineDataDto.VersionWindows,
                                InstallType = UserMachineDataDto.InstallType,
                                UserName = UserMachineDataDto.UserName,
                                Email = UserMachineDataDto.Email,
                                Company = UserMachineDataDto.Company,
                                Title = UserMachineDataDto.Title,
                                Phone = UserMachineDataDto.Phone,
                                UserTimeZone = UserMachineDataDto.UserTimeZone,
                                CreateDT = DateTime.UtcNow,
                                LastUpdDT = DateTime.UtcNow
                            };
                            sdaCloudEntities.UserMachineDatas.AddObject(existingEntity);
                            result = sdaCloudEntities.SaveChanges();
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="UserMachineDataDto"></param>
        /// <returns></returns>
        public int UpdateUserMachineData(UserMachineDataDto UserMachineDataDto) 
        {
            int result = 0;
            try
            {
                using (var sdaCloudEntities = new SDACloudEntities())
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        var existingEntity = sdaCloudEntities
                                           .UserMachineDatas
                                           .Where(i => i.ClientID == UserMachineDataDto.ClientID && i.UserID == UserMachineDataDto.UserID && i.WindowsUserName.Equals(UserMachineDataDto.WindowsUserName) && i.MACAddress.Equals(UserMachineDataDto.MACAddress))
                                           .FirstOrDefault();
                        if (existingEntity != null)
                        {
                            existingEntity.VersionDotNet = UserMachineDataDto.VersionDotNet;
                            existingEntity.VersionExcel = UserMachineDataDto.VersionExcel;
                            existingEntity.VersionWord = UserMachineDataDto.VersionWord;
                            existingEntity.VersionSDA = UserMachineDataDto.VersionSDA;
                            existingEntity.VersionSalesManager = UserMachineDataDto.VersionSalesManager;
                            existingEntity.VersionWindows = UserMachineDataDto.VersionWindows;
                            existingEntity.InstallType = UserMachineDataDto.InstallType;
                            existingEntity.UserName = UserMachineDataDto.UserName;
                            existingEntity.Email = UserMachineDataDto.Email;
                            existingEntity.Company = UserMachineDataDto.Company;
                            existingEntity.Title = UserMachineDataDto.Title;
                            existingEntity.Phone = UserMachineDataDto.Phone;
                            existingEntity.UserTimeZone = UserMachineDataDto.UserTimeZone;
                            existingEntity.LastUpdDT = DateTime.UtcNow;
                            result = sdaCloudEntities.SaveChanges();
                            transactionScope.Complete();
                        }
                    }
                }
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
            return result;
        }

    }
}
