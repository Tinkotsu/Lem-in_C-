using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace WebApi.DAL.Entities.User
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
    }
}
