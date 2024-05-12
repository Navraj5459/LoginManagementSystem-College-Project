using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.ViewModels.Login
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must contain minimum of 8 characters.")]
        [MaxLength(20, ErrorMessage = "Password must contain maximum of 20 characters.")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$", ErrorMessage = "Password does not match the password policy.")]
        public string Password { get; set; }
    }
}
