using System;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Application.Queries.Models
{
    public class ApplicationUser: IdentityUser
    {
        [PersonalData]
        public string Ho_ten { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }
    }
}