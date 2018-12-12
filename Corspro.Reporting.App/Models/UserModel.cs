namespace Corspro.Reporting.App.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public int ClientId { get; set; }

        public string LoginID { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? ManagerUserID { get; set; }

        public string ManagerUserName { get; set; }

        public bool Administrator { get; set; }

        public string EmailSent { get; set; }
    }
}