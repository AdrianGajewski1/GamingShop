﻿using System;

namespace GamingShop.Web.API.Models
{
    public class UpdateGameModel 
    {
        public int ID { get; set; }
        public int Pegi { get; set; }
        public decimal Price { get; set; }
        public bool BestSeller { get; set; }
        public bool Sold { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Producent { get; set; }
        public string Platform { get; set; }
        public string Type { get; set; }
        public string OwnerID { get; set; }
        public DateTime Posted { get; set; }
        public string LaunchDate { get; set; }
    }
}
