﻿using GamingShop.Data.Models;
using GamingShop.Web.API.Pagination;
using System.Collections.Generic;

namespace GamingShop.Web.API.Models.Response
{
    public class MessageResponseModel
    {
        public IEnumerable<Message> MessagesSentByUser { get; set; }
        public IEnumerable<Message> MessagesSentToUser { get; set; }
        public IEnumerable<Message> NewMessages { get; set; }
        public int NewMessagesAvailable{ get; set; }
        public PaginatedResponseInfo ResponseInfo { get; set; }
    }
}
