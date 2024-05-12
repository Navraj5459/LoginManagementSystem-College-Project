using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.ViewModels.User
{
    public class ResetPasswordViewModel
    {
        public string? Id { get; set; }
        public string? Token { get; set; }
        public string? FullName { get; set; }
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must contain minimum of 8 characters.")]
        [MaxLength(20, ErrorMessage = "New password must contain maximum of 20 characters.")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$", ErrorMessage = "New password does not match the password policy.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not matched")]
        public string ConfirmPassword { get; set; }
    }
}
