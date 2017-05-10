using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPool.CDSConsole
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserPrincipalName { get; set; }
        public List<string> BusinessPhones { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public object JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public object OfficeLocation { get; set; }
        public string PreferredLanguage { get; set; }
        public string Surname { get; set; }
    }

    public class Users
    {
        public List<UserModel> Value { get; set; }
    }
}
