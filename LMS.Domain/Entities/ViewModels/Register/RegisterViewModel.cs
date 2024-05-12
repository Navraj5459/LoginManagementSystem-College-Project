using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities.ViewModels.Register
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "User Type is required.")]
        public string UserType { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must contain minimum of 8 characters.")]
        [MaxLength(20, ErrorMessage = "Password must contain maximum of 20 characters.")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,20}$", ErrorMessage = "Password does not match the password policy.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirm password does not matched")]
        public string ConfirmPassword { get; set; }
        public List<SelectListItem>? UserTypeList { get; set; }
    }
}
