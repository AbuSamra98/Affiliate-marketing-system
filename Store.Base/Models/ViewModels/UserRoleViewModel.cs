using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            Claims = new List<ClaimsForUser>();
        }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public List<ClaimsForUser> Claims { get; set; }

    }

    public class ClaimsForUser
    {
        //public string ClaimId { get; set; }
        public string ClaimName { get; set; }
        public bool IsSelected { get; set; }
    }
}
