using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain.User.ResponseModel
{
    public class LoginResponseModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? UserType { get; set; }
        public string? ForcePasswordChange { get; set; }
        public string? PasswordExpiryDate { get; set; }
    }
}
