using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain.User.RequestModel
{
    public class ResetPasswordRequestModel
    {
        public string? Id { get; set; }
        public string? Token { get; set; }
        public string? NewPassword { get; set; }
        public string? Flag { get; set; }
    }
}
