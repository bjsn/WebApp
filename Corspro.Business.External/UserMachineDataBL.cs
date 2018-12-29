using Corspro.Data.External;
using Corspro.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corspro.Business.External
{
    public class UserMachineDataBL
    {
        public UserMachineDataDto GetUserMachineData(int ClientId, int UserId, string WindowsUserName, string MACAddress) 
        {
            try
            {
                return new UserMachineDataDL().GetUserMachineData(ClientId, UserId, WindowsUserName, MACAddress);
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }

        public int AddUserMachineData(UserMachineDataDto UserMachineDataDto) 
        {
            try
            {
                return new UserMachineDataDL().AddUserMachineData(UserMachineDataDto);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int UpdateUserMachineData(UserMachineDataDto UserMachineDataDto) 
        {
            try
            {
                return new UserMachineDataDL().UpdateUserMachineData(UserMachineDataDto);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
