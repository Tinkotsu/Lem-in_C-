using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ChangeUserRoleModel
    {
        public string Id { get; set; }
        public string NewRole { get; set; }
    }
}
