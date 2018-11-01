using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string RegisterLogin { get; set; }

        [Required]
        [Display(Name = "Электронная почта")]
        [RegularExpression(".+\\@.+\\..+")]
        public string RegisterEmail { get; set; }    

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string RegisterPassword { get; set; }

        [Required]
        [Compare("RegisterPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        public string RegisterPasswordConfirm { get; set; }
    }
}