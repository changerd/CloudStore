using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudStore.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Login")]
        public string RegisterLogin { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression(".+\\@.+\\..+")]
        public string RegisterEmail { get; set; }    

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string RegisterPassword { get; set; }

        [Required]
        [Compare("RegisterPassword", ErrorMessage = "Password do not match")]
        [Display(Name = "Confirm the password")]
        [DataType(DataType.Password)]
        public string RegisterPasswordConfirm { get; set; }
    }
}