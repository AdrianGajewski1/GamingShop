﻿using GamingShop.Web.ViewModels;
using System.Collections.Generic;

namespace GamingShop.Web.Models
{
    public class HomeIndexModel
    {
        public IEnumerable<GameIndexViewModel> Games { get; set; }
    }
}
