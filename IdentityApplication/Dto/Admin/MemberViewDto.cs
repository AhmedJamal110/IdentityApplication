using System;
using System.Collections;
using System.Collections.Generic;

namespace IdentityApplication.API.Dto.Admin
{
    public class MemberViewDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public bool IsLooked { get; set; }

        public DateTime DateCreated { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
