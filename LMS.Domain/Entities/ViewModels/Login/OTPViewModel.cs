using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.ViewModels.Login
{
    public class OTPViewModel
    {
        [Required(ErrorMessage = "Id is required.")]
        public string? Id { get; set; }
        [Required(ErrorMessage = "OTP is required.")]
        public string? OTP { get; set; }
    }
}
