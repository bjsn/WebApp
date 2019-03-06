using AutoMapper;
using Corspro.Domain.Dto;
using Corspro.Domain.External;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Corspro.Data.External
{
    public class ClientUpdateDBDL
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public bool HasPermissionToUpload(int clientId, string fileName)
        {
            bool HasPermissionToUpload = false;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var clientBase = sdaCloudEntities.ClientUpdateDBs
                                    .Where(i => i.ClientID == clientId && i.DBFileName == fileName)
                                    .ToList();

                    HasPermissionToUpload = (clientBase.Count > 0);
                }
            }
            return HasPermissionToUpload;
        }


        /// <summary>
        ///  0 = Normal user
        ///  1 = Administrator
        ///  2 = EndUser
        ///  SDA 1.5.0.1
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientUpdateDBDto> GetClientUpdateDB(int clientId, int userType)
        {
            List<ClientUpdateDBDto> clientBase = new List<ClientUpdateDBDto>();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var clientUpdates = new List<ClientUpdateDB>();
                    if (userType == 0)
                    {
                        clientUpdates = sdaCloudEntities
                                        .ClientUpdateDBs
                                        .Where(i => i.ClientID == clientId)
                                        .ToList();
                    }
                    else if (userType == 1)
                    {
                        clientUpdates = sdaCloudEntities
                                                    .ClientUpdateDBs
                                                    .Where(i => (i.ClientID == 999999 &&
                                                          (i.DBFileType == null || i.DBFileType == "" || i.DBFileType == "Admin")))
                                                     .ToList();

                        List<ClientUpdateDB> clientBaseContent = GetClientUpdateDBByClientContentId(clientId);
                        clientUpdates.AddRange(clientBaseContent);
                    }
                    else if (userType == 2)
                    {
                        clientUpdates = sdaCloudEntities
                                                      .ClientUpdateDBs
                                                      .Where(i => (i.ClientID == 999999 &&
                                                            (i.DBFileType == null || i.DBFileType == "")))
                                                       .ToList();
                    }

                    Mapper.CreateMap<ClientUpdateDB, ClientUpdateDBDto>()
                        .ForSourceMember(x => x.BetaUploaderClientID, opt => opt.Ignore())
                        .ForSourceMember(x => x.BetaUploaderUserID, opt => opt.Ignore())
                        .ForSourceMember(x => x.BetaUploaderUserName, opt => opt.Ignore());

                    if (clientUpdates != null)
                    {
                        clientBase = Mapper.Map<List<ClientUpdateDB>, List<ClientUpdateDBDto>>(clientUpdates);
                        Mapper.AssertConfigurationIsValid();
                    }
                }
            }
            return clientBase;
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientUpdateDB> GetClientUpdateDBByClientContentId(int clientId)
        {
            List<ClientUpdateDB> clientBase = new List<ClientUpdateDB>();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                List<int> clientContentId = new List<int>();
                //get all ids in contentClientTable
                clientContentId = sdaCloudEntities.ClientContents
                                .Where(i => i.ClientID == clientId)
                                .Select(i => i.ContentClientID)
                                .ToList();

                if (clientContentId.Count > 0)
                {
                    clientBase = sdaCloudEntities
                        .ClientUpdateDBs
                        .Where(i => clientContentId.Contains(i.ClientID.Value))
                        .ToList();
                }
            }

            foreach (var clientContent in clientBase)
            {
                if (!String.IsNullOrEmpty(clientContent.DBFileName))
                {
                    clientContent.DBFileName = (clientContent.DBDescription + Path.GetExtension(clientContent.DBFileName));
                }
            }
            return clientBase;
        }
    

        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="fileName"></param>
        /// <param name="AWSFilePath"></param>
        /// <param name="AWSFileName"></param>
        /// <param name="DBFileUpdDT"></param>
        /// <param name="DBUploadedDt"></param>
        /// <returns></returns>
        public ClientUpdateDBDto UpdateClientUpdateDB(int ClientId, string FileName, string AWSID, string AWSFilePath,
                                                      string AWSFileName, string DBFileUpdDT, string DBUploadedDt, bool BetaVersion,
                                                      int UploaderClientID, int UploaderUserID, string UploaderUserName)
        {
            ClientUpdateDBDto clientBase = null;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    ClientUpdateDB client = sdaCloudEntities.ClientUpdateDBs
                                            .Where(i => i.ClientID == ClientId && i.DBFileName == FileName)
                                            .FirstOrDefault();
                    if (client != null)
                    {
                        if (!BetaVersion)
                        {
                            client.AWSFlePath = AWSFilePath;
                            client.AWSFileName = AWSFileName;
                            client.AWSFileID = AWSID;
                            client.DBFileUpdDT = DateTime.Parse(DBFileUpdDT);
                            client.DBUploadedDT = DateTime.Parse(DBUploadedDt);
                        }
                        else
                        {
                            client.BetaAwsFilePath = AWSFilePath;
                            client.BetaAWSFileName = AWSFileName;
                            client.BetaAWSFileID = AWSID;
                            client.BetaDBFileUpdDT = DateTime.Parse(DBFileUpdDT);
                            client.BetaUploaderClientID = UploaderClientID;
                            client.BetaUploaderUserID = UploaderUserID;
                            client.BetaUploaderUserName = UploaderUserName;
                        }
                        client.UploaderClientID = UploaderClientID;
                        client.UploaderUserID = UploaderUserID;
                        client.UploaderUserName = UploaderUserName;

                        Mapper.CreateMap<ClientUpdateDB, ClientUpdateDBDto>()
                          .ForSourceMember(x => x.BetaUploaderClientID, opt => opt.Ignore())
                          .ForSourceMember(x => x.BetaUploaderUserID, opt => opt.Ignore())
                          .ForSourceMember(x => x.BetaUploaderUserName, opt => opt.Ignore());

                        if (client != null)
                        {
                            clientBase = Mapper.Map<ClientUpdateDB, ClientUpdateDBDto>(client);
                            Mapper.AssertConfigurationIsValid();
                        }
                        sdaCloudEntities.SaveChanges();
                    }
                }
            }
            return clientBase;
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetClientNameByIdAndUserId(int clientId, int userId)
        {
            string clientName = "";
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                User user = sdaCloudEntities.Users
                            .Where(i => i.ClientID == clientId && i.UserID == userId)
                            .FirstOrDefault();

                if (user != null)
                {
                    if (user.DeleteInd.ToString().ToUpper().Equals("Y"))
                    {
                        clientName = "Error 405: The user exist but is not available";
                    }
                    else
                    {
                        Client client = sdaCloudEntities.Clients
                                        .Where(i => i.ClientID == clientId)
                                        .FirstOrDefault();

                        if (client != null)
                        {
                            clientName = client.ClientName;
                        }
                        else
                        {
                            clientName = "Error 404: The client does not exist in the clients table";
                        }
                    }
                }
                else
                {
                    clientName = "Error 404: The user does not exist";
                }
            }
            return clientName;
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ClientUpdateDBDto GetLastCloudDBFileUpdDT(int clientId, string dbName)
        {
            ClientUpdateDBDto clientUpdate = null;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                ClientUpdateDB client = sdaCloudEntities.ClientUpdateDBs
                                            .Where(i => i.ClientID == clientId && i.DBFileName == dbName)
                                            .SingleOrDefault();
                Mapper.CreateMap<ClientUpdateDB, ClientUpdateDBDto>()
                      .ForSourceMember(x => x.BetaUploaderClientID, opt => opt.Ignore())
                      .ForSourceMember(x => x.BetaUploaderUserID, opt => opt.Ignore())
                      .ForSourceMember(x => x.BetaUploaderUserName, opt => opt.Ignore());
                if (client != null)
                {
                    clientUpdate = Mapper.Map<ClientUpdateDB, ClientUpdateDBDto>(client);
                    Mapper.AssertConfigurationIsValid();
                }
            }
            return clientUpdate;
        }


        /// <summary>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public ClientUpdateDBBetaDto GetLastCloudDBFileUpdDTBeta(int clientId, string dbName)
        {
            ClientUpdateDBBetaDto clientUpdate = null;
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                ClientUpdateDB client = sdaCloudEntities.ClientUpdateDBs
                                            .Where(i => i.ClientID == clientId && i.DBFileName == dbName)
                                            .SingleOrDefault();
                Mapper.CreateMap<ClientUpdateDB, ClientUpdateDBBetaDto>();
                if (client != null)
                {
                    clientUpdate = Mapper.Map<ClientUpdateDB, ClientUpdateDBBetaDto>(client);
                    Mapper.AssertConfigurationIsValid();
                }
            }
            return clientUpdate;
        }


        /// <summary>
        ///  0 = Normal user
        ///  1 = Administrator
        ///  2 = EndUser
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<ClientUpdateDBBetaDto> GetClientUpdateDBList(int clientId, int userType)
        {
            List<ClientUpdateDBBetaDto> clientBase = new List<ClientUpdateDBBetaDto>();
            using (var sdaCloudEntities = new SDACloudEntities())
            {
                using (sdaCloudEntities)
                {
                    var clientUpdates = new List<ClientUpdateDB>();
                    if (userType == 0)
                    {
                        clientUpdates = sdaCloudEntities
                                        .ClientUpdateDBs
                                        .Where(i => i.ClientID == clientId)
                                        .ToList();
                    }
                    else if (userType == 1)
                    {
                        clientUpdates = sdaCloudEntities
                                                    .ClientUpdateDBs
                                                    .Where(i => (i.ClientID == 999999 &&
                                                          (i.DBFileType == null || i.DBFileType == "" || i.DBFileType == "Admin")))
                                                     .ToList();

                        List<ClientUpdateDB> clientBaseContent = GetClientUpdateDBByClientContentId(clientId);
                        clientUpdates.AddRange(clientBaseContent);
                    }
                    else if (userType == 2)
                    {
                        clientUpdates = sdaCloudEntities
                                                      .ClientUpdateDBs
                                                      .Where(i => (i.ClientID == 999999 &&
                                                            (i.DBFileType == null || i.DBFileType == "")))
                                                       .ToList();
                    }
                    Mapper.CreateMap<ClientUpdateDB, ClientUpdateDBBetaDto>();
                    if (clientUpdates != null)
                    {
                        clientBase = Mapper.Map<List<ClientUpdateDB>, List<ClientUpdateDBBetaDto>>(clientUpdates);
                        Mapper.AssertConfigurationIsValid();
                    }
                }
            }
            return clientBase;
        }
        
    }
}
