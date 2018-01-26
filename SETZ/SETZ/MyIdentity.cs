using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
namespace SETZ
{
    public class MyIdentity :IIdentity
    {
        public IIdentity Identity { get; set; }
        public User User { get; set; }
        public MyIdentity(User user)
        {
            Identity = new GenericIdentity(user.PhoneNumber);
            User = user;
        }
        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return Identity.IsAuthenticated; }
        }

        public string Name
        {
            get { return Identity.Name; }
        }
    }
}