﻿namespace GamingShop.Data.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int CartID { get; set; }
        public int UserID { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

    }
}
