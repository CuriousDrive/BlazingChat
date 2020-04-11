using System;
using System.Collections.Generic;

namespace BlazingChat.Server.Models
{
    public partial class ChatHistory
    {
        public long ChatHistoryId { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public string Message { get; set; }
        public byte[] CreatedDate { get; set; }

        public virtual User FromUser { get; set; }
        public virtual User ToUser { get; set; }
    }
}
