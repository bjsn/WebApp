using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Net.Mail;
using System.Configuration;

namespace Corspro.Data.External
{
    public class Utilitary
    {
        public static bool IsServiceIsRunning(string serviceName) 
        {
            ServiceController sc = null;
            try
            {
                sc = new ServiceController(serviceName);
                ServiceControllerStatus status;
                sc.Refresh(); // calling sc.Refresh() is unnecessary on the first use of `Status` but if you keep the ServiceController in-memory then be sure to call this if you're using it periodically.
                status = sc.Status;
                if (status == ServiceControllerStatus.Running) 
                {
                    return true;
                }
                else if (status == ServiceControllerStatus.Stopped || status == ServiceControllerStatus.Paused) 
                {
                    try
                    {
                        sc.Start();
                        return true;
                    }
                    catch (Exception e) 
                    {
                        string mjs = "The service " + serviceName + " is not running \n " + "Error " + e.Message;
                        SendErrorServiceEmail(mjs);
                        return false;
                    }
                } 
            }
            catch (ArgumentException ae) 
            {
                string mjs = "The service " + serviceName + " is not running \n " + "Because: " + ae.Message;
                SendErrorServiceEmail(mjs);
                return false;
            }
            return false;
        }


        private static void SendErrorServiceEmail(string error) 
        {
            try 
            {
                MailMessage mail = new MailMessage("support@corspro.com", "support@corspro.com");
                SmtpClient client = new SmtpClient("CorsPro-com.mail.protection.outlook.com", 25);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                mail.Subject = "Error trying to run Email Windows Service";
                mail.Body = error;
                client.Send(mail);
            }
            catch(Exception)
            {}
        }


        public static string GetConfigurationVariable(string name) 
        {
            string value = "";
            try 
            {
                value = ConfigurationSettings.AppSettings[name];
            }
            catch(Exception)
            { }
            return value;
        }
    }
}
