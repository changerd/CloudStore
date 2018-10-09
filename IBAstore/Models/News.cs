using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class News
    {
        [Required]
        [Display(Name = "Тема новости")]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Текст новости")]
        public string Body { get; set; }
    }
}