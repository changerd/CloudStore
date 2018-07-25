using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IBAstore.Models
{
    public class CreateRoleModel
    {
        public string Name { get; set; }       
    }
    public class EditRoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }   
}