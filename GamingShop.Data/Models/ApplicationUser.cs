﻿using Microsoft.AspNetCore.Identity;

namespace GamingShop.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int CartID { get; set; }
        public string Password { get; set; }
        public string ImageUrl { get; set; }
    }
}
