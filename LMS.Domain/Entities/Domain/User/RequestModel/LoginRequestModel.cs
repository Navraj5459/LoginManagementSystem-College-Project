using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain.User.RequestModel
{
    public class LoginRequestModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? IPAddress { get; set; }
        public string? Flag { get; set; }
    }
}
