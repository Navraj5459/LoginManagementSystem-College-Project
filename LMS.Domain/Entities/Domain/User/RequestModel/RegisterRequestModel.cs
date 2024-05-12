using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.Domain.User.RequestModel
{
    public class RegisterRequestModel
    {
        public string? FullName { get; set; }
        public string? UserType { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Flag { get; set; }
    }
}
