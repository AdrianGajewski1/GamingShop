﻿using System;

namespace GamingShop.Web.API.Models
{
    public class NewMessage
    {
 
        public int ID { get; set; }
        public int GameID { get; set; }
        public string SenderID { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientID { get; set; }
        public string RecipientEmail { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public bool Read { get; set; }
        public bool Replying { get; set; }
        public DateTime Sent { get; set; }
    }
}
