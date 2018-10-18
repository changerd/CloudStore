using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{    
    public class CreateRoleModel
    {
        [Required]
        public string RoleName { get; set; }       
    }
    public class EditRoleModel
    {
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
    public class RoleEditModel
    {
        public ApplicationRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}